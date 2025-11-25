using System;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace AStar.Dev.OneDrive.Client.Services;

public partial class DeltaStore
{
    private readonly string _connectionString;

    public DeltaStore(string dbPath = "onedrive_sync.db")
    {
        _connectionString = $"Data Source={dbPath}";
        Initialize();
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

    public async Task<string?> GetDeltaTokenAsync(string driveId)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();

        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT DeltaToken FROM DeltaState WHERE DriveId = $driveId;";
        _ = cmd.Parameters.AddWithValue("$driveId", driveId);

        return (string?)await cmd.ExecuteScalarAsync();
    }

    public async Task SaveItemAsync(string id, string name, bool isFolder, DateTimeOffset? lastModified, string? parentPath = null, string? eTag = null)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();

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

        _ = await cmd.ExecuteNonQueryAsync();

    }
}

public partial class DeltaStore
{
    public async Task<LocalDriveItem?> GetItemByIdAsync(string id)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();

        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM DriveItems WHERE Id = $id;";
        _ = cmd.Parameters.AddWithValue("$id", id);

        using SqliteDataReader reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? reader.ToLocalDriveItem() : null;
    }

    public async Task<List<LocalDriveItem>> GetItemsByParentPathAsync(string parentPath)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();

        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM DriveItems WHERE ParentPath = $parentPath;";
        _ = cmd.Parameters.AddWithValue("$parentPath", parentPath);

        var results = new List<LocalDriveItem>();
        using SqliteDataReader reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(reader.ToLocalDriveItem());
        }

        return results;
    }

    public async Task<List<LocalDriveItem>> GetAllFoldersAsync()
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();

        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM DriveItems WHERE IsFolder = 1;";

        var results = new List<LocalDriveItem>();
        using SqliteDataReader reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
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
        string parentPath, DateTime sinceUtc)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();

        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT * FROM DriveItems
            WHERE ParentPath = $parentPath
              AND LastModifiedUtc > $sinceUtc;";
        _ = cmd.Parameters.AddWithValue("$parentPath", parentPath);
        _ = cmd.Parameters.AddWithValue("$sinceUtc", sinceUtc.ToString("o"));

        var results = new List<LocalDriveItem>();
        using SqliteDataReader reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(reader.ToLocalDriveItem());
        }

        return results;
    }

    /// <summary>
    /// Search items by name within a parent path.
    /// </summary>
    public async Task<List<LocalDriveItem>> SearchItemsByNameAsync(
        string parentPath, string nameLike)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();

        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT * FROM DriveItems
            WHERE ParentPath = $parentPath
              AND Name LIKE $pattern;";
        _ = cmd.Parameters.AddWithValue("$parentPath", parentPath);
        _ = cmd.Parameters.AddWithValue("$pattern", $"%{nameLike}%");

        var results = new List<LocalDriveItem>();
        using SqliteDataReader reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(reader.ToLocalDriveItem());
        }

        return results;
    }

    /// <summary>
    /// Get all files (not folders) modified since a given UTC timestamp.
    /// </summary>
    public async Task<List<LocalDriveItem>> GetFilesModifiedSinceAsync(DateTime sinceUtc)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();

        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT * FROM DriveItems
            WHERE IsFolder = 0
              AND LastModifiedUtc > $sinceUtc;";
        _ = cmd.Parameters.AddWithValue("$sinceUtc", sinceUtc.ToString("o"));

        var results = new List<LocalDriveItem>();
        using SqliteDataReader reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(reader.ToLocalDriveItem());
        }

        return results;
    }
}
