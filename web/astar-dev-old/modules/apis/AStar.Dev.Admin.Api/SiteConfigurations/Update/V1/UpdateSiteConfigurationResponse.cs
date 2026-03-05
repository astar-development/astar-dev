namespace AStar.Dev.Admin.Api.SiteConfigurations.Update.V1;

/// <summary>
///     The <see cref="UpdateSiteConfigurationResponse" /> class containing the details of the updated Site Configuration
/// </summary>
public sealed class UpdateSiteConfigurationResponse
{
    /// <summary>
    ///     Gets or sets the SiteConfigurationSlug used to update the Site Configuration
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Base Url for the login and search
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Login Url
    /// </summary>
    public string LoginUrl { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Login Email Address
    /// </summary>
    public string LoginEmailAddress { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Username
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Password
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
