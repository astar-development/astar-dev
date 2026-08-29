using System;
using System.Collections.Generic;

namespace AStarDev.OneDriveSyncClient.Persistence;

public class SearchConfiguration
{
    public SearchConfiguration(SearchConfigurationId id, Uri baseUri, string searchStringPrefix, string searchStringSuffix, string topWallpapers, string subscriptions, int maxImagePauseInMilliseconds, int startingPageNumber, int totalPages, int subscriptionsStartingPageNumber, int subscriptionsTotalPages, int topWallpapersStartingPageNumber, int topWallpapersTotalPages, bool useHeadless)
    {
        Id = id;
        BaseUri = baseUri;
        SearchStringPrefix = searchStringPrefix;
        SearchStringSuffix = searchStringSuffix;
        TopWallpapers = topWallpapers;
        Subscriptions = subscriptions;
        MaxImagePauseInMilliseconds = maxImagePauseInMilliseconds;
        StartingPageNumber = startingPageNumber;
        TotalPages = totalPages;
        SubscriptionsStartingPageNumber = subscriptionsStartingPageNumber;
        SubscriptionsTotalPages = subscriptionsTotalPages;
        TopWallpapersStartingPageNumber = topWallpapersStartingPageNumber;
        TopWallpapersTotalPages = topWallpapersTotalPages;
        UseHeadless = useHeadless;
        SearchCategories = new List<SearchCategory>();
    }

    public SearchConfigurationId Id { get; init; }

    public Uri BaseUri { get; init; }

    public string SearchStringPrefix { get; init; }

    public string SearchStringSuffix { get; init; }

    public string TopWallpapers { get; init; }

    public string Subscriptions { get; init; }

    public int MaxImagePauseInMilliseconds { get; init; }

    public int StartingPageNumber { get; init; }

    public int TotalPages { get; init; }

    public int SubscriptionsStartingPageNumber { get; init; }

    public int SubscriptionsTotalPages { get; init; }

    public int TopWallpapersStartingPageNumber { get; init; }

    public int TopWallpapersTotalPages { get; init; }

    public bool UseHeadless { get; init; }

    public ICollection<SearchCategory> SearchCategories { get; private set; } = new List<SearchCategory>();
}