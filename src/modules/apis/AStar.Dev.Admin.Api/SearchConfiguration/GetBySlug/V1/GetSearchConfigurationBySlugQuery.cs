using System.Security.Claims;
using System.Text.Json.Serialization;
using AStar.Dev.Api.Usage.Sdk.Metrics;

namespace AStar.Dev.Admin.Api.SearchConfiguration.GetBySlug.V1;

/// <summary>
///     The <see cref="GetSearchConfigurationBySlugQuery" /> class defines the parameter(s) used to perform the Site Configuration Query.
/// </summary>
/// <param name="user">The <see cref="ClaimsPrincipal" /> containing information about the user making the call</param>
public sealed class GetSearchConfigurationBySlugQuery(string slug, ClaimsPrincipal user) : IEndpointName
{
    /// <summary>
    ///     Gets the SiteConfigurationSlug used to uniquely identify the Site Configuration.
    /// </summary>
    public string Slug { get; } = slug;

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
