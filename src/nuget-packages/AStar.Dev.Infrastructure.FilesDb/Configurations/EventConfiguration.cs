﻿using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AStar.Dev.Infrastructure.FilesDb.Configurations;

/// <summary>
/// </summary>
public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        _ = builder
            .ToTable(nameof(Event), Constants.SchemaName)
            .HasKey(fileDetail => fileDetail.Id);

        builder.Property(fileDetail => fileDetail.FileName).HasMaxLength(256);
        builder.Property(fileDetail => fileDetail.DirectoryName).HasMaxLength(256);
        builder.Property(fileDetail => fileDetail.Handle).HasMaxLength(256);
        builder.Property(fileDetail => fileDetail.UpdatedBy).HasMaxLength(30);

        builder.ComplexProperty(fileDetail => fileDetail.Type).Configure(new EventTypeConfiguration());
    }
}
