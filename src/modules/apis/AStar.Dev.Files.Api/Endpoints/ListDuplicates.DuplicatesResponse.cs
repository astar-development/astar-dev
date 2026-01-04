using AStar.Dev.Files.Api.Models;

namespace AStar.Dev.Files.Api.Endpoints;

/// <summary>
///     The <see href="DuplicateGroup"></see> class.
/// </summary>
public sealed class DuplicateGroup
{
    /// <summary>
    ///     Gets or sets The ID of the File Group.
    /// </summary>
    public FileSizeDto Group { get; set; } = new();

    /// <summary>
    ///     Gets or sets the list of <see href="FileInfoDto"></see>.
    /// </summary>
    public IReadOnlyCollection<FileInfoDto> Files { get; set; } = [];
}
