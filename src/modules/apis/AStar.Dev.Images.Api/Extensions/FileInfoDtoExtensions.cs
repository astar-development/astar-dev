using AStar.Dev.Images.Api.Models;

namespace AStar.Dev.Images.Api.Extensions;

/// <summary>
/// </summary>
public static class FileInfoDtoExtensions
{
    /// <summary>
    /// </summary>
    public static bool IsImage(this FileInfoDto fileInfo)
        => fileInfo.Extension.ToUpperInvariant() is ".JPEG"
                                                    or ".JPG"
                                                    or ".PNG"
                                                    or ".GIF"
                                                    or ".BMP";
}
