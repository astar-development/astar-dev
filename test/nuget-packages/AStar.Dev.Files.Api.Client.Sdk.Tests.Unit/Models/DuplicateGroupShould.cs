using AStar.Dev.Files.Api.Client.SDK.Models;

namespace AStar.Dev.Files.Api.Client.Sdk.Models;

public sealed class DuplicateGroupShould
{
    [Fact]
    public void ReturnTheExpectedToString() =>
        new DuplicateGroup
            {
                Duplicates = new List<DuplicatesDetails>
                             {
                                 new ()
                                 {
                                     DetailsLastUpdated = new (2025, 12, 23) ,
                                     FileName           = "MockFileName",
                                     FileSize           = 1234,
                                     Height             = 345,
                                     Width              = 567,
                                     DirectoryName      = "MockDirectoryName",
                                     FileAccessDetailId = 1,
                                     FileHandle         = "MockFileHandle",
                                     HardDeletePending  = true,
                                     Id                 = 2,
                                     Instances          = 3,
                                     IsImage            = true,
                                     LastViewed         = new DateTime(2025, 12, 24),
                                     MoveRequired       = true,
                                     SoftDeleted        = true,
                                     SoftDeletePending  = true
                                 }
                             },
                FileSize     = 123456,
                FileGrouping = { FileSize = 2345, Height = 1432, Width = 2221 }
            }
            .ToString()
            .ShouldMatchApproved();
}
