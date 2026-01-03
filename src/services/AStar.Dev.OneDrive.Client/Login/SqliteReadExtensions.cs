using Microsoft.Data.Sqlite;

namespace AStar.Dev.OneDrive.Client.Login;

public static class SqliteReadExtensions
{
    /// <summary>
    ///     Safely reads a column value, handling NULLs.
    /// </summary>
    public static T? GetValueOrDefault<T>(this SqliteDataReader reader, string columnName)
    {
        var ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? default : (T)reader.GetValue(ordinal);
    }

    /// <summary>
    ///     Hydrates a lightweight DriveItem metadata object from a row.
    /// </summary>
    public static LocalDriveItem ToLocalDriveItem(this SqliteDataReader reader) => new()
    {
        Id = reader.GetValueOrDefault<string>("Id")!,
        Name = reader.GetValueOrDefault<string>("Name"),
        IsFolder = reader.GetValueOrDefault<long>("IsFolder") == 1,
        LastModifiedUtc = reader.GetValueOrDefault<string>("LastModifiedUtc"),
        ParentPath = reader.GetValueOrDefault<string>("ParentPath"),
        ETag = reader.GetValueOrDefault<string>("ETag")
    };
}
