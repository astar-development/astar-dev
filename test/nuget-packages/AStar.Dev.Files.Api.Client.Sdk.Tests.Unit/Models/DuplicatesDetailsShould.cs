using AStar.Dev.Files.Api.Client.SDK.Models;
using AStar.Dev.Utilities;
using JetBrains.Annotations;

namespace AStar.Dev.Files.Api.Client.Sdk.Models;

[TestSubject(typeof(DuplicatesDetails))]
public class DuplicatesDetailsShould
{
    [Fact]
    public void ContainTheExpectedProperties()
        => new DuplicatesDetails
           {
               DetailsLastUpdated = DateTime.MaxValue,
               FileSize           = 1234,
               Height             = 123,
               Width              = 456,
               DirectoryName      = "MockDirectory",
               FileAccessDetailId = 567,
               FileHandle         = "MockHandle",
               FileName           = "MockFileName",
               HardDeletePending  = true,
               Id                 = 1,
               Instances          = 2,
               IsImage            = true,
               LastViewed         = new DateTime(2025, 12, 23),
               MoveRequired       = true,
               SoftDeleted        = true,
               SoftDeletePending  = true
           }
           .ToJson()
           .ShouldMatchApproved();
}
