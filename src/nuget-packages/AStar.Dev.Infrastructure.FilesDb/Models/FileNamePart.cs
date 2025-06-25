namespace AStar.Dev.Infrastructure.FilesDb.Models;

/// <summary>
/// </summary>
public class FileNamePart
{
    /// <summary>
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// </summary>
    public virtual ICollection<FileClassification> FileClassifications { get ; set ; } = [];
}
