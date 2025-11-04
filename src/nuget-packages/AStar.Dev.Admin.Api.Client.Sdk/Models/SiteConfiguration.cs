namespace AStar.Dev.Admin.Api.Client.Sdk.Models;

/// <summary>
///     The <see href="SiteConfiguration"></see> class containing Site Configuration.
/// </summary>
public sealed class SiteConfiguration
{
    /// <summary>
    ///     Gets or sets The ID of the configuration
    /// </summary>
    public int Id { get; set; }

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

    /// <summary>
    ///     Gets or sets the Site Configuration Slug
    /// </summary>
    public string SiteConfigurationSlug { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Base URL
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Login URL
    /// </summary>
    public string LoginUrl { get; set; } = string.Empty;
}
