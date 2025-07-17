using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AStar.Dev.Infrastructure.FilesDb.Configurations;

internal class DeletionStatusConfiguration : IComplexPropertyConfiguration<DeletionStatus>
{
    public ComplexPropertyBuilder<DeletionStatus> Configure(ComplexPropertyBuilder<DeletionStatus> builder)
    {
        builder.Property(deletionStatus => deletionStatus.HardDeletePending).HasColumnName("HardDeletePending").HasColumnType("datetimeoffset").IsRequired(false);
        builder.Property(deletionStatus => deletionStatus.SoftDeletePending).HasColumnName("SoftDeletePending").HasColumnType("datetimeoffset").IsRequired(false);
        builder.Property(deletionStatus => deletionStatus.SoftDeleted).HasColumnName("SoftDeleted").HasColumnType("datetimeoffset").IsRequired(false);

        return builder;
    }
}
