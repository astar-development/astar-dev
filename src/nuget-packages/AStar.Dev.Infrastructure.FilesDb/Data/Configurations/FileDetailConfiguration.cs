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
        _ = builder.ToTable("FileDetail");

        _ = builder.HasKey(file => file.Id);

        _ = builder.Property(file => file.Id)
                   .ValueGeneratedNever()
                   .HasConversion(fileId => fileId.Value, fileId => new FileId { Value = fileId });

        _ = builder.Ignore(fileDetail => fileDetail.FileName);
        _ = builder.Ignore(fileDetail => fileDetail.DirectoryName);
        _ = builder.Ignore(fileDetail => fileDetail.FullNameWithPath);
        _ = builder.Ignore(fileDetail => fileDetail.ImageDetail);

        _ = builder.Property(file => file.FileHandle)
                   .HasColumnType("nvarchar(256)")
                   .HasConversion(fileHandle => fileHandle.Value, fileHandle => new FileHandle(fileHandle));

        _ = builder.ComplexProperty(fileDetail => fileDetail.DirectoryName)
                   .Configure(new DirectoryNameConfiguration());

        _ = builder.ComplexProperty(fileDetail => fileDetail.FileName)
                   .Configure(new FileNameConfiguration());

        _ = builder.ComplexProperty(fileDetail => fileDetail.ImageDetail)
                   .Configure(new ImageDetailConfiguration());

        _ = builder.HasMany(fd => fd.FileClassifications)
                   .WithMany(fc => fc.FileDetails)
                   .UsingEntity<Dictionary<string, object>>(
                                                            "FileDetailClassification",
                                                            j => j.HasOne<FileClassification>()
                                                                  .WithMany()
                                                                  .HasForeignKey("FileClassificationId")
                                                                  .OnDelete(DeleteBehavior.Cascade),
                                                            j => j.HasOne<FileDetail>()
                                                                  .WithMany()
                                                                  .HasForeignKey("FileDetailId")
                                                                  .OnDelete(DeleteBehavior.Cascade),
                                                            j =>
                                                            {
                                                                _ = j.HasKey("FileDetailId", "FileClassificationId");
                                                                _ = j.ToTable("FileDetailClassifications");
                                                            });

        _ = builder.HasIndex(fileDetail => fileDetail.FileHandle).IsUnique();
        _ = builder.HasIndex(fileDetail => fileDetail.FileSize);

        // Composite index to optimize duplicate images search (partial optimization)
        // Note: ImageHeight and ImageWidth can't be indexed directly as they're complex properties
        _ = builder.HasIndex(fileDetail => new { fileDetail.IsImage, fileDetail.FileSize })
                   .HasDatabaseName("IX_FileDetail_DuplicateImages");
    }
}
