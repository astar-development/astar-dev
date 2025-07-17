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
        builder.ToTable("FileDetail");

        builder.HasKey(file => file.Id);

        builder.Property(file => file.FileName).HasColumnType("nvarchar(256)");

        builder.Property(file => file.DirectoryName).HasColumnType("nvarchar(256)");

        builder.Property(file => file.FileHandle).HasColumnType("nvarchar(256)");

        builder.ComplexProperty(fileDetail => fileDetail.ImageDetail).Configure(new ImageDetailConfiguration());

        builder.ComplexProperty(fileDetail => fileDetail.DeletionStatus).Configure(new DeletionStatusConfiguration());
    }
}
