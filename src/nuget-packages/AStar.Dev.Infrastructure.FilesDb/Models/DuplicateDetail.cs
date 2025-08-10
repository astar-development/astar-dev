using System.ComponentModel.DataAnnotations;

namespace AStar.Dev.Infrastructure.FilesDb.Models;

/// <summary>
///     The <see cref="DuplicateDetail" /> class defines the fields that will be mapped from the vw_DuplicateDetails in the database
/// </summary>
public class DuplicateDetail
{
    /// <summary>
    ///     Gets or sets the File Name
    /// </summary>
    [MaxLength(256)]
    public required string FileName { get; set; }

    /// <summary>
    ///     Gets or sets the Directory Name
    /// </summary>
    [MaxLength(256)]
    public required string DirectoryName { get; set; }

    /// <summary>
    ///     Gets or sets the File Height
    /// </summary>
    public int    ImageHeight     { get; set; }

    /// <summary>
    ///     Gets or sets the File Width
    /// </summary>
    public int    ImageWidth      { get; set; }

    /// <summary>
    ///     Gets or sets the File Size
    /// </summary>
    public long    FileSize   { get; set; }

    /// <summary>
    ///     Gets or sets the File Handle
    /// </summary>
    [MaxLength(256)]
    public required string FileHandle { get; set; }

    /// <summary>
    ///     Gets or sets whether File is an image
    /// </summary>
    public bool   IsImage    { get; set; }

    /// <summary>
    ///     Gets or sets the Instance count for the duplicate group
    /// </summary>
    public int   Instances  { get; set; }

    /// <summary>
    ///     Gets or sets the Details Last Updated
    /// </summary>
    public DateTimeOffset UpdatedOn { get; set; }

    /// <summary>
    ///     Gets or sets the Last Viewed date
    /// </summary>
    public DateTimeOffset? FileLastViewed { get; set; }

    /// <summary>
    ///     Gets or sets the Soft Deleted flag
    /// </summary>
    public DateTimeOffset? SoftDeleted { get; set; }

    /// <summary>
    ///     Gets or sets the SoftDeletePending flag
    /// </summary>
    public DateTimeOffset? SoftDeletePending { get; set; }

    /// <summary>
    ///     Gets or sets the Move Required flag
    /// </summary>
    public bool MoveRequired { get; set; }

    /// <summary>
    ///     Gets or sets the Hard Delete Pending flag
    /// </summary>
    public DateTimeOffset? HardDeletePending { get; set; }
}
