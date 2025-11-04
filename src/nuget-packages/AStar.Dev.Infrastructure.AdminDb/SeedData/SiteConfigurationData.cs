using AStar.Dev.Infrastructure.AdminDb.Models;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Infrastructure.AdminDb.SeedData;

/// <summary>
///     The <see cref="SiteConfigurationData" /> adds the default Site Configuration when no entry exists.
/// </summary>
public static class SiteConfigurationData
{
    /// <summary>
    ///     The seed method to populate the database with
    /// </summary>
    /// <param name="context">The <see cref="DbContext" /> to populate</param>
    public static void Seed(DbContext context)
    {
        _ = context.SaveChanges();
        DbSet<SiteConfiguration> siteConfiguration = context.Set<SiteConfiguration>();

        if(siteConfiguration.Any()) return;

        _ = context.Set<SiteConfiguration>()
            .Add(new SiteConfiguration
            {
                            BaseUrl               = "https://wallhaven.cc",
                            LoginUrl              = "login",
                            LoginEmailAddress     = "jason.j.barden2@outlook.com",
                            Password              = "E0CfdJnTMl04OzxqWnxsb0gQutz5dFTyD/0b/KkfDbU=",
                            Username              = "jbarden",
                            SiteConfigurationSlug = "https---wallhaven-cc"
                        });

        _ = context.SaveChanges();
    }
}
