using AStar.Dev.Infrastructure.AdminDb.Models;

namespace AStar.Dev.Admin.Api.SearchCategories.GetById.V1;

/// <summary>
///     The <see cref="GetSearchCategoriesByIdResponse" /> class used to return the Site Configuration to the caller.
/// </summary>
/// <param name="searchCategory">The instance of <see cref="SearchCategory" /> to map to the response</param>
public sealed class GetSearchCategoriesByIdResponse(SearchCategory searchCategory)
{
    /// <summary>
    ///     Gets or sets the Base Url for the login and search.
    /// </summary>
    public DateTime UpdatedOn { get; } = searchCategory.UpdatedOn;

    /// <summary>
    ///     Gets or sets the Category Name.
    /// </summary>
    public string Name { get; } = searchCategory.Name;

    /// <summary>
    ///     Gets or sets the Updated By.
    /// </summary>
    public string UpdatedBy { get; } = searchCategory.UpdatedBy;

    /// <summary>
    ///     Gets or sets the Order.
    /// </summary>
    public int Order { get; } = searchCategory.Order;

    /// <summary>
    ///     Gets or sets the Total Pages.
    /// </summary>
    public int TotalPages { get; } = searchCategory.TotalPages;

    /// <summary>
    ///     Gets or sets the Last Page Visited.
    /// </summary>
    public int LastPageVisited { get; } = searchCategory.LastPageVisited;

    /// <summary>
    ///     Gets or sets the Last Known Image Count.
    /// </summary>
    public int LastKnownImageCount { get; } = searchCategory.LastKnownImageCount;

    /// <summary>
    ///     Gets or sets the SearchCategoryId.
    /// </summary>
    public Guid SearchCategoryId { get; } = searchCategory.SearchCategoryId;
}
