namespace AStar.Dev.Infrastructure.FilesDb.Models;

/// <summary>
/// </summary>
public interface IFileDetail
{
    /// <summary>
    /// </summary>
    public string DirectoryName { get; set; }

    /// <summary>
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    ///     Gets or sets the File Size
    /// </summary>
    public long    FileSize   { get; set; }

    /// <summary>
    ///     Gets or sets whether File is an image
    /// </summary>
    public bool   IsImage    { get; set; }

    /// <summary>
    /// </summary>
    public DateTimeOffset? FileLastViewed { get; set; }
}
