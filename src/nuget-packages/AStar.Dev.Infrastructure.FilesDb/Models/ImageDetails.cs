using AStar.Dev.Utilities;

namespace AStar.Dev.Infrastructure.FilesDb.Models;

/// <summary>
/// </summary>
public sealed class ImageDetails
{
    /// <summary>
    ///     Gets or sets The ID of the <see href="FileAccessDetail"></see>. I know, shocking...
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     Gets or sets The Width
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    ///     Gets or sets The Height
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    ///     Gets or sets The FileDetailsId
    /// </summary>
    public int FileDetailsId { get; set; }

    /// <summary>
    ///     Returns this object in JSON format
    /// </summary>
    /// <returns>
    ///     This object serialized as a JSON object
    /// </returns>
    public override string ToString() =>
        this.ToJson();
}
