using System.ComponentModel.DataAnnotations;
using AStar.Dev.Utilities;

namespace AStar.Dev.Files.Api.Client.SDK.Models;

/// <summary>
///     The <see href="DirectoryChangeRequest"></see> class
/// </summary>
public sealed class DirectoryChangeRequest
{
    /// <summary>
    ///     Gets or sets the Old Directory name
    /// </summary>
    [Required]
    public string OldDirectoryName { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the New Directory name
    /// </summary>
    [Required]
    public string NewDirectoryName { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Filename
    /// </summary>
    [Required]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    ///     Returns this object in JSON format
    /// </summary>
    /// <returns>This object serialized as a JSON object</returns>
    public override string ToString() =>
        this.ToJson();
}
