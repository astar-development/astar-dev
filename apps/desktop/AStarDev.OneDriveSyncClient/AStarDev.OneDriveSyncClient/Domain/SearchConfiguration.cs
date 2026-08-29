using System;

namespace AStarDev.OneDriveSyncClient.Domain;

public record SearchConfiguration(SearchConfigurationId Id, Uri BaseUri, string SearchStringPrefix, string SearchStringSuffix, string TopWallpapers, string Subscriptions, int MaxImagePauseInMilliseconds, int StartingPageNumber, int TotalPages, int SubscriptionsStartingPageNumber, int SubscriptionsTotalPages, int TopWallpapersStartingPageNumber, int TopWallpapersTotalPages, bool UseHeadless);
