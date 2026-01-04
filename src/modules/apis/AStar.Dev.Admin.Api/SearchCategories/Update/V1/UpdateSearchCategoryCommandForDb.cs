using System.Security.Claims;
using System.Text.Json.Serialization;
using AStar.Dev.Api.Usage.Sdk.Metrics;

namespace AStar.Dev.Admin.Api.SearchCategories.Update.V1;

/// <summary>
///     The <see cref="UpdateSearchCategoryCommandForDb" /> class used to update the actual database table.
/// </summary>
/// <param name="searchCategoryId">The searchCategoryId used to uniquely identify the Search Category</param>
/// <param name="user">The <see cref="ClaimsPrincipal" /> containing information about the user making the call</param>
/// <param name="updateSearchCategoryCommand">
///     The <see cref="UpdateSearchCategoryCommand" /> class containing the
///     command details for the Update.
/// </param>
public sealed class UpdateSearchCategoryCommandForDb(
    Guid                        searchCategoryId,
    ClaimsPrincipal             user,
    UpdateSearchCategoryCommand updateSearchCategoryCommand) : IEndpointName
{
    /// <summary>
    ///     Gets or sets the SearchCategoryId - whilst the name sounds the same, this is the Search-category-specific ID and is
    ///     used as
    ///     part of the Historical modelling for each search category contained within the table.
    /// </summary>
    public Guid SearchCategoryId { get; set; } = searchCategoryId;

    /// <summary>
    ///     Gets or sets the Order of the search category - i.e. which category should be 1st, 2nd, etc.
    /// </summary>
    public int Order { get; set; } = updateSearchCategoryCommand.Order;

    /// <summary>
    ///     Gets or sets the Name of the category.
    /// </summary>
    public string CategoryName { get; set; } = updateSearchCategoryCommand.Name;

    /// <summary>
    ///     Gets or sets the Last Known Image Count.
    /// </summary>
    public int LastKnownImageCount { get; set; } = updateSearchCategoryCommand.LastKnownImageCount;

    /// <summary>
    ///     Gets or sets the Last Page Visited number.
    /// </summary>
    public int LastPageVisited { get; set; } = updateSearchCategoryCommand.LastPageVisited;

    /// <summary>
    ///     Gets or sets the Total Pages for the results.
    /// </summary>
    public int TotalPages { get; set; } = updateSearchCategoryCommand.TotalPages;

    /// <summary>
    ///     Gets the UpdatedBy (the user performing the action) value
    /// </summary>
    public string UpdatedBy => user.Identity?.Name ?? "me";

    /// <inheritdoc />
    [JsonIgnore]
    public string Name => EndpointConstants.SiteConfigurationsEndpoint;

    /// <inheritdoc />
    [JsonIgnore]
    public string HttpMethod => HttpMethods.Put;
}
