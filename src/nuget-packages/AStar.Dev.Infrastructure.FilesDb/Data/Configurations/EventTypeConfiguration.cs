using AStar.Dev.Infrastructure.Data.Configurations;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AStar.Dev.Infrastructure.FilesDb.Data.Configurations;

internal sealed class EventTypeConfiguration : IComplexPropertyConfiguration<EventType>
{
    public void Configure(ComplexPropertyBuilder<EventType> builder)
    {
        builder.Property(eventType => eventType.Value).HasColumnName("EventType").IsRequired();
        builder.Property(eventType => eventType.Name).HasColumnName("EventName").IsRequired();
    }
}
