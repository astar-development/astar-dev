using AStar.Dev.Infrastructure.AdminDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AStar.Dev.Infrastructure.AdminDb.Configurations;

/// <summary>
///     The <see cref="ScrapeDirectoryConfiguration" /> class containing, you guessed it, the ScrapeDirectory configuration settings
/// </summary>
public sealed class ScrapeDirectoryConfiguration : IEntityTypeConfiguration<ScrapeDirectory>
{
    /// <summary>
    ///     The Configure method will configure the ScrapeDirectory table in the database
    /// </summary>
    /// <param name="builder">An instance of the <see cref="ElementTypeBuilder" /> to add the configuration to</param>
    /// <returns>void</returns>
    public void Configure(EntityTypeBuilder<ScrapeDirectory> builder)
    {
        _ = builder
           .ToTable("ScrapeDirectory", "admin").HasKey(scrapeDirectory => scrapeDirectory.Id);

        builder.Property(p => p.RootDirectory).HasMaxLength(300);
        builder.Property(p => p.BaseDirectory).HasMaxLength(300);
        builder.Property(p => p.BaseSaveDirectory).HasMaxLength(300);
        builder.Property(p => p.BaseDirectoryFamous).HasMaxLength(300);
        builder.Property(p => p.SubDirectoryName).HasMaxLength(300);
    }
}
