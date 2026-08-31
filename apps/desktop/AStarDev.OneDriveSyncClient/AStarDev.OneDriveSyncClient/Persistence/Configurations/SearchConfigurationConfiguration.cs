using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AStarDev.OneDriveSyncClient.Persistence.Configurations;

public class SearchConfigurationConfiguration : IEntityTypeConfiguration<SearchConfiguration>
{
    private static readonly ValueConverter<SearchConfigurationId, Guid> IdConverter =
        new(v => v.Value, v => new SearchConfigurationId(v));

    public void Configure(EntityTypeBuilder<SearchConfiguration> builder)
    {
        builder.ToTable("SearchConfigurations");
        builder.HasKey(sc => sc.Id);
        builder.Property(sc => sc.Id)
            .HasConversion(IdConverter)
            .ValueGeneratedNever();
        builder.Property(sc => sc.BaseUri).IsRequired();
        builder.Property(sc => sc.SearchStringPrefix).IsRequired();
        builder.Property(sc => sc.SearchStringSuffix).IsRequired();
        builder.Property(sc => sc.TopWallpapers).IsRequired();
        builder.Property(sc => sc.Subscriptions).IsRequired();
        builder.Property(sc => sc.MaxImagePauseInMilliseconds).IsRequired();
        builder.Property(sc => sc.StartingPageNumber).IsRequired();
        builder.Property(sc => sc.TotalPages).IsRequired();
        builder.Property(sc => sc.SubscriptionsStartingPageNumber).IsRequired();
        builder.Property(sc => sc.SubscriptionsTotalPages).IsRequired();
        builder.Property(sc => sc.TopWallpapersStartingPageNumber).IsRequired();
        builder.Property(sc => sc.TopWallpapersTotalPages).IsRequired();
        builder.Property(sc => sc.UseHeadless).IsRequired();

        builder.HasMany(sc => sc.SearchCategories)
            .WithOne(sc => sc.SearchConfiguration)
            .HasForeignKey(sc => sc.SearchConfigurationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}