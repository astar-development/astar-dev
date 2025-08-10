using AStar.Dev.Infrastructure.Data.Configurations;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AStar.Dev.Infrastructure.FilesDb.Data.Configurations;

/// <summary>
/// </summary>
public class FileDetailConfiguration : IEntityTypeConfiguration<FileDetail>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<FileDetail> builder)
    {
        builder.ToTable("FileDetail");

        builder.HasKey(file => file.Id);

        builder.Property(file => file.Id)
               .HasConversion(fileId => fileId.Value, fileId => new (fileId));

        builder.Ignore(fileDetail => fileDetail.FileName);
        builder.Ignore(fileDetail => fileDetail.DirectoryName);
        builder.Ignore(fileDetail => fileDetail.FullNameWithPath);

        builder.Property(file => file.FileHandle)
               .HasColumnType("nvarchar(256)")
               .HasConversion(fileHandle => fileHandle.Value, fileHandle => new (fileHandle));

        builder.ComplexProperty(fileDetail => fileDetail.ImageDetail)
               .Configure(new ImageDetailConfiguration());

        builder.ComplexProperty(fileDetail => fileDetail.DirectoryName)
               .Configure(new DirectoryNameConfiguration());

        builder.ComplexProperty(fileDetail => fileDetail.FileName)
               .Configure(new FileNameConfiguration());

        builder.ComplexProperty(fileDetail => fileDetail.DeletionStatus)
               .Configure(new DeletionStatusConfiguration());

        builder.HasIndex(fileDetail => fileDetail.FileHandle).IsUnique();
        builder.HasIndex(fileDetail => fileDetail.FileSize);

        // Composite index to optimize duplicate images search (partial optimization)
        // Note: ImageHeight and ImageWidth can't be indexed directly as they're complex properties
        builder.HasIndex(fileDetail => new { fileDetail.IsImage, fileDetail.FileSize })
               .HasDatabaseName("IX_FileDetail_DuplicateImages");
    }
}
