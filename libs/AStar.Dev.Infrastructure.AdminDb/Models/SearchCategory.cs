using AStar.Dev.Infrastructure.Data;

namespace AStar.Dev.Infrastructure.AdminDb.Models;

/// <summary>
///     The <see href="SearchCategory"></see> class containing the Search Category Configuration and extending <see cref="AuditableEntity" />
/// </summary>
public sealed class SearchCategory : AuditableEntity
{
    /// <summary>
    ///     Gets or sets The ID of the search category
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     Gets or sets the SearchCategoryId - whilst the name sounds the same, this is the Search-category-specific ID and is
    ///     used as part of the Historical modelling for each search category contained within the table
    /// </summary>
    public Guid SearchCategoryId { get; set; } = Guid.CreateVersion7();

    /// <summary>
    ///     Gets or sets the Order of the search category - i.e. which category should be 1st, 2nd, etc.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    ///     Gets or sets the Name of the category
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Last Known Image Count
    /// </summary>
    public int LastKnownImageCount { get; set; }

    /// <summary>
    ///     Gets or sets the Last Page Visited number
    /// </summary>
    public int LastPageVisited { get; set; }

    /// <summary>
    ///     Gets or sets the Total Pages for the results
    /// </summary>
    public int TotalPages { get; set; }
}
