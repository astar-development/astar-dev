using System.Security.Claims;
using System.Text.Json.Serialization;
using AStar.Dev.Api.Usage.Sdk.Metrics;

namespace AStar.Dev.Admin.Api.SiteConfigurations.GetBySlug.V1;

/// <summary>
///     The <see cref="GetSiteConfigurationBySlugQuery" /> class defines the parameter(s) used to perform the Site Configuration Query
/// </summary>
public sealed class GetSiteConfigurationBySlugQuery(string slug, ClaimsPrincipal user) : IEndpointName
{
    /// <summary>
    ///     Gets the ScrapeDirectoryId used to uniquely identify the Site Configuration
    /// </summary>
    public string Slug { get; } = slug;

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
