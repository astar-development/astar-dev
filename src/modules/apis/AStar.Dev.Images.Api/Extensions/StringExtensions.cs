namespace AStar.Dev.Images.Api.Extensions;

/// <summary>
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// </summary>
    public static bool IsImage(this string imagePath)
        => imagePath.Extension().Equals("jpg",     StringComparison.OrdinalIgnoreCase)
           || imagePath.Extension().Equals("gif",  StringComparison.OrdinalIgnoreCase)
           || imagePath.Extension().Equals("jpeg", StringComparison.OrdinalIgnoreCase)
           || imagePath.Extension().Equals("png",  StringComparison.OrdinalIgnoreCase)
           || imagePath.Extension().Equals("bmp",  StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// </summary>
    public static string Extension(this string imagePath)
        => imagePath[(imagePath.LastIndexOf('.') + 1)..];

    /// <summary>
    /// </summary>
    public static string DirectoryName(this string imagePath)
        => imagePath[..imagePath.LastIndexOf('/')];

    /// <summary>
    /// </summary>
    public static string FileName(this string imagePath)
        => imagePath[(imagePath.LastIndexOf('/') + 1)..];
}
