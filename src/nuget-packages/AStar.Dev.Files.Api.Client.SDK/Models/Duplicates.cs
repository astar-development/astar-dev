namespace AStar.Dev.Files.Api.Client.SDK.Models;

/// <summary>
/// </summary>
/// <param name="FileSize"></param>
/// <param name="Duplicates"></param>
public record DuplicatesList(FileSizeDetail FileSize, Duplicates[] Duplicates);

/// <summary>
/// </summary>
/// <param name="FileSize"></param>
/// <param name="Height"></param>
/// <param name="Width"></param>
public record FileSizeDetail(int FileSize, int Height, int Width)
{
    /// <summary>
    ///     Gets the FileSize in a nicer, display-friendly, style
    /// </summary>
    public string FileSizeForDisplay
        => FileSize / 1024 / 1024 > 0
            ? (FileSize / 1024D / 1024D).ToString("N2") + " Mb"
            : (FileSize / 1024D).ToString("N2") + " Kb";
}

/// <summary>
/// </summary>
/// <param name="Id"></param>
/// <param name="FileAccessDetailId"></param>
/// <param name="FileName"></param>
/// <param name="DirectoryName"></param>
/// <param name="Height"></param>
/// <param name="Width"></param>
/// <param name="FileSize"></param>
/// <param name="FileHandle"></param>
/// <param name="IsImage"></param>
/// <param name="Instances"></param>
/// <param name="DetailsLastUpdated"></param>
/// <param name="LastViewed"></param>
/// <param name="SoftDeleted"></param>
/// <param name="SoftDeletePending"></param>
/// <param name="MoveRequired"></param>
/// <param name="HardDeletePending"></param>
public record Duplicates(
    int Id,
    int FileAccessDetailId,
    string FileName,
    string DirectoryName,
    int Height,
    int Width,
    int FileSize,
    string FileHandle,
    bool IsImage,
    int Instances,
    DateTimeOffset DetailsLastUpdated,
    DateTimeOffset? LastViewed,
    bool SoftDeleted,
    bool SoftDeletePending,
    bool MoveRequired,
    bool HardDeletePending
)
{
    /// <summary>
    /// </summary>
    public string FullNameWithPath => $"{DirectoryName}/{FileName}";
}
