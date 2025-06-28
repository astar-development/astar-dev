using AStar.Dev.Utilities;
using JetBrains.Annotations;

namespace AStar.Dev.Infrastructure.FilesDb.Models;

[TestSubject(typeof(Events))]
public class EventsShould
{
    [Fact]
    public void ContainTheExpectedProperties()
        => new Events
           {
               Id               = 1,
               Height           = 123,
               Width            = 456,
               DirectoryName    = "Mock Directory Name",
               FileCreated      = new (new (2025,               6, 28, 22, 05, 37, DateTimeKind.Utc)),
               FileName         = "Mock Filename",
               FileSize         = 1234,
               ModifiedBy       = "Test User",
               EventOccurredAt  = new (new (2025,               6, 28, 22, 15, 37, DateTimeKind.Utc)),
               EventType        = "Mock Event",
               FileLastModified = new (new (2025,               6, 28, 22, 10, 37, DateTimeKind.Utc)),
               Handle           = "Mock Handle"
           }.ToJson().ShouldMatchApproved();
}
