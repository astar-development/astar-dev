using AStar.Dev.Utilities;

namespace AStar.Dev.Infrastructure.FilesDb.Models;

public class EventShould
{
    [Fact]
    public void ShouldContainTheExpectedProperties()
        => new Event
           {
               FileName         = "FileName",
               DirectoryName    = "DirectoryName",
               FileSize         = 124,
               FileCreated      = new(2025, 07, 28, 11, 22, 33, TimeSpan.Zero),
               FileLastModified = new(2025, 07, 28, 1, 2, 3, TimeSpan.Zero),
               Height           = 456,
               Id               = 1324,
               UpdatedBy        = "TestUser",
               Width            = 987,
               EventOccurredAt  = new(2025, 07, 28, 1, 24, 34, TimeSpan.Zero),
               Handle           = "mock-handle",
               Type             = EventType.Add
           }
           .ToJson()
           .ShouldMatchApproved();
}
