namespace AStar.Dev.Images.Api.Extensions;

/// <summary>
/// </summary>
public static class FileInfoExtensions
{
    /// <summary>
    /// </summary>
    public static bool IsImage(this FileInfo fileInfo)
        => fileInfo.Name.ToUpperInvariant().EndsWith(".JPEG")
           || fileInfo.Name.ToUpperInvariant().EndsWith(".JPG")
           || fileInfo.Name.ToUpperInvariant().EndsWith(".GIF")
           || fileInfo.Name.ToUpperInvariant().EndsWith(".BMP");
}
