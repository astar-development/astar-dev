using AStar.Dev.Infrastructure.AdminDb.Models;

namespace AStar.Dev.Admin.Api.SiteConfigurations.GetBySlug.V1;

/// <summary>
///     The <see cref="GetSiteConfigurationBySlugResponse" /> class used to return the Site Configuration to the caller
/// </summary>
public sealed class GetSiteConfigurationBySlugResponse(SiteConfiguration config)
{
    /// <summary>
    ///     Gets or sets the Base Url for the login and search
    /// </summary>
    public string BaseUrl { get; } = config.BaseUrl;

    /// <summary>
    ///     Gets or sets the Login Url
    /// </summary>
    public string LoginUrl { get; } = config.LoginUrl;

    /// <summary>
    ///     Gets or sets the Login Email Address
    /// </summary>
    public string LoginEmailAddress { get; } = config.LoginEmailAddress;

    /// <summary>
    ///     Gets or sets the Username
    /// </summary>
    public string Username { get; } = config.Username;

    /// <summary>
    ///     Gets or sets the Password
    /// </summary>
    public string Password { get; } = config.Password;
}
