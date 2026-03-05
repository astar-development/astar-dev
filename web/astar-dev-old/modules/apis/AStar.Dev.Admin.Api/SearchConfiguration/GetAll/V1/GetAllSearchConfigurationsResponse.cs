namespace AStar.Dev.Admin.Api.SearchConfiguration.GetAll.V1;

/// <summary>
///     The <see cref="GetAllSearchConfigurationsResponse" /> class used to return the Site Configuration to the caller.
/// </summary>
public sealed class GetAllSearchConfigurationsResponse(Infrastructure.AdminDb.Models.SearchConfiguration searchConfiguration)
{
    /// <summary>
    ///     Gets or sets the default Search String.
    /// </summary>
    public string SearchString { get; init; } = searchConfiguration.SearchString;

    /// <summary>
    ///     Gets or sets the TopWallpapers URI.
    /// </summary>
    public string TopWallpapers { get; init; } = searchConfiguration.TopWallpapers;

    /// <summary>
    ///     Gets or sets the Search String Prefix.
    /// </summary>
    public string SearchStringPrefix { get; init; } = searchConfiguration.SearchStringPrefix;

    /// <summary>
    ///     Gets or sets the Search StringSuffix.
    /// </summary>
    public string SearchStringSuffix { get; init; } = searchConfiguration.SearchStringSuffix;

    /// <summary>
    ///     Gets or sets the Subscriptions something.
    /// </summary>
    public string Subscriptions { get; init; } = searchConfiguration.Subscriptions;

    /// <summary>
    ///     Gets or sets the base Image Pause In Seconds.
    /// </summary>
    public int ImagePauseInSeconds { get; init; } = searchConfiguration.ImagePauseInSeconds;

    /// <summary>
    ///     Gets or sets the Starting Page Number.
    /// </summary>
    public int StartingPageNumber { get; init; } = searchConfiguration.StartingPageNumber;

    /// <summary>
    ///     Gets or sets the Total Pages for the New Subscription search.
    /// </summary>
    public int TotalPages { get; init; } = searchConfiguration.TotalPages;

    /// <summary>
    ///     Gets or sets the Use Headless to determine whether to run in headless mode or not.
    /// </summary>
    public bool UseHeadless { get; init; } = searchConfiguration.UseHeadless;

    /// <summary>
    ///     Gets or sets the Subscriptions Starting Page Number.
    /// </summary>
    public int SubscriptionsStartingPageNumber { get; init; } = searchConfiguration.SubscriptionsStartingPageNumber;

    /// <summary>
    ///     Gets or sets the Subscriptions Total Pages.
    /// </summary>
    public int SubscriptionsTotalPages { get; init; } = searchConfiguration.SubscriptionsTotalPages;

    /// <summary>
    ///     Gets or sets the Top Wallpapers Total Pages.
    /// </summary>
    public int TopWallpapersTotalPages { get; init; } = searchConfiguration.TopWallpapersTotalPages;

    /// <summary>
    ///     Gets or sets the Top Wallpapers Starting Page Number.
    /// </summary>
    public int TopWallpapersStartingPageNumber { get; init; } = searchConfiguration.TopWallpapersStartingPageNumber;

    /// <summary>
    ///     Gets or sets the slow-motion delay (in milliseconds).
    /// </summary>
    public int SlowMotionDelayInMilliseconds { get; init; } = searchConfiguration.SlowMotionDelayInMilliseconds;

    /// <summary>
    ///     Gets or sets the SiteConfigurationSlug - this is the site-specific SiteConfigurationSlug (text representation of the site) and is
    ///     used as part of the Historical modelling for each Site Configuration contained within the table.
    /// </summary>
    public string SiteConfigurationSlug { get; init; } = searchConfiguration.SiteConfigurationSlug;

    /// <summary>
    ///     Gets or sets the Updated By property to track who made the change.
    /// </summary>
    public string UpdatedBy { get; set; } = searchConfiguration.UpdatedBy;

    /// <summary>
    ///     Sets the date and time of the update. This is specified in UTC.
    /// </summary>
    public DateTime UpdatedOn { get; set; } = searchConfiguration.UpdatedOn;
}
