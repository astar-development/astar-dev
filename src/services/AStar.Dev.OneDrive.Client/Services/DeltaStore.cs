using System;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace AStar.Dev.OneDrive.Client.Services;

public class DeltaStore
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
            LastModifiedUtc TEXT
        );";
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

    public async Task SaveItemAsync(string id, string name, bool isFolder, DateTimeOffset? lastModified)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();

        SqliteCommand cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO DriveItems (Id, Name, IsFolder, LastModifiedUtc)
            VALUES ($id, $name, $folder, $ts)
            ON CONFLICT(Id) DO UPDATE SET
                Name = excluded.Name,
                IsFolder = excluded.IsFolder,
                LastModifiedUtc = excluded.LastModifiedUtc;";
        _ = cmd.Parameters.AddWithValue("$id", id);
        _ = cmd.Parameters.AddWithValue("$name", name);
        _ = cmd.Parameters.AddWithValue("$folder", isFolder ? 1 : 0);
        _ = cmd.Parameters.AddWithValue("$ts", lastModified?.UtcDateTime.ToString("o") ?? "");
        _ = await cmd.ExecuteNonQueryAsync();
    }
}
