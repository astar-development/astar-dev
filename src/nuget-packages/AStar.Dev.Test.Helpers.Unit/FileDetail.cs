namespace AStar.Dev.Test.Helpers.Unit;

/// <summary>
///     The FileDetail class containing the current properties
/// </summary>
public sealed class FileDetail
{
    /// <summary>
    ///     Gets or sets the file name. I know, shocking...
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the name of the directory containing the file detail. I know, shocking...
    /// </summary>
    public string DirectoryName { get; set; } = string.Empty;

    /// <summary>
    ///     Gets the full name of the file with the path combined
    /// </summary>
    public string FullNameWithPath => Path.Combine(DirectoryName, FileName);

    /// <summary>
    ///     Gets or sets the file size. I know, shocking...
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    ///     Gets or sets whether the file is of a supported image type
    /// </summary>
    public bool IsImage { get; set; }

    /// <summary>
    ///     Gets or sets the file handle. I know, shocking...
    /// </summary>
    public string FileHandle { get; set; } = string.Empty;

    /// <summary>
    /// </summary>
    public DateTimeOffset FileCreated { get; set; }

    /// <summary>
    /// </summary>
    public DateTimeOffset FileLastModified { get; set; }

    /// <summary>
    /// </summary>
    public DateTimeOffset? FileLastViewed { get; set; }

    /// <summary>
    ///     Gets or sets whether the file has been 'soft deleted'. I know, shocking...
    /// </summary>
    public bool SoftDeleted { get; set; }

    /// <summary>
    ///     Gets or sets whether the file has been marked as 'delete pending'. I know, shocking...
    /// </summary>
    public bool SoftDeletePending { get; set; }

    /// <summary>
    ///     Gets or sets whether the file has been marked as 'needs to move'. I know, shocking...
    /// </summary>
    public bool MoveRequired { get; set; }

    /// <summary>
    ///     Gets or sets whether the file has been marked as 'delete permanently pending'. I know, shocking...
    /// </summary>
    public bool HardDeletePending { get; set; }

    /// <summary>
    ///     Gets or sets whether the file has been permanently deleted. I know, shocking...
    /// </summary>
    public bool HardDeleted { get; set; }

    /// <summary>
    /// </summary>
    public DateTimeOffset DetailsModified { get; set; }

    /// <summary>
    /// </summary>
    public string ModifiedBy { get; set; } = string.Empty;

    /// <summary>
    /// </summary>
    public int Id { get; set; }
}
