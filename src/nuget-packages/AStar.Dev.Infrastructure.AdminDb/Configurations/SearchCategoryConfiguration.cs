using AStar.Dev.Infrastructure.AdminDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AStar.Dev.Infrastructure.AdminDb.Configurations;

/// <summary>
///     The <see cref="SearchCategoryConfiguration" /> class containing, you guessed it, the SearchCategory configuration settings
/// </summary>
public sealed class SearchCategoryConfiguration : IEntityTypeConfiguration<SearchCategory>
{
    /// <summary>
    ///     The Configure method will configure the SearchCategory table in the database
    /// </summary>
    /// <param name="builder">An instance of the <see cref="ElementTypeBuilder" /> to add the configuration to</param>
    /// <returns>void</returns>
    public void Configure(EntityTypeBuilder<SearchCategory> builder)
    {
        _ = builder
           .ToTable("SearchCategory", "admin").HasKey(searchCategory => searchCategory.Id);

        builder.Property(p => p.Name).HasMaxLength(300);
        builder.Property(category => category.Id).ValueGeneratedNever();
    }
}
