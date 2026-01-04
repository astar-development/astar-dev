using System.Security.Claims;
using System.Text.Json.Serialization;
using AStar.Dev.Api.Usage.Sdk.Metrics;

namespace AStar.Dev.Admin.Api.SearchCategories.GetById.V1;

/// <summary>
///     The <see cref="GetSearchCategoriesByIdQuery" /> class defines the parameter(s) used to perform the Search Category Query
/// </summary>
/// <param name="searchCategoryId">The Search Category Id</param>
/// <param name="user">The <see cref="ClaimsPrincipal" /> containing information about the user making the call</param>
public sealed class GetSearchCategoriesByIdQuery(Guid searchCategoryId, ClaimsPrincipal user) : IEndpointName
{
    /// <summary>
    ///     Gets the ScrapeDirectoryId used to uniquely identify the Search Category
    /// </summary>
    public Guid SearchCategoryId { get; } = searchCategoryId;

    /// <summary>
    ///     Gets the UpdatedBy (the user performing the action) value
    /// </summary>
    public string UpdatedBy => user.Identity?.Name ?? "me";

    /// <inheritdoc />
    [JsonIgnore]
    public string Name => EndpointConstants.SiteConfigurationsEndpoint;

    /// <inheritdoc />
    [JsonIgnore]
    public string HttpMethod => HttpMethods.Get;
}
