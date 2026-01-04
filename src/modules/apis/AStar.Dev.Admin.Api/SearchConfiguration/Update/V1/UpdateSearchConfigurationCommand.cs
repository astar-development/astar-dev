namespace AStar.Dev.Admin.Api.SearchConfiguration.Update.V1;

/// <summary>
///     The UpdateSearchConfigurationCommand class contains the parameters required to Update the Site Configuration.
///     <para>
///         The class implements the <see cref="IRequest{IResult}" /> as defined within MediatR.
///     </para>
/// </summary>
public sealed class UpdateSearchConfigurationCommand
{
    /// <summary>
    ///     Gets or sets the SiteConfigurationSlug for the Site Configuration.
    /// </summary>
    public string SiteConfigurationSlug { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Base Url for the login and search.
    /// </summary>
    public string SearchString { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Login Url.
    /// </summary>
    public string Subscriptions { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the SearchStringPrefix.
    /// </summary>
    public string SearchStringPrefix { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the TopWallpapers.
    /// </summary>
    public string TopWallpapers { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the TotalPages.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    ///     Gets or sets the Image Pause in Seconds value
    /// </summary>
    public int ImagePauseInSeconds { get; set; }

    /// <summary>
    ///     Gets or sets the SearchStringSuffix value
    /// </summary>
    public string SearchStringSuffix { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the StartingPageNumber value
    /// </summary>
    public int StartingPageNumber { get; set; }

    /// <summary>
    ///     Gets or sets the UseHeadless value
    /// </summary>
    public bool UseHeadless { get; set; }

    /// <summary>
    ///     Gets or sets the SubscriptionsStartingPageNumber value
    /// </summary>
    public int SubscriptionsStartingPageNumber { get; set; }

    /// <summary>
    ///     Gets or sets the SubscriptionsTotalPages value
    /// </summary>
    public int SubscriptionsTotalPages { get; set; }

    /// <summary>
    ///     Gets or sets the TopWallpapersTotalPages value
    /// </summary>
    public int TopWallpapersTotalPages { get; set; }

    /// <summary>
    ///     Gets or sets the TopWallpapersStartingPageNumber value
    /// </summary>
    public int TopWallpapersStartingPageNumber { get; set; }

    /// <summary>
    ///     Gets or sets the SlowMotionDelayInMilliseconds value
    /// </summary>
    public int SlowMotionDelayInMilliseconds { get; set; }
}
