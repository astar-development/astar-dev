namespace AStar.Dev.Files.Api.Files.Add.V1;

/// <summary>
///     The <see cref="AddFilesRequest" /> contains the detail of the files being added.
/// </summary>
public class AddFilesRequest
{
    /// <summary>
    /// </summary>
    public required IReadOnlyCollection<FileDetailToAdd> FilesToAdd { get; set; } = [];
}
