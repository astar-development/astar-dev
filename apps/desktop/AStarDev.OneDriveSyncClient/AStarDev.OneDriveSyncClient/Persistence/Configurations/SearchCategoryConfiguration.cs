using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AStarDev.OneDriveSyncClient.Persistence.Configurations;

public class SearchCategoryConfiguration : IEntityTypeConfiguration<SearchCategory>
{
    private static readonly ValueConverter<SearchCategoryId, Guid> IdConverter =
        new(v => v.Value, v => new SearchCategoryId(v));

    private static readonly ValueConverter<SearchConfigurationId, Guid> SearchConfigurationIdConverter =
        new(v => v.Value, v => new SearchConfigurationId(v));

    public void Configure(EntityTypeBuilder<SearchCategory> builder)
    {
        builder.ToTable("SearchCategories");
        builder.HasKey(sc => sc.Id);
        builder.Property(sc => sc.Id)
            .HasConversion(IdConverter)
            .ValueGeneratedNever();
        builder.Property(sc => sc.SearchConfigurationId)
            .HasConversion(SearchConfigurationIdConverter)
            .IsRequired();
        builder.Property(sc => sc.Name).IsRequired();
        builder.Property(sc => sc.LastKnownImageCount).IsRequired();
        builder.Property(sc => sc.LastPageVisited).IsRequired();
        builder.Property(sc => sc.TotalPages).IsRequired();

        builder.HasOne(sc => sc.SearchConfiguration)
            .WithMany(sc => sc.SearchCategories)
            .HasForeignKey(sc => sc.SearchConfigurationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}