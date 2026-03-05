using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AStar.Dev.Utilities;

namespace AStar.Dev.Files.Api.Client.SDK.Models;

/// <summary>
///     The <see href="FileDetail"></see> class
/// </summary>
public sealed class FileDetail
{
    /// <summary>
    ///     Gets or sets The ID of the <see href="FileDetail"></see>. I know, shocking...
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// </summary>
    public FileAccessDetail FileAccessDetail { get; set; } = new();

    /// <summary>
    ///     Gets or sets the file name. I know, shocking...
    /// </summary>
    [MaxLength(300)]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the name of the directory containing the file detail. I know, shocking...
    /// </summary>
    [MaxLength(300)]
    public string DirectoryName { get; set; } = string.Empty;

    /// <summary>
    ///     Gets the full name of the file with the path combined
    /// </summary>
    [NotMapped]
    public string FullNameWithPath => $"{DirectoryName}\\{FileName}";

    /// <summary>
    ///     Gets or sets the height of the image. I know, shocking...
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    ///     Gets or sets the width of the image. I know, shocking...
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    ///     Gets or sets the file size. I know, shocking...
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    ///     Returns true when the file is of a supported image type
    /// </summary>
    public bool IsImage => FileName.IsImage();

    /// <summary>
    ///     Returns this object in JSON format
    /// </summary>
    /// <returns>
    ///     This object serialized as a JSON object.
    /// </returns>
    public override string ToString()
        => this.ToJson();
}
