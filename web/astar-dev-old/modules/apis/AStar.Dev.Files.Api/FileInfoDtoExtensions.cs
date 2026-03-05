using AStar.Dev.Files.Api.Models;

namespace AStar.Dev.Files.Api;

/// <summary>
/// </summary>
public static class FileInfoDtoExtensions
{
    /// <summary>
    /// </summary>
    public static bool IsImage(this FileInfoDto fileInfo)
        => fileInfo.Extension.ToUpperInvariant() is "JPEG"
                                                    or "JPG"
                                                    or "PNG"
                                                    or "GIF"
                                                    or "BMP";
}
