namespace AStar.Dev.Admin.Api.SearchConfiguration.GetBySlug.V1;

/// <summary>
///     The <see cref="GetSearchConfigurationBySlugResponse" /> class used to return the Site Configuration to the caller.
/// </summary>
public sealed class GetSearchConfigurationBySlugResponse(Infrastructure.AdminDb.Models.SearchConfiguration config)
{
    /// <summary>
    ///     Gets or sets the SiteConfigurationSlug for the Site Configuration.
    /// </summary>
    public string SiteConfigurationSlug { get; } = config.SiteConfigurationSlug;

    /// <summary>
    ///     Gets or sets the Base Url for the login and search.
    /// </summary>
    public string SearchString { get; } = config.SearchString;

    /// <summary>
    ///     Gets or sets the Login Url.
    /// </summary>
    public string Subscriptions { get; } = config.Subscriptions;

    /// <summary>
    ///     Gets or sets the SearchStringPrefix.
    /// </summary>
    public string SearchStringPrefix { get; } = config.SearchStringPrefix;

    /// <summary>
    ///     Gets or sets the TopWallpapers.
    /// </summary>
    public string TopWallpapers { get; } = config.TopWallpapers;

    /// <summary>
    ///     Gets or sets the TotalPages.
    /// </summary>
    public int TotalPages { get; } = config.TotalPages;
}
