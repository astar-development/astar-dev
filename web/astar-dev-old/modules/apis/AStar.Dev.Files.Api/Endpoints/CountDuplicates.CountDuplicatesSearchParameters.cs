using System.ComponentModel.DataAnnotations;
using AStar.Dev.Files.Api.Config;
using AStar.Dev.Utilities;

namespace AStar.Dev.Files.Api.Endpoints;

/// <summary>
/// </summary>
public sealed class CountDuplicatesSearchParameters
{
    /// <summary>
    /// </summary>
    [Required]
    public string SearchFolder { get; set; } = string.Empty;

    /// <summary>
    /// </summary>
    public bool Recursive { get; set; } = true;

    /// <summary>
    /// </summary>
    public bool ExcludeViewed { get; set; }

    /// <summary>
    /// </summary>
    public bool IncludeSoftDeleted { get; set; }

    /// <summary>
    /// </summary>
    public bool IncludeMarkedForDeletion { get; set; }

    /// <summary>
    /// </summary>
    public string? SearchText { get; set; }

    /// <summary>
    /// </summary>
    internal SearchType SearchType { get; } = SearchType.Duplicates;

    /// <summary>
    /// </summary>
    public override string ToString()
        => this.ToJson();
}
