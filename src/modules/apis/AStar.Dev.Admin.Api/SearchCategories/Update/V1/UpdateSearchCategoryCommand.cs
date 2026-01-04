namespace AStar.Dev.Admin.Api.SearchCategories.Update.V1;

/// <summary>
///     The <see cref="UpdateSearchCategoryCommand" /> class contains the parameters required to Update the Search Category
/// </summary>
public sealed class UpdateSearchCategoryCommand
{
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
