namespace AStar.Dev.Infrastructure.FilesDb.Models;

/// <summary>
/// </summary>
public class FileClassification
{
    /// <summary>
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// </summary>
    public ICollection<FileDetail> FileDetails { get ; set ; } = [];

    /// <summary>
    /// </summary>
    public bool Celebrity { get; set; }

    /// <summary>
    /// </summary>
    public ICollection<FileNamePart> FileNameParts { get ; set ; } = [];
}
