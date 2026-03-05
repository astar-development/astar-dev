namespace AStar.Dev.Admin.Api.Client.Sdk.Models;

/// <summary>
///     The <see href="SearchConfiguration"></see> class containing the full Search Configuration.
/// </summary>
public sealed class SearchConfiguration
{
    /// <summary>
    ///     Gets or sets The ID of the configuration.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     Gets or sets the Base Url for the login and search.
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Login Url.
    /// </summary>
    public string LoginUrl { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Search Categories.
    /// </summary>
    public IList<SearchCategory> SearchCategories { get; set; } = [];

    /// <summary>
    ///     Gets or sets the default Search String.
    /// </summary>
    public string SearchString { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the TopWallpapers something.
    /// </summary>
    public string TopWallpapers { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Search String Prefix.
    /// </summary>
    public string SearchStringPrefix { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Search StringSuffix.
    /// </summary>
    public string SearchStringSuffix { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Subscriptions something.
    /// </summary>
    public string Subscriptions { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the base Image Pause In Seconds.
    /// </summary>
    public int ImagePauseInSeconds { get; set; }

    /// <summary>
    ///     Gets or sets the Starting Page Number.
    /// </summary>
    public int StartingPageNumber { get; set; }

    /// <summary>
    ///     Gets or sets the Total Pages for the New Subscription search.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    ///     Gets or sets the Use Headless to determine whether to run in headless mode or not.
    /// </summary>
    public bool UseHeadless { get; set; }

    /// <summary>
    ///     Gets or sets the Subscriptions Starting Page Number.
    /// </summary>
    public int SubscriptionsStartingPageNumber { get; set; }

    /// <summary>
    ///     Gets or sets the Subscriptions Total Pages.
    /// </summary>
    public int SubscriptionsTotalPages { get; set; }

    /// <summary>
    ///     Gets or sets the Top Wallpapers Total Pages.
    /// </summary>
    public int TopWallpapersTotalPages { get; set; }

    /// <summary>
    ///     Gets or sets the Top Wallpapers Starting Page Number.
    /// </summary>
    public int TopWallpapersStartingPageNumber { get; set; }
}
