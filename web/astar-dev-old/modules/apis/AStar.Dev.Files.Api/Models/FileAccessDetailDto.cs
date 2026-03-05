using AStar.Dev.Infrastructure.FilesDb.Models;
using AStar.Dev.Utilities;

namespace AStar.Dev.Files.Api.Models;

/// <summary>
/// </summary>
public sealed class FileAccessDetailDto
{
    /// <summary>
    /// </summary>
    public FileAccessDetailDto(FileAccessDetail fileAccessDetail)
    {
        Id                 = fileAccessDetail.Id;
        DetailsLastUpdated = fileAccessDetail.DetailsLastUpdated;
        LastViewed         = fileAccessDetail.LastViewed;
        NeedsToMove        = fileAccessDetail.MoveRequired;
    }

    /// <summary>
    /// </summary>
    public FileAccessDetailDto()
    {
    }

    /// <summary>
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     Gets or sets the date the file details were last updated. I know, shocking...
    /// </summary>
    public DateTime? DetailsLastUpdated { get; set; }

    /// <summary>
    ///     Gets or sets the date the file was last viewed. I know, shocking...
    /// </summary>
    public DateTime? LastViewed { get; set; }

    /// <summary>
    ///     Gets or sets whether the file has been 'soft deleted'. I know, shocking...
    /// </summary>
    public bool SoftDeleted { get; set; }

    /// <summary>
    ///     Gets or sets whether the file has been marked as 'delete pending'. I know, shocking...
    /// </summary>
    public bool SoftDeletePending { get; set; }

    /// <summary>
    ///     Gets or sets whether the NeedsToMove flag is set for the file
    /// </summary>
    public bool NeedsToMove { get; set; }

    /// <summary>
    ///     Gets or sets whether the HardDeletePending flag is set for the file
    /// </summary>
    public bool HardDeletePending { get; set; }

    /// <summary>
    ///     Returns this object in JSON format
    /// </summary>
    /// <returns>This object serialized as a JSON object</returns>
    public override string ToString()
        => this.ToJson();
}
