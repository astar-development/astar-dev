using System.Security.Claims;
using System.Text.Json.Serialization;
using AStar.Dev.Api.Usage.Sdk.Metrics;

namespace AStar.Dev.Admin.Api.ScrapeDirectories.GetAll.V1;

/// <summary>
///     The <see cref="GetAllScrapeDirectoriesQuery" /> class is used to define the All Scrape Directories Query.
/// </summary>
/// <param name="user">The <see cref="ClaimsPrincipal" /> containing information about the user making the call</param>
public sealed class GetAllScrapeDirectoriesQuery(ClaimsPrincipal user) : IEndpointName
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
