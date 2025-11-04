using AStar.Dev.Infrastructure.AdminDb.Models;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Infrastructure.AdminDb.SeedData;

/// <summary>
///     The <see cref="ScrapeDirectoryData" /> adds the default Scrape Directory configuration when no entry exists
/// </summary>
public static class ScrapeDirectoryData
{
    /// <summary>
    ///     The seed method to populate the database with
    /// </summary>
    /// <param name="context">The <see cref="DbContext" /> to populate</param>
    public static void Seed(DbContext context)
    {
        DbSet<ScrapeDirectory> scrapeDirectories = context.Set<ScrapeDirectory>();

        if(scrapeDirectories.Any()) return;

        _ = context.Set<ScrapeDirectory>()
            .Add(new ScrapeDirectory
            {
                            RootDirectory       = @"C:\Users\jason_17jx22b",
                            BaseSaveDirectory   = @"/home/jason-barden/snap/onedrive-cli/current/OneDrive/Wallpapers\",
                            BaseDirectory       = @"/home/jason-barden/snap/onedrive-cli/current/OneDrive/Wallpapers\Wallhaven\",
                            BaseDirectoryFamous = @"/home/jason-barden/snap/onedrive-cli/current/OneDrive/Famous\",
                            SubDirectoryName    = "New-Subscription-Wallpapers"
                        });
    }
}
