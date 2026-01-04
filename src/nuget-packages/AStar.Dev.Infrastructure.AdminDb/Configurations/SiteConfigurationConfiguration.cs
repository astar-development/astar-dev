using AStar.Dev.Infrastructure.AdminDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AStar.Dev.Infrastructure.AdminDb.Configurations;

/// <summary>
///     The <see cref="SiteConfigurationConfiguration" /> class containing, you guessed it, the SiteConfiguration configuration settings
/// </summary>
public sealed class SiteConfigurationConfiguration : IEntityTypeConfiguration<SiteConfiguration>
{
    /// <summary>
    ///     The Configure method will configure the SiteConfiguration table in the database
    /// </summary>
    /// <param name="builder">An instance of the <see cref="ElementTypeBuilder" /> to add the configuration to</param>
    /// <returns>void</returns>
    public void Configure(EntityTypeBuilder<SiteConfiguration> builder)
    {
        _ = builder
           .ToTable("SiteConfiguration", "admin")
           .HasKey(siteConfiguration => siteConfiguration.Id);

        builder.Property(p => p.LoginEmailAddress).HasMaxLength(255);
        builder.Property(p => p.Username).HasMaxLength(50);
        builder.Property(p => p.Password).HasMaxLength(50);
        builder.Property(p => p.BaseUrl).HasMaxLength(300);
        builder.Property(p => p.LoginUrl).HasMaxLength(300);
        builder.Property(p => p.SiteConfigurationSlug).HasMaxLength(300);
    }
}
