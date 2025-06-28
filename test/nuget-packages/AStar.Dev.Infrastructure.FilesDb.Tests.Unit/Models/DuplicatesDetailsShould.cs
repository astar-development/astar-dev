using AStar.Dev.Utilities;
using JetBrains.Annotations;

namespace AStar.Dev.Infrastructure.FilesDb.Models;

[TestSubject(typeof(DuplicatesDetails))]
public class DuplicatesDetailsShould
{
    [Fact]
    public void ContainTheExpectedProperties()
        => new DuplicatesDetails
           {
               Id                 = 1,
               ImageDetailsId     = 1,
               DirectoryName      = "MockDirectoryName",
               SoftDeleted        = true,
               FileName           = "MockFileName",
               FileSize           = 1234,
               FileHandle         = "MockFileHandle",
               LastViewed         = new DateTimeOffset(new (2025, 6, 28, 22, 20, 37, DateTimeKind.Utc)),
               IsImage            = true,
               DetailsLastUpdated = new (new (2025, 6, 28, 22, 30, 37, DateTimeKind.Utc)),
               HardDeletePending  = true,
               MoveRequired       = true,
               SoftDeletePending  = true,
               Height             = 456,
               Width              = 123,
               Instances          = 987
           }
           .ToJson().ShouldMatchApproved();
}
