using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;

namespace AStar.Dev.OneDrive.Client.Services;

public class DeltaStore
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
                PathId TEXT,
                Name TEXT,
                IsFolder INTEGER,
                LastModifiedUtc TEXT,
                ParentPath TEXT NULL,
                ETag TEXT NULL,
                DownloadedDate TEXT NULL
            );

            -- Indexes for fast lookups
            CREATE INDEX IF NOT EXISTS idx_driveitems_parentpath ON DriveItems(ParentPath);
            CREATE INDEX IF NOT EXISTS idx_driveitems_lastmodified ON DriveItems(LastModifiedUtc);
            CREATE INDEX IF NOT EXISTS idx_driveitems_isfolder ON DriveItems(IsFolder);
            CREATE INDEX IF NOT EXISTS idx_driveitems_name ON DriveItems(Name);
            CREATE INDEX IF NOT EXISTS idx_driveitems_PathId ON DriveItems(PathId);
            CREATE INDEX IF NOT EXISTS idx_driveitems_downloadeddate ON DriveItems(DownloadedDate);
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
                INSERT INTO DriveItems (Id, PathId, Name, IsFolder, LastModifiedUtc, ParentPath, ETag)
                VALUES ($id, $pathId, $name, $folder, $ts, $parentPath, $eTag)
                ON CONFLICT(Id) DO UPDATE SET
                    Name = excluded.Name,
                    PathId = excluded.PathId,
                    IsFolder = excluded.IsFolder,
                    LastModifiedUtc = excluded.LastModifiedUtc,
                    ParentPath = excluded.ParentPath,
                    ETag = excluded.ETag;";

                cmd.AddSmartParameter("$id", item.Id);
                cmd.AddSmartParameter("$pathId", item.PathId);
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
        catch (OperationCanceledException)
        {
            await tx.RollbackAsync(token);
            _logger.LogWarning("Sync canceled for DriveId={DriveId}", driveId);
        }
        catch(Exception ex)
        {
            await tx.RollbackAsync(token);
            _logger.LogError(ex, "Sync failed for DriveId={DriveId}", driveId);
            throw;
        }
    }

    public async Task MarkItemsAsDownloadedAsync(IEnumerable<string> ids, CancellationToken token)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync(token);

        using SqliteTransaction tx = conn.BeginTransaction();

        foreach(var id in ids)
        {
            using SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"
            UPDATE DriveItems
            SET DownloadedDate = $downloadedDate
            WHERE Id = $id;";
            _ = cmd.Parameters.AddWithValue("$downloadedDate", DateTime.UtcNow.ToString("o"));
            _ = cmd.Parameters.AddWithValue("$id", id);

            _ = await cmd.ExecuteNonQueryAsync(token);
        }

        tx.Commit();
    }

    public async Task<LocalDriveItem?> GetRootAsync(string driveId, CancellationToken token)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync(token);

        using SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = @"
        SELECT Id, Name, IsFolder, ParentPath
        FROM DriveItems
        WHERE Id = $rootId;";
        _ = cmd.Parameters.AddWithValue("$rootId", $"/drives/{driveId}/root:");

        using SqliteDataReader reader = await cmd.ExecuteReaderAsync(token);
        return await reader.ReadAsync(token)
        ? new LocalDriveItem
        {
            Id = reader.GetString(0),
            Name = reader.GetString(1),
            IsFolder = reader.GetBoolean(2),
            ParentPath = reader.IsDBNull(3) ? null : reader.GetString(3)
        }
        : null;
    }

    public async Task InsertRootAsync(string driveId, DriveItem root, CancellationToken token)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync(token);

        using SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = @"
        INSERT INTO DriveItems (Id, PathId, Name, IsFolder, LastModifiedUtc, ParentPath, ETag)
        VALUES ($id, $pathId, $name, $folder, $ts, $parentPath, $eTag)
        ON CONFLICT(Id) DO UPDATE SET
            Name = excluded.Name,
            PathId = excluded.PathId,
            IsFolder = excluded.IsFolder,
            LastModifiedUtc = excluded.LastModifiedUtc,
            ParentPath = excluded.ParentPath,
            ETag = excluded.ETag;";

        // Id: path-based id for root; PathId: GUID
        cmd.AddSmartParameter("$id", $"/drives/{driveId}/root:");
        cmd.AddSmartParameter("$pathId", root.Id);
        cmd.AddSmartParameter("$name", root.Name ?? "root");
        cmd.AddSmartParameter("$folder", root.Folder != null); // bool, not root.Folder.IsFolder
        cmd.AddSmartParameter("$ts", root.LastModifiedDateTime?.UtcDateTime.ToString("o") ?? (object)DBNull.Value);
        cmd.AddSmartParameter("$parentPath", DBNull.Value); // root has no parent
        cmd.AddSmartParameter("$eTag", root.ETag ?? (object)DBNull.Value);

        _ = await cmd.ExecuteNonQueryAsync(token);
    }

    public async Task InsertChildrenAsync(string parentPath, IEnumerable<DriveItem> children, CancellationToken token)
    {
        await using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync(token);

        using SqliteTransaction tx = conn.BeginTransaction();

        foreach(DriveItem child in children)
        {
            await using SqliteCommand cmd = conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = @"
            INSERT INTO DriveItems (Id, PathId, Name, IsFolder, LastModifiedUtc, ParentPath, ETag)
            VALUES ($id, $pathId, $name, $folder, $ts, $parentPath, $eTag)
            ON CONFLICT(Id) DO UPDATE SET
                Name = excluded.Name,
                PathId = excluded.PathId,
                IsFolder = excluded.IsFolder,
                LastModifiedUtc = excluded.LastModifiedUtc,
                ParentPath = excluded.ParentPath,
                ETag = excluded.ETag;";

            var effectiveParentPath = child.ParentReference?.Path ?? parentPath;
            var pathIdForRow = $"{effectiveParentPath}/{child.Name}".Replace("//", "/");

            cmd.AddSmartParameter("$id", pathIdForRow);
            cmd.AddSmartParameter("$pathId", child.Id);
            cmd.AddSmartParameter("$name", child.Name ?? string.Empty);
            cmd.AddSmartParameter("$folder", child.Folder != null ? 1 : 0); // store as int
            cmd.AddSmartParameter("$ts", child.LastModifiedDateTime?.UtcDateTime.ToString("o") ?? (object)DBNull.Value);
            cmd.AddSmartParameter("$parentPath", effectiveParentPath);
            cmd.AddSmartParameter("$eTag", child.ETag ?? (object)DBNull.Value);

            _ = await cmd.ExecuteNonQueryAsync(token);
        }

        await tx.CommitAsync(token);
    }

    public async Task<int> CountTotalFilesAsync(CancellationToken token)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync(token);

        using SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = @"
        SELECT COUNT(*)
        FROM DriveItems
        WHERE IsFolder = 0;";

        var result = await cmd.ExecuteScalarAsync(token);
        return Convert.ToInt32(result);
    }

    public async Task<IReadOnlyList<LocalDriveItem>> GetChildrenAsync(string parentPath, CancellationToken token)
    {
        var results = new List<LocalDriveItem>();

        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync(token);

        using SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = @"
        SELECT Id, Name, IsFolder, ParentPath
        FROM DriveItems
        WHERE ParentPath = $parentPath;";
        _ = cmd.Parameters.AddWithValue("$parentPath", parentPath);

        using SqliteDataReader reader = await cmd.ExecuteReaderAsync(token);
        while(await reader.ReadAsync(token))
        {
            results.Add(new LocalDriveItem
            {
                Id = reader.GetString(0),
                Name = reader.GetString(1),
                IsFolder = reader.GetBoolean(2),
                ParentPath = reader.IsDBNull(3) ? null : reader.GetString(3)
            });
        }

        return results;
    }
}
