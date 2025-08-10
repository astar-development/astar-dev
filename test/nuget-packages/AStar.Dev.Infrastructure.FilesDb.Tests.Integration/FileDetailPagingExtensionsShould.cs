using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Test.DbContext.Helpers.Fixtures;
using Shouldly;

namespace AStar.Dev.Infrastructure.FilesDb.Tests.Integration;

/// <summary>
/// </summary>
public class FileDetailPagingExtensionsShould (FilesContextFixture filesContextFixture) : IClassFixture<FilesContextFixture>
{
    [Fact]
    public void ShouldReturnExpectedFirstPageOfTenResultsWithCorrectDetails()
    {
        var sut = filesContextFixture.Sut;

        var result = sut.FileDetails.GetPage(1, 10).ToList();

        result.Count.ShouldBe(10);
        result[0].FileName.Value.ShouldBe(@"\some\file.png");
        result[0].FileSize.ShouldBe(58887L);
    }

    [Fact]
    public void ShouldReturnExpectedThirdPageOfTwentyResultsWithCorrectDetails()
    {
        var sut = filesContextFixture.Sut;

        var result = sut.FileDetails.GetPage(28, 20).ToList();

        result.Count.ShouldBe(20);
        result[0].FileName.Value.ShouldBe(@"\some\file.jpg");
        result[0].FileSize.ShouldBe(20676L);
    }

    [Fact]
    public void ShouldReturnFirstPageOfResultsWhenPageRequestedIsZero()
    {
        var sut = filesContextFixture.Sut;

        var result = sut.FileDetails.GetPage(0, 20).ToList();

        result.Count.ShouldBe(20);
        result[0].FileName.Value.ShouldBe(@"\some\file.png");
        result[0].FileSize.ShouldBe(99512L);
    }

    [Fact]
    public void ShouldReturnExpectedFiftyResultsWhenMaxItemsIsExceeded()
    {
        var sut = filesContextFixture.Sut;

        var result = sut.FileDetails.GetPage(1, 51).ToList();

        result.Count.ShouldBe(50);
    }
}
