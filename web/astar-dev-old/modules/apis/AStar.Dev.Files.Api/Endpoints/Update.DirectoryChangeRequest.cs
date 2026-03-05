using System.ComponentModel.DataAnnotations;

namespace AStar.Dev.Files.Api.Endpoints;

/// <summary>
/// </summary>
public sealed class DirectoryChangeRequest
{
    /// <summary>
    /// </summary>
    [Required]
    public string OldDirectoryName { get; set; } = string.Empty;

    /// <summary>
    /// </summary>
    [Required]
    public string NewDirectoryName { get; set; } = string.Empty;

    /// <summary>
    /// </summary>
    [Required]
    public string FileName { get; set; } = string.Empty;
}
