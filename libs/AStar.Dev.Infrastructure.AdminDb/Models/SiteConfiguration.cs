using AStar.Dev.Infrastructure.Data;

namespace AStar.Dev.Infrastructure.AdminDb.Models;

/// <summary>
///     The <see cref="SiteConfiguration" /> class, extending <see cref="AuditableEntity" />
/// </summary>
public sealed class SiteConfiguration : AuditableEntity
{
    /// <summary>
    ///     Gets or sets The ID of the Site Configuration
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    ///     Gets or sets the SiteConfigurationSlug - this is the site-specific SiteConfigurationSlug (text representation of the site) and is
    ///     used as part of the Historical modelling for each Site Configuration contained within the table
    /// </summary>
    public string SiteConfigurationSlug { get; init; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Base Url for the login and search
    /// </summary>
    public string BaseUrl { get; init; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Login Url - relative to the <see cref="BaseUrl" />
    /// </summary>
    public string LoginUrl { get; init; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Login Email Address
    /// </summary>
    public string LoginEmailAddress { get; init; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Username
    /// </summary>
    public string Username { get; init; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Password
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
