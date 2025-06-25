using AStar.Dev.Infrastructure.AdminDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AStar.Dev.Infrastructure.AdminDb.Configurations;

/// <summary>
///     The <see cref="SearchConfigurationConfiguration" /> class containing, you guessed it, the SearchConfiguration configuration settings
/// </summary>
public sealed class SearchConfigurationConfiguration : IEntityTypeConfiguration<SearchConfiguration>
{
    /// <summary>
    ///     The Configure method will configure the SearchConfiguration table in the database
    /// </summary>
    /// <param name="builder">An instance of the <see cref="ElementTypeBuilder" /> to add the configuration to</param>
    /// <returns>void</returns>
    public void Configure(EntityTypeBuilder<SearchConfiguration> builder)
    {
        _ = builder
           .ToTable("SearchConfiguration", "admin").HasKey(searchConfiguration => searchConfiguration.Id);

        builder.Property(p => p.SearchString).HasMaxLength(300);
        builder.Property(p => p.TopWallpapers).HasMaxLength(300);
        builder.Property(p => p.SearchStringPrefix).HasMaxLength(300);
        builder.Property(p => p.SearchStringSuffix).HasMaxLength(300);
        builder.Property(p => p.Subscriptions).HasMaxLength(300);
        builder.Property(p => p.SiteConfigurationSlug).HasMaxLength(300);
    }
}
