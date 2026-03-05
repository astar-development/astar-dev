namespace AStar.Dev.Admin.Api.SearchConfiguration.Update.V1;

/// <summary>
///     The <see cref="UpdateSearchConfigurationResponse" /> class containing the details of the updated Site Configuration.
/// </summary>
public sealed class UpdateSearchConfigurationResponse(Infrastructure.AdminDb.Models.SearchConfiguration searchConfiguration)
{
    /// <summary>
    ///     Gets the SiteConfigurationSlug for the Site Configuration.
    /// </summary>
    public string SiteConfigurationSlug { get; } = searchConfiguration.SiteConfigurationSlug;

    /// <summary>
    ///     Gets the Base Url for the login and search.
    /// </summary>
    public string SearchString { get; } = searchConfiguration.SearchString;

    /// <summary>
    ///     Gets the Login Url.
    /// </summary>
    public string Subscriptions { get; } = searchConfiguration.Subscriptions;

    /// <summary>
    ///     Gets the SearchStringPrefix.
    /// </summary>
    public string SearchStringPrefix { get; } = searchConfiguration.SearchStringPrefix;

    /// <summary>
    ///     Gets the TopWallpapers.
    /// </summary>
    public string TopWallpapers { get; } = searchConfiguration.TopWallpapers;

    /// <summary>
    ///     Gets the TotalPages.
    /// </summary>
    public int TotalPages { get; } = searchConfiguration.TotalPages;

    /// <summary>
    ///     Gets the Image Pause in Seconds value
    /// </summary>
    public int ImagePauseInSeconds { get; } = searchConfiguration.ImagePauseInSeconds;

    /// <summary>
    /// </summary>
    public string SearchStringSuffix { get; } = searchConfiguration.SearchStringSuffix;

    /// <summary>
    ///     Gets the StartingPageNumber value
    /// </summary>
    public int StartingPageNumber { get; } = searchConfiguration.StartingPageNumber;

    /// <summary>
    ///     Gets the UseHeadless value
    /// </summary>
    public bool UseHeadless { get; } = searchConfiguration.UseHeadless;

    /// <summary>
    ///     Gets the SubscriptionsStartingPageNumber value
    /// </summary>
    public int SubscriptionsStartingPageNumber { get; } = searchConfiguration.SubscriptionsStartingPageNumber;

    /// <summary>
    ///     Gets the SubscriptionsTotalPages value
    /// </summary>
    public int SubscriptionsTotalPages { get; } = searchConfiguration.SubscriptionsTotalPages;

    /// <summary>
    ///     Gets the TopWallpapersTotalPages value
    /// </summary>
    public int TopWallpapersTotalPages { get; } = searchConfiguration.TopWallpapersTotalPages;

    /// <summary>
    ///     Gets the TopWallpapersStartingPageNumber value
    /// </summary>
    public int TopWallpapersStartingPageNumber { get; } = searchConfiguration.TopWallpapersStartingPageNumber;

    /// <summary>
    ///     Gets the SlowMotionDelayInMilliseconds value
    /// </summary>
    public int SlowMotionDelayInMilliseconds { get; } = searchConfiguration.SlowMotionDelayInMilliseconds;
}
