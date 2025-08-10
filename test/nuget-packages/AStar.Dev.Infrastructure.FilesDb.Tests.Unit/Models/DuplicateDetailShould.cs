using AStar.Dev.Utilities;

namespace AStar.Dev.Infrastructure.FilesDb.Models;

public class DuplicateDetailShould
{
    [Fact]
    public void ContainTheExpectedProperties()
        => new DuplicateDetail
           {
               FileHandle        = new("mock-file-handle"),
               DirectoryName     = new("mock-directory-name"),
               FileName          = new("mock-file-name"),
               FileSize          = 1245,
               IsImage           = true,
               FileLastViewed    = new DateTimeOffset(2025, 07, 28, 11, 22, 33, TimeSpan.Zero),
               UpdatedOn         = new(2025, 07, 28, 8, 7, 9, TimeSpan.Zero),
               MoveRequired      = true,
               ImageHeight       = 123,
               ImageWidth        = 345,
               HardDeletePending = new DateTimeOffset(2025, 07, 28, 2, 3, 4, TimeSpan.Zero),
               SoftDeletePending = new DateTimeOffset(2025, 07, 28, 3, 4, 5, TimeSpan.Zero),
               SoftDeleted       = new DateTimeOffset(2025, 07, 28, 5, 6, 1, TimeSpan.Zero),
               Instances         = 2
           }
           .ToJson()
           .ShouldMatchApproved();
}
