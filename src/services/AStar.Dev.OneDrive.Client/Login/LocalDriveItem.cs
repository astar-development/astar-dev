namespace AStar.Dev.OneDrive.Client.Login;

/// <summary>
///     Lightweight representation of a DriveItem for local persistence.
/// </summary>
public class LocalDriveItem
{
    public string Id { get; set; } = ""; // Graph GUID (stable, unique)
    public string PathId { get; set; } = ""; // Path-based ID for mirroring
    public string? Name { get; set; }
    public bool IsFolder { get; set; }
    public string? LastModifiedUtc { get; set; }
    public string? ParentPath { get; set; }
    public string? ETag { get; set; }
}
