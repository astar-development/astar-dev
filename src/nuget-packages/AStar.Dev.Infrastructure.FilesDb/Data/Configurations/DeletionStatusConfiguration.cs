using AStar.Dev.Infrastructure.Data.Configurations;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AStar.Dev.Infrastructure.FilesDb.Data.Configurations;

internal sealed class DeletionStatusConfiguration : IComplexPropertyConfiguration<DeletionStatus>
{
    public void Configure(ComplexPropertyBuilder<DeletionStatus> builder)
    {
        builder
            .Property(deletionStatus => deletionStatus.HardDeletePending)
            .HasColumnName("HardDeletePending")
            .HasColumnType("datetimeoffset");

        builder
            .Property(deletionStatus => deletionStatus.SoftDeletePending)
            .HasColumnName("SoftDeletePending")
            .HasColumnType("datetimeoffset");

        builder
            .Property(deletionStatus => deletionStatus.SoftDeleted)
            .HasColumnName("SoftDeleted")
            .HasColumnType("datetimeoffset");
    }
}
