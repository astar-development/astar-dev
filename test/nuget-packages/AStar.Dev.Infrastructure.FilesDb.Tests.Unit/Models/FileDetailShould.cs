using System.IO.Abstractions;
using JetBrains.Annotations;
using NSubstitute;

namespace AStar.Dev.Infrastructure.FilesDb.Models;

[TestSubject(typeof(FileDetail))]
public sealed class FileDetailShould
{
    [Fact]
    public void ReturnTheExpectedToStringRepresentation()
    {
        var fileDetail = new FileDetail
                         {
                             Id                  = 1,
                             DirectoryName       = "MockDirectoryName",
                             FileCreated         = new (new (2025,               6, 28, 22, 15, 37, DateTimeKind.Utc)),
                             FileLastModified    = new (new (2025, 6, 28, 22, 15, 37, DateTimeKind.Utc)),
                             SoftDeleted         = true,
                             FileName            = "MockFileName",
                             FileSize            = 1234,
                             FileHandle          = "MockFileHandle",
                             FileLastViewed      = new DateTimeOffset(new (2025, 6, 28, 22, 20, 37, DateTimeKind.Utc)),
                             IsImage             = true,
                             ImageDetail         = new(1234, 5678),
                             UpdatedBy           = "Test User",
                             UpdatedOn           = new (new (2025, 6, 28, 22, 30, 37, DateTimeKind.Utc)),
                             FileClassifications = [new () { Id = 1, Name = "Test Classification", Celebrity = true }],
                             HardDeleted         = true,
                             HardDeletePending   = true,
                             MoveRequired        = true,
                             SoftDeletePending   = true
                         };

        fileDetail.ToString().ShouldMatchApproved();
    }

    [Fact]
    public void ReturnTheExpectedDataFromTheCopyConstructor()
    {
        var mockFileInfo      = Substitute.For<IFileInfo>();
        mockFileInfo.Name.Returns("MockFileName");
        mockFileInfo.DirectoryName.Returns("MockDirectoryName");
        mockFileInfo.Length.Returns(1234);

        var fileDetail = new FileDetail(mockFileInfo) { UpdatedOn = new (new (2025, 6, 28, 22, 20, 37, DateTimeKind.Utc)) };

        fileDetail.ToString().ShouldMatchApproved();
    }
}
