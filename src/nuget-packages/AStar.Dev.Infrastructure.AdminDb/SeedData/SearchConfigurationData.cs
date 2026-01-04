using AStar.Dev.Infrastructure.AdminDb.Models;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Infrastructure.AdminDb.SeedData;

/// <summary>
///     The <see cref="SearchConfigurationData" /> adds the default Search Configuration when no entry exists
/// </summary>
public static class SearchConfigurationData
{
    /// <summary>
    ///     The seed method to populate the database with
    /// </summary>
    /// <param name="context">The <see cref="DbContext" /> to populate</param>
    public static void Seed(DbContext context)
    {
        DbSet<SearchConfiguration> searchCategories = context.Set<SearchConfiguration>();

        if(searchCategories.Any()) return;

        context.Set<SearchConfiguration>()
               .Add(new SearchConfiguration
                    {
                        SearchString                    = @"/search?q=id:872\u0026categories=001\u0026purity=111\u0026sorting=date_added\u0026order=desc\u0026ai_art_filter=0\u0026page=",
                        TopWallpapers                   = @"/search?categories=001\u0026purity=111\u0026topRange=1M\u0026sorting=toplist\u0026order=desc\u0026ai_art_filter=0\u0026page=",
                        SearchStringPrefix              = "/search?q=id:",
                        SearchStringSuffix              = @"\u0026categories=001\u0026purity=111\u0026sorting=date_added\u0026order=desc\u0026ai_art_filter=0\u0026page=""",
                        Subscriptions                   = "subscription?page=",
                        ImagePauseInSeconds             = 5,
                        SlowMotionDelayInMilliseconds   = 500,
                        TotalPages                      = 0,
                        SubscriptionsTotalPages         = 0,
                        SubscriptionsStartingPageNumber = 0,
                        StartingPageNumber              = 1,
                        TopWallpapersTotalPages         = 0,
                        TopWallpapersStartingPageNumber = 1,
                        UseHeadless                     = false,
                    });
    }
}
