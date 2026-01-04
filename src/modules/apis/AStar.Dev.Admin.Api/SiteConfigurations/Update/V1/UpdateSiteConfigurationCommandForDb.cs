using System.Security.Claims;
using System.Text.Json.Serialization;
using AStar.Dev.Api.Usage.Sdk.Metrics;

namespace AStar.Dev.Admin.Api.SiteConfigurations.Update.V1;

/// <summary>
///     The <see cref="UpdateSiteConfigurationCommandForDb" /> class used to update the actual database table
///     <para>
///         The class implements the <see cref="IRequest{IResult}" /> as defined within MediatR
///     </para>
/// </summary>
/// <param name="slug">The SiteConfigurationSlug used to uniquely identify the Site Configuration</param>
/// <param name="user"></param>
/// <param name="updateSiteConfigurationCommand">
///     The <see cref="UpdateSiteConfigurationCommand" /> class containing the
///     command details for the Update
/// </param>
public sealed class UpdateSiteConfigurationCommandForDb(
    string                         slug,
    ClaimsPrincipal                user,
    UpdateSiteConfigurationCommand updateSiteConfigurationCommand) : IEndpointName
{
    /// <summary>
    ///     Gets the SiteConfigurationSlug used to uniquely identify the Site Configuration
    /// </summary>
    public string Slug { get; } = slug;

    /// <summary>
    ///     Gets or sets the Base Url for the login and search
    /// </summary>
    public string BaseUrl { get; } = updateSiteConfigurationCommand.BaseUrl;

    /// <summary>
    ///     Gets or sets the Login Url
    /// </summary>
    public string LoginUrl { get; } = updateSiteConfigurationCommand.LoginUrl;

    /// <summary>
    ///     Gets or sets the Login Email Address
    /// </summary>
    public string LoginEmailAddress { get; } = updateSiteConfigurationCommand.LoginEmailAddress;

    /// <summary>
    ///     Gets or sets the Username
    /// </summary>
    public string Username { get; } = updateSiteConfigurationCommand.Username;

    /// <summary>
    ///     Gets or sets the Password
    /// </summary>
    public string Password { get; } = updateSiteConfigurationCommand.Password;

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
