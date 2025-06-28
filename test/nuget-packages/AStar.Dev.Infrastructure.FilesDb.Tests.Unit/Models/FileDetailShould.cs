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
                             ImageDetails        = new() { FileDetailsId = 1, Height = 123, Width = 456, Id = 1 },
                             ModifiedBy          = "Test User",
                             DetailsModified     = new (new (2025, 6, 28, 22, 30, 37, DateTimeKind.Utc)),
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
        var fInfo      = Substitute.For<IFileInfo>();
        fInfo.Name.Returns("MockFileName");
        fInfo.DirectoryName.Returns("MockDirectoryName");
        fInfo.Length.Returns(1234);

        var fileDetail = new FileDetail(fInfo);

        fileDetail.ToString().ShouldMatchApproved();
    }
}
