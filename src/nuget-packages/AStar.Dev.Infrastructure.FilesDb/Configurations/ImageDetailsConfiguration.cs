using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AStar.Dev.Infrastructure.FilesDb.Configurations;

/// <summary>
/// </summary>
public class ImageDetailsConfiguration : IEntityTypeConfiguration<ImageDetails>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<ImageDetails> builder) =>
        _ = builder
            .ToTable(nameof(ImageDetails), Constants.SchemaName)
            .HasKey(fileDetail => fileDetail.Id);
}
