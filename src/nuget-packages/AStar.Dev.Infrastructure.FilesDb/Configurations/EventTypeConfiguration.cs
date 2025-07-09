using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AStar.Dev.Infrastructure.FilesDb.Configurations;

internal class EventTypeConfiguration : IComplexPropertyConfiguration<EventType>
{
    public ComplexPropertyBuilder<EventType> Configure(ComplexPropertyBuilder<EventType> builder)
    {
        builder.Property(image => image.Value).HasColumnName("EventType").IsRequired();
        builder.Property(image => image.Name).HasColumnName("EventName").IsRequired();

        return builder;
    }
}
