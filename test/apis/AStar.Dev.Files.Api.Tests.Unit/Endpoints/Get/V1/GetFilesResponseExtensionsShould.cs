using AStar.Dev.Infrastructure.FilesDb.Models;
using AStar.Dev.Utilities;

namespace AStar.Dev.Files.Api.Endpoints.Get.V1;

public class GetFilesResponseExtensionsShould
{
    // [Fact]
    // public void MapTheDuplicateDetailsAsExpected()
    // {
    //     var duplicateDetail = new DuplicateDetail
    //                           {
    //                               DirectoryName     = "MockDirectoryName",
    //                               FileName          = "MockFileName",
    //                               FileHandle        = "MockFileHandle",
    //                               FileSize          = 12345,
    //                               FileLastViewed    = new (2025, 12, 23, 0, 1, 2, TimeSpan.Zero),
    //                               HardDeletePending = new (2025, 12, 23, 0, 2, 2, TimeSpan.Zero),
    //                               SoftDeletePending = new (2025, 12, 23, 0, 3, 2, TimeSpan.Zero),
    //                               SoftDeleted       = new (2025, 12, 23, 0, 4, 2, TimeSpan.Zero),
    //                               UpdatedOn         = new (2025, 12, 23, 0, 5, 2, TimeSpan.Zero),
    //                               ImageHeight       = 123,
    //                               ImageWidth        = 456,
    //                               Instances         = 1,
    //                               IsImage           = true
    //                           };
    //
    //     var sut = duplicateDetail.ToGetFilesResponse();
    //
    //     sut.ToJson().ShouldMatchApproved();
    // }

    [Fact]
    public void MapTheFileDetailsAsExpected()
    {
        var fileDetail = new FileDetail
                         {
                             DirectoryName  = "MockDirectoryName",
                             FileName       = "MockFileName",
                             FileHandle     = "MockFileHandle",
                             FileSize       = 12345,
                             FileLastViewed = new (2025, 12, 23, 0, 1, 2, TimeSpan.Zero),
                             DeletionStatus = new()
                                              {
                                                  HardDeletePending = new (2025, 12, 23, 0, 2, 2, TimeSpan.Zero),
                                                  SoftDeletePending = new (2025, 12, 23, 0, 3, 2, TimeSpan.Zero),
                                                  SoftDeleted       = new (2025, 12, 23, 0, 4, 2, TimeSpan.Zero)
                                              },
                             UpdatedOn   = new (2025, 12, 23, 0, 5, 2, TimeSpan.Zero),
                             ImageDetail = new(123, 456),
                             IsImage     = true
                         };

        var sut = fileDetail.ToGetFilesResponse();

        sut.ToJson().ShouldMatchApproved();
    }
}
