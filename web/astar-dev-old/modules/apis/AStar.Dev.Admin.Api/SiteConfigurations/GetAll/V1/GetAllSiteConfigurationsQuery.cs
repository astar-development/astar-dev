using System.Security.Claims;
using System.Text.Json.Serialization;
using AStar.Dev.Api.Usage.Sdk.Metrics;

namespace AStar.Dev.Admin.Api.SiteConfigurations.GetAll.V1;

/// <summary>
///     The GetAllScrapeDirectoriesQuery class is used to define the All Site Configurations Query
/// </summary>
public sealed class GetAllSiteConfigurationsQuery(ClaimsPrincipal user) : IEndpointName
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
