namespace AStar.Dev.Admin.Api.Client.Sdk.Models;

/// <summary>
///     The <see href="SearchCategory"></see> class containing the full Search Category definition
/// </summary>
public sealed class SearchCategory
{
    /// <summary>
    ///     Gets or sets The ID of the search category.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Order of the search category - i.e. which category should be 1st, 2nd, etc.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    ///     Gets or sets the Name of the category.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Last Known Image Count.
    /// </summary>
    public int LastKnownImageCount { get; set; }

    /// <summary>
    ///     Gets or sets the Last Page Visited number.
    /// </summary>
    public int LastPageVisited { get; set; }

    /// <summary>
    ///     Gets or sets the Total Pages for the results.
    /// </summary>
    public int TotalPages { get; set; }
}
