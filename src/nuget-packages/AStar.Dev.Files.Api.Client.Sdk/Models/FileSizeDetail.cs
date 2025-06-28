namespace AStar.Dev.Files.Api.Client.SDK.Models;

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
    public string FileSizeForDisplay =>
        FileSize / 1024 / 1024 > 0
            ? (FileSize / 1024D / 1024D).ToString("N2") + " Mb"
            : (FileSize / 1024D).ToString("N2")         + " Kb";
}
