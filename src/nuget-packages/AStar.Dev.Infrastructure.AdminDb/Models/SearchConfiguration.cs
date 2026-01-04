using AStar.Dev.Infrastructure.Data;

namespace AStar.Dev.Infrastructure.AdminDb.Models;

/// <summary>
///     The <see href="SearchConfiguration"></see> class extends <see cref="AuditableEntity" />.
///     The <see href="SearchConfiguration"></see> class containing the full Scrape Configuration.
/// </summary>
public sealed class SearchConfiguration : AuditableEntity
{
    /// <summary>
    ///     Gets or sets The ID of the configuration.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    ///     Gets or sets the SearchConfigurationId - whilst the name sounds the same, this is the search-configuration-specific
    ///     ID and is used as part of the Historical modelling for each Search Configuration contained within the table
    /// </summary>
    public Guid SearchConfigurationId { get; init; } = Guid.CreateVersion7();

    /// <summary>
    ///     Gets or sets the default Search String
    /// </summary>
    public string SearchString { get; init; } = string.Empty;

    /// <summary>
    ///     Gets or sets the TopWallpapers URI
    /// </summary>
    public string TopWallpapers { get; init; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Search String Prefix
    /// </summary>
    public string SearchStringPrefix { get; init; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Search StringSuffix
    /// </summary>
    public string SearchStringSuffix { get; init; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Subscriptions something
    /// </summary>
    public string Subscriptions { get; init; } = string.Empty;

    /// <summary>
    ///     Gets or sets the base Image Pause In Seconds
    /// </summary>
    public int ImagePauseInSeconds { get; init; }

    /// <summary>
    ///     Gets or sets the Starting Page Number
    /// </summary>
    public int StartingPageNumber { get; init; }

    /// <summary>
    ///     Gets or sets the Total Pages for the New Subscription search
    /// </summary>
    public int TotalPages { get; init; }

    /// <summary>
    ///     Gets or sets the Use Headless to determine whether to run in headless mode or not
    /// </summary>
    public bool UseHeadless { get; init; }

    /// <summary>
    ///     Gets or sets the Subscriptions Starting Page Number
    /// </summary>
    public int SubscriptionsStartingPageNumber { get; init; }

    /// <summary>
    ///     Gets or sets the Subscriptions Total Pages
    /// </summary>
    public int SubscriptionsTotalPages { get; init; }

    /// <summary>
    ///     Gets or sets the Top Wallpapers Total Pages
    /// </summary>
    public int TopWallpapersTotalPages { get; init; }

    /// <summary>
    ///     Gets or sets the Top Wallpapers Starting Page Number
    /// </summary>
    public int TopWallpapersStartingPageNumber { get; init; }

    /// <summary>
    ///     Gets or sets the slow-motion delay (in milliseconds)
    /// </summary>
    public int SlowMotionDelayInMilliseconds { get; init; }

    /// <summary>
    ///     Gets or sets the SiteConfigurationSlug - this is the site-specific SiteConfigurationSlug (text representation of the site) and is
    ///     used as part of the Historical modelling for each Site Configuration contained within the table
    /// </summary>
    public string SiteConfigurationSlug { get; init; } = string.Empty;
}
