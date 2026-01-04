using System.Security.Claims;
using System.Text.Json.Serialization;
using AStar.Dev.Api.Usage.Sdk.Metrics;

namespace AStar.Dev.Admin.Api.ScrapeDirectories.GetById.V1;

/// <summary>
///     The <see cref="GetScrapeDirectoriesByIdQuery" /> class defines the parameter(s) used to perform the Scrape Directory Query
/// </summary>
/// <param name="user">The <see cref="ClaimsPrincipal" /> containing information about the user making the call</param>
public sealed class GetScrapeDirectoriesByIdQuery(Guid scrapeDirectoryId, ClaimsPrincipal user) : IEndpointName
{
    /// <summary>
    ///     Gets the ScrapeDirectoryId used to uniquely identify the Scrape Directories
    /// </summary>
    public Guid ScrapeDirectoryId { get; } = scrapeDirectoryId;

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
