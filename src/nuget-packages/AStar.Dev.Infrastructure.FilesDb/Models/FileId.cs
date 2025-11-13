// using AStar.Dev.Annotations;

namespace AStar.Dev.Infrastructure.FilesDb.Models;

/// <summary>
///     Defines the FileId
/// </summary>
public partial record struct FileId()
{
    /// <summary>The value of the File ID</summary>
    public Guid Value { get; set; } = Guid.CreateVersion7();
}

/// <summary>
/// 
/// </summary>
// [RegisterService]
public class Xxx{}
