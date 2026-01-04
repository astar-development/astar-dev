using System.Security.Claims;
using System.Text.Json.Serialization;
using AStar.Dev.Api.Usage.Sdk.Metrics;

namespace AStar.Dev.Admin.Api.SearchCategories.GetAll.V1;

/// <summary>
///     The GetScrapeDirectoriesByIdQuery class is used to define the All Site Configurations Query.
/// </summary>
/// <param name="user">The <see cref="ClaimsPrincipal" /> containing information about the user making the call</param>
public sealed class GetAllSearchCategoriesQuery(ClaimsPrincipal user) : IEndpointName
{
    /// <summary>
    /// </summary>
    public ClaimsPrincipal User { get; } = user;

    /// <inheritdoc />
    [JsonIgnore]
    public string Name => EndpointConstants.SiteConfigurationsEndpoint;

    /// <inheritdoc />
    [JsonIgnore]
    public string HttpMethod => HttpMethods.Get;
}
