namespace AStar.Dev.Files.Api.Endpoints.Get.V1;

/// <summary>
/// </summary>
public class GetFilesResponse
{
    /// <summary>
    /// </summary>
    public required string DirectoryName { get; set; }

    /// <summary>
    /// </summary>
    public required string FileName { get; set; }

    /// <summary>
    /// </summary>
    public required string FileHandle { get; set; }
}
