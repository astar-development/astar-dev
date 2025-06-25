using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AStar.Dev.Infrastructure.FilesDb.Configurations;

/// <summary>
/// </summary>
public class FileDetailConfiguration : IEntityTypeConfiguration<FileDetail>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<FileDetail> builder)
    {
        _ = builder
            .ToTable(nameof(FileDetail), Constants.SchemaName)
            .HasKey(fileDetail => fileDetail.Id);

        builder.Property(fileDetail => fileDetail.FileName).HasMaxLength(300);
        builder.Property(fileDetail => fileDetail.DirectoryName).HasMaxLength(300);
        builder.Property(fileDetail => fileDetail.FileHandle).HasMaxLength(350);
        builder.HasIndex(fileDetail => fileDetail.FileHandle).IsUnique();
        builder.Ignore(fileDetail => fileDetail.FullNameWithPath);
    }
}
