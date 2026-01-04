using AStar.Dev.Infrastructure.AdminDb.Models;

namespace AStar.Dev.Admin.Api.SearchCategories.GetAll.V1;

/// <summary>
///     The <see cref="GetAllSearchCategoriesResponse" /> class used to return the Search Categories to the caller.
/// </summary>
public sealed class GetAllSearchCategoriesResponse(SearchCategory config)
{
    /// <summary>
    ///     Gets or sets The ID of the search category.
    /// </summary>
    public int Id { get; } = config.Id;

    /// <summary>
    ///     Gets or sets the SearchCategoryId - whilst the name sounds the same, this is the Search-category-specific ID and is
    ///     used as
    ///     part of the Historical modelling for each search category contained within the table.
    /// </summary>
    public Guid SearchCategoryId { get; } = config.SearchCategoryId;

    /// <summary>
    ///     Gets or sets the Order of the search category - i.e. which category should be 1st, 2nd, etc.
    /// </summary>
    public int Order { get; } = config.Order;

    /// <summary>
    ///     Gets or sets the Name of the category.
    /// </summary>
    public string Name { get; } = config.Name;

    /// <summary>
    ///     Gets or sets the Last Known Image Count.
    /// </summary>
    public int LastKnownImageCount { get; } = config.LastKnownImageCount;

    /// <summary>
    ///     Gets or sets the Last Page Visited number.
    /// </summary>
    public int LastPageVisited { get; } = config.LastPageVisited;

    /// <summary>
    ///     Gets or sets the Total Pages for the results.
    /// </summary>
    public int TotalPages { get; } = config.TotalPages;
}
