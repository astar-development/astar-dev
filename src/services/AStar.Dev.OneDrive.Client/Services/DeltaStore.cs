using System;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace AStar.Dev.OneDrive.Client.Services;

public partial class DeltaStore
{
    private readonly string _connectionString;
    private readonly ILogger<OneDriveService> _logger;

    public DeltaStore(ILogger<OneDriveService> logger, string dbPath = "onedrive_sync.db")
    {
        _connectionString = $"Data Source={dbPath}";
        Initialize();
        _logger = logger;
    }

    private void Initialize()
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS DeltaState (
                DriveId TEXT PRIMARY KEY,
                DeltaToken TEXT NOT NULL,
                LastSyncedUtc TEXT NOT NULL
            );
            CREATE TABLE IF NOT EXISTS DriveItems (
                Id TEXT PRIMARY KEY,
                Name TEXT,
                IsFolder INTEGER,
                LastModifiedUtc TEXT,
                ParentPath TEXT NULL,
                ETag TEXT NULL
            );

            -- Indexes for fast lookups
            CREATE INDEX IF NOT EXISTS idx_driveitems_parentpath ON DriveItems(ParentPath);
            CREATE INDEX IF NOT EXISTS idx_driveitems_lastmodified ON DriveItems(LastModifiedUtc);
            CREATE INDEX IF NOT EXISTS idx_driveitems_isfolder ON DriveItems(IsFolder);
            CREATE INDEX IF NOT EXISTS idx_driveitems_name ON DriveItems(Name);
        ";
        _ = cmd.ExecuteNonQuery();
        cmd = conn.CreateCommand();
        cmd.CommandText = @"
    CREATE TABLE IF NOT EXISTS SyncLog (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        DriveId TEXT NOT NULL,
        Inserted INTEGER NOT NULL,
        Updated INTEGER NOT NULL,
        Deleted INTEGER NOT NULL,
        DeltaToken TEXT NOT NULL,
        LastSyncedUtc TEXT NOT NULL
    );
";
        _ = cmd.ExecuteNonQuery();
    }

    public async Task<List<SyncResult>> GetRecentSyncLogsAsync(int count = 10)
    {
        var results = new List<SyncResult>();

        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();

        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = @"
        SELECT DriveId, Inserted, Updated, Deleted, DeltaToken, LastSyncedUtc
        FROM SyncLog
        ORDER BY LastSyncedUtc DESC
        LIMIT $count;";
        _ = cmd.Parameters.AddWithValue("$count", count);

        using SqliteDataReader reader = await cmd.ExecuteReaderAsync();
        while(await reader.ReadAsync())
        {
            results.Add(new SyncResult
            {
                DeltaToken = reader.GetString(reader.GetOrdinal("DeltaToken")),
                LastSyncedUtc = DateTime.Parse(reader.GetString(reader.GetOrdinal("LastSyncedUtc"))),
                Inserted = reader.GetInt32(reader.GetOrdinal("Inserted")),
                Updated = reader.GetInt32(reader.GetOrdinal("Updated")),
                Deleted = reader.GetInt32(reader.GetOrdinal("Deleted"))
            });
        }

        _logger.LogInformation("Fetched {Count} recent sync logs", results.Count);

        return results;
    }
    public async Task<SyncAggregate> GetSyncAggregateAsync(TimeSpan period)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();

        DateTime since = DateTime.UtcNow.Subtract(period);

        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = @"
        SELECT 
            SUM(Inserted) as TotalInserted,
            SUM(Updated) as TotalUpdated,
            SUM(Deleted) as TotalDeleted,
            COUNT(*) as RunCount
        FROM SyncLog
        WHERE LastSyncedUtc >= $since;";
        _ = cmd.Parameters.AddWithValue("$since", since.ToString("o"));

        using SqliteDataReader reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync()
            ? new SyncAggregate
            {
                TotalInserted = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                TotalUpdated = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                TotalDeleted = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                RunCount = reader.IsDBNull(3) ? 0 : reader.GetInt32(3)
            }
            : new SyncAggregate();
    }
    public async Task SaveDeltaTokenAsync(string driveId, string deltaToken)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();

        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO DeltaState (DriveId, DeltaToken, LastSyncedUtc)
            VALUES ($driveId, $token, $ts)
            ON CONFLICT(DriveId) DO UPDATE SET
                DeltaToken = excluded.DeltaToken,
                LastSyncedUtc = excluded.LastSyncedUtc;";
        _ = cmd.Parameters.AddWithValue("$driveId", driveId);
        _ = cmd.Parameters.AddWithValue("$token", deltaToken);
        _ = cmd.Parameters.AddWithValue("$ts", DateTime.UtcNow.ToString("o"));
        _ = await cmd.ExecuteNonQueryAsync();
    }

    public async Task<string?> GetDeltaTokenAsync(string driveId, CancellationToken token)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync(token);

        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT DeltaToken FROM DeltaState WHERE DriveId = $driveId;";
        _ = cmd.Parameters.AddWithValue("$driveId", driveId);

        return (string?)await cmd.ExecuteScalarAsync(token);
    }

    public async Task SaveItemAsync(string id, string name, bool isFolder, DateTimeOffset? lastModified, string? parentPath = null, string? eTag = null, CancellationToken token = new())
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync(token);

        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO DriveItems (Id, Name, IsFolder, LastModifiedUtc, ParentPath, ETag)
            VALUES ($id, $name, $folder, $ts, $parentPath, $eTag)
            ON CONFLICT(Id) DO UPDATE SET
                Name = excluded.Name,
                IsFolder = excluded.IsFolder,
                LastModifiedUtc = excluded.LastModifiedUtc,
                ParentPath = excluded.ParentPath,
                ETag = excluded.ETag;";

        cmd.AddSmartParameter("$id", id);
        cmd.AddSmartParameter("$name", name);
        cmd.AddSmartParameter("$folder", isFolder); // bool → INTEGER
        cmd.AddSmartParameter("$ts", lastModified); // DateTimeOffset → ISO string
        cmd.AddSmartParameter("$parentPath", parentPath); // null → DBNull
        cmd.AddSmartParameter("$eTag", eTag);

        _ = await cmd.ExecuteNonQueryAsync(token);
    }

    public async Task<SyncResult> SaveBatchWithTokenAsync(
    IEnumerable<LocalDriveItem> items,
    IEnumerable<string> deletedIds,
    string driveId,
    string deltaToken,
    DateTime lastSyncedUtc, CancellationToken token)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync(token);

        using SqliteTransaction tx = conn.BeginTransaction();

        var result = new SyncResult
        {
            DeltaToken = deltaToken,
            LastSyncedUtc = lastSyncedUtc
        };

        try
        {
            // Upsert items
            foreach(LocalDriveItem item in items)
            {
                SqliteCommand cmd = conn.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = @"
                INSERT INTO DriveItems (Id, Name, IsFolder, LastModifiedUtc, ParentPath, ETag)
                VALUES ($id, $name, $folder, $ts, $parentPath, $eTag)
                ON CONFLICT(Id) DO UPDATE SET
                    Name = excluded.Name,
                    IsFolder = excluded.IsFolder,
                    LastModifiedUtc = excluded.LastModifiedUtc,
                    ParentPath = excluded.ParentPath,
                    ETag = excluded.ETag;";

                cmd.AddSmartParameter("$id", item.Id);
                cmd.AddSmartParameter("$name", item.Name);
                cmd.AddSmartParameter("$folder", item.IsFolder);
                cmd.AddSmartParameter("$ts", item.LastModifiedUtc);
                cmd.AddSmartParameter("$parentPath", item.ParentPath);
                cmd.AddSmartParameter("$eTag", item.ETag);

                _ = await cmd.ExecuteNonQueryAsync(token);
                result.Inserted++; // Simplified: count all upserts as "inserted"
            }

            // Delete tombstoned items
            foreach(var id in deletedIds)
            {
                SqliteCommand delCmd = conn.CreateCommand();
                delCmd.Transaction = tx;
                delCmd.CommandText = "DELETE FROM DriveItems WHERE Id = $id;";
                delCmd.AddSmartParameter("$id", id);
                var affected = await delCmd.ExecuteNonQueryAsync(token);
                if(affected > 0)
                    result.Deleted++;
            }

            // Update delta token atomically
            SqliteCommand tokenCmd = conn.CreateCommand();
            tokenCmd.Transaction = tx;
            tokenCmd.CommandText = @"
            INSERT INTO DeltaState (DriveId, DeltaToken, LastSyncedUtc)
            VALUES ($driveId, $token, $ts)
            ON CONFLICT(DriveId) DO UPDATE SET
                DeltaToken = excluded.DeltaToken,
                LastSyncedUtc = excluded.LastSyncedUtc;";
            tokenCmd.AddSmartParameter("$driveId", driveId);
            tokenCmd.AddSmartParameter("$token", deltaToken);
            tokenCmd.AddSmartParameter("$ts", lastSyncedUtc.ToString("o"));

            _ = await tokenCmd.ExecuteNonQueryAsync(token);

            // Persist metrics into SyncLog
            SqliteCommand logCmd = conn.CreateCommand();
            logCmd.Transaction = tx;
            logCmd.CommandText = @"
            INSERT INTO SyncLog (DriveId, Inserted, Updated, Deleted, DeltaToken, LastSyncedUtc)
            VALUES ($driveId, $inserted, $updated, $deleted, $token, $ts);";
            logCmd.AddSmartParameter("$driveId", driveId);
            logCmd.AddSmartParameter("$inserted", result.Inserted);
            logCmd.AddSmartParameter("$updated", result.Updated);
            logCmd.AddSmartParameter("$deleted", result.Deleted);
            logCmd.AddSmartParameter("$token", deltaToken);
            logCmd.AddSmartParameter("$ts", lastSyncedUtc.ToString("o"));

            _ = await logCmd.ExecuteNonQueryAsync(token);

            await tx.CommitAsync(token);

            // 🔑 Runtime logging
            _logger.LogInformation(
                "Sync completed for DriveId={DriveId}. Inserted={Inserted}, Updated={Updated}, Deleted={Deleted}, Token={Token}, LastSyncedUtc={LastSyncedUtc}",
                driveId, result.Inserted, result.Updated, result.Deleted, result.DeltaToken, result.LastSyncedUtc);

            return result;
        }
        catch(Exception ex)
        {
            await tx.RollbackAsync(token);
            _logger.LogError(ex, "Sync failed for DriveId={DriveId}", driveId);
            throw;
        }
    }
}

public partial class DeltaStore
{
    public async Task<LocalDriveItem?> GetItemByIdAsync(string id, CancellationToken token)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync(token);

        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM DriveItems WHERE Id = $id;";
        _ = cmd.Parameters.AddWithValue("$id", id);

        using SqliteDataReader reader = await cmd.ExecuteReaderAsync(token);
        return await reader.ReadAsync(token) ? reader.ToLocalDriveItem() : null;
    }

    public async Task<List<LocalDriveItem>> GetItemsByParentPathAsync(string parentPath, CancellationToken token)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync(token);

        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM DriveItems WHERE ParentPath = $parentPath;";
        _ = cmd.Parameters.AddWithValue("$parentPath", parentPath);

        var results = new List<LocalDriveItem>();
        using SqliteDataReader reader = await cmd.ExecuteReaderAsync(token);
        while(await reader.ReadAsync(token))
        {
            results.Add(reader.ToLocalDriveItem());
        }

        return results;
    }

    public async Task<List<LocalDriveItem>> GetAllFoldersAsync(CancellationToken token)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync(token);

        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM DriveItems WHERE IsFolder = 1;";

        var results = new List<LocalDriveItem>();
        using SqliteDataReader reader = await cmd.ExecuteReaderAsync(token);
        while(await reader.ReadAsync(token))
        {
            results.Add(reader.ToLocalDriveItem());
        }

        return results;
    }
}

public partial class DeltaStore
{
    /// <summary>
    /// Get items under a parent path modified since a given UTC timestamp.
    /// </summary>
    public async Task<List<LocalDriveItem>> GetItemsByParentPathModifiedSinceAsync(
        string parentPath, DateTime sinceUtc, CancellationToken token)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync(token);

        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT * FROM DriveItems
            WHERE ParentPath = $parentPath
              AND LastModifiedUtc > $sinceUtc;";
        _ = cmd.Parameters.AddWithValue("$parentPath", parentPath);
        _ = cmd.Parameters.AddWithValue("$sinceUtc", sinceUtc.ToString("o"));

        var results = new List<LocalDriveItem>();
        using SqliteDataReader reader = await cmd.ExecuteReaderAsync(token);
        while(await reader.ReadAsync(token))
        {
            results.Add(reader.ToLocalDriveItem());
        }

        return results;
    }

    /// <summary>
    /// Search items by name within a parent path.
    /// </summary>
    public async Task<List<LocalDriveItem>> SearchItemsByNameAsync(
        string parentPath, string nameLike, CancellationToken token)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync(token);

        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT * FROM DriveItems
            WHERE ParentPath = $parentPath
              AND Name LIKE $pattern;";
        _ = cmd.Parameters.AddWithValue("$parentPath", parentPath);
        _ = cmd.Parameters.AddWithValue("$pattern", $"%{nameLike}%");

        var results = new List<LocalDriveItem>();
        using SqliteDataReader reader = await cmd.ExecuteReaderAsync(token);
        while(await reader.ReadAsync(token))
        {
            results.Add(reader.ToLocalDriveItem());
        }

        return results;
    }

    /// <summary>
    /// Get all files (not folders) modified since a given UTC timestamp.
    /// </summary>
    public async Task<List<LocalDriveItem>> GetFilesModifiedSinceAsync(DateTime sinceUtc, CancellationToken token)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync(token);

        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT * FROM DriveItems
            WHERE IsFolder = 0
              AND LastModifiedUtc > $sinceUtc;";
        _ = cmd.Parameters.AddWithValue("$sinceUtc", sinceUtc.ToString("o"));

        var results = new List<LocalDriveItem>();
        using SqliteDataReader reader = await cmd.ExecuteReaderAsync(token);
        while(await reader.ReadAsync(token))
        {
            results.Add(reader.ToLocalDriveItem());
        }

        return results;
    }
}
