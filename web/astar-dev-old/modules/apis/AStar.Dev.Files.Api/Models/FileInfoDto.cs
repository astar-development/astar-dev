using AStar.Dev.Infrastructure.FilesDb.Models;
using AStar.Dev.Utilities;

namespace AStar.Dev.Files.Api.Models;

/// <summary>
/// </summary>
public sealed class FileInfoDto
{
    /// <summary>
    /// </summary>
    public FileInfoDto(FileDetail fileDetail)
    {
        Name     = fileDetail.FileName.Value;
        FullName = Path.Combine(fileDetail.DirectoryName.Value, fileDetail.FileName.Value);
        Height   = fileDetail.ImageDetail?.Height ?? 0;
        Width    = fileDetail.ImageDetail?.Width  ?? 0;
        Size     = fileDetail.FileSize;
        Id       = fileDetail.Id.Value;
    }

    /// <summary>
    /// </summary>
    public FileInfoDto()
    {
    }

    /// <summary>
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// </summary>
    public long Height { get; set; }

    /// <summary>
    /// </summary>
    public long Width { get; set; }

    /// <summary>
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    ///     Gets or sets the date the file details were last updated. I know, shocking...
    /// </summary>
    public DateTime? DetailsLastUpdated { get; set; }

    /// <summary>
    ///     Gets or sets the date the file was last viewed. I know, shocking...
    /// </summary>
    public DateTime? LastViewed { get; set; }

    /// <summary>
    /// </summary>
    public string Extension
    {
        get
        {
            var extensionIndex = Name.LastIndexOf('.') + 1;

            return Name[extensionIndex..];
        }
    }

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
