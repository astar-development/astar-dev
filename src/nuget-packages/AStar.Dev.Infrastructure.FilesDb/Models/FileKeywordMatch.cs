using System.ComponentModel.DataAnnotations;

namespace AStar.Dev.Infrastructure.FilesDb.Models;

/// <summary>
/// </summary>
public record FileKeywordMatch
{
    /// <summary>
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// </summary>
    [MaxLength(300)]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// </summary>
    [MaxLength(300)]
    public string Keyword { get; set; } = string.Empty;

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"{FileName} - {Keyword} ({Id})";
}
