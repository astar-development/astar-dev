namespace AStar.Dev.Files.Api.Client.SDK.Models;

/// <summary>
/// </summary>
public class FileDetailClassification
{
    /// <summary>
    /// </summary>
    public int FileId { get; set; }

    /// <summary>
    /// </summary>
    public FileDetail FileDetail { get; set; } = null!;

    /// <summary>
    /// </summary>
    public int ClassificationId { get; set; }

    /// <summary>
    /// </summary>
    public FileDetailClassifications FileDetailClassifications { get; set; } = null!;
}
