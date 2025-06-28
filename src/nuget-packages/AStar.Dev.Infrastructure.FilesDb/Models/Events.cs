namespace AStar.Dev.Infrastructure.FilesDb.Models;

/// <summary>
/// </summary>
public class Events
{
    /// <summary>
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// </summary>
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// </summary>
    public DateTimeOffset EventOccurredAt { get; set; }

    /// <summary>
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// </summary>
    public string DirectoryName { get; set; } = string.Empty;

    /// <summary>
    /// </summary>
    public string Handle { get; set; } = string.Empty;

    /// <summary>
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// </summary>
    public int? Height { get; set; }

    /// <summary>
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// </summary>
    public DateTimeOffset FileCreated { get; set; }

    /// <summary>
    /// </summary>
    public DateTimeOffset FileLastModified { get; set; }

    /// <summary>
    /// </summary>
    public string ModifiedBy { get; set; } = string.Empty;
}
