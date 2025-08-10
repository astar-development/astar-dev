using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Test.DbContext.Helpers.Fixtures;
using Shouldly;

namespace AStar.Dev.Infrastructure.FilesDb.Tests.Integration;

/// <summary>
/// </summary>
public class FileDetailDeletionStatusExtensionsShould(FilesContextFixture filesContextFixture) : IClassFixture<FilesContextFixture>
{
    [Fact]
    public void ShouldReturnExpectedFileDetailsWhenIncludeDeletedOrDeletePendingIsFalse()
    {
        var sut = filesContextFixture.Sut;

        var result = sut.FileDetails.IncludeDeletedOrDeletePending(false).ToList();

        result.Count.ShouldBe(2600);
    }

    [Fact]
    public void ShouldReturnExpectedFileDetailsWhenIncludeDeletedOrDeletePendingIsTrue()
    {
        var sut = filesContextFixture.Sut;

        var result = sut.FileDetails.IncludeDeletedOrDeletePending(true).ToList();

        result.Count.ShouldBe(3000);
    }
}
