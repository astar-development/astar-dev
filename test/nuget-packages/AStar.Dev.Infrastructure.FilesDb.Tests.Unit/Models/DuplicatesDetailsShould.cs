// using AStar.Dev.Utilities;
// using JetBrains.Annotations;
//
// namespace AStar.Dev.Infrastructure.FilesDb.Models;
//
// [TestSubject(typeof(DuplicateDetail))]
// public class DuplicatesDetailsShould
// {
//     [Fact]
//     public void ContainTheExpectedProperties()
//         => new DuplicateDetail
//            {
//                DirectoryName     = "MockDirectoryName",
//                SoftDeleted       = new (new (2025, 6, 28, 22, 33, 37, DateTimeKind.Utc)),
//                FileName          = "MockFileName",
//                FileSize          = 1234,
//                FileHandle        = "MockFileHandle",
//                FileLastViewed    = new DateTimeOffset(new (2025, 6, 28, 22, 20, 37, DateTimeKind.Utc)),
//                IsImage           = true,
//                UpdatedOn         = new (new (2025, 6, 28, 22, 30, 37, DateTimeKind.Utc)),
//                HardDeletePending = new (new (2025, 6, 28, 22, 31, 37, DateTimeKind.Utc)),
//                MoveRequired      = true,
//                SoftDeletePending = new (new (2025, 6, 28, 22, 32, 37, DateTimeKind.Utc)),
//                ImageHeight       = 456,
//                ImageWidth        = 123,
//                Instances         = 987
//            }
//            .ToJson().ShouldMatchApproved();
// }


