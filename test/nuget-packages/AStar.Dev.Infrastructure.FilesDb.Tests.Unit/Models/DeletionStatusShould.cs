using AStar.Dev.Utilities;

namespace AStar.Dev.Infrastructure.FilesDb.Models;

public class DeletionStatusShould
{
    [Fact]
    public void ContainTheExpectedProperties()
        => new DeletionStatus
           {
               SoftDeleted       = new DateTimeOffset(2025, 7, 28, 1, 2, 3, TimeSpan.Zero),
               HardDeletePending = new DateTimeOffset(2025, 7, 28, 2, 3, 4, TimeSpan.Zero),
               SoftDeletePending = new DateTimeOffset(2025, 7, 28, 3, 4, 5, TimeSpan.Zero)
           }
           .ToJson()
           .ShouldMatchApproved();
}
