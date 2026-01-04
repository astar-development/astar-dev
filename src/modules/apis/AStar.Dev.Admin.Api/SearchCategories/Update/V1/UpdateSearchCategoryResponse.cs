using AStar.Dev.Infrastructure.AdminDb.Models;

namespace AStar.Dev.Admin.Api.SearchCategories.Update.V1;

/// <summary>
///     The <see cref="UpdateSearchCategoryResponse" /> class containing the details of the updated Site Configuration.
/// </summary>
public sealed class UpdateSearchCategoryResponse(SearchCategory config)
{
    /// <summary>
    ///     Gets or sets the Base Url for the login and search.
    /// </summary>
    public DateTime UpdatedOn { get; } = config.UpdatedOn;

    /// <summary>
    ///     Gets or sets the Category Name.
    /// </summary>
    public string Name { get; } = config.Name;

    /// <summary>
    ///     Gets or sets the Updated By.
    /// </summary>
    public string UpdatedBy { get; } = config.UpdatedBy;

    /// <summary>
    ///     Gets or sets the Order.
    /// </summary>
    public int Order { get; } = config.Order;

    /// <summary>
    ///     Gets or sets the Total Pages.
    /// </summary>
    public int TotalPages { get; } = config.TotalPages;

    /// <summary>
    ///     Gets or sets the Last Page Visited.
    /// </summary>
    public int LastPageVisited { get; } = config.LastPageVisited;

    /// <summary>
    ///     Gets or sets the Last Known Image Count.
    /// </summary>
    public int LastKnownImageCount { get; } = config.LastKnownImageCount;

    /// <summary>
    ///     Gets or sets the SearchCategoryId.
    /// </summary>
    public Guid SearchCategoryId { get; } = config.SearchCategoryId;
}
