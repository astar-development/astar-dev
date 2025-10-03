namespace AStar.Dev.Infrastructure.FilesDb.Models;

/// <summary>
///     Defines the FileId
/// </summary>
public record struct FileId()
{
    /// <summary>The value of the File ID</summary>
    public Guid Value { get; set; } = Guid.CreateVersion7();
}
