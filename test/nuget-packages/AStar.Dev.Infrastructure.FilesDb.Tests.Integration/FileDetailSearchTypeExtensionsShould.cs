using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using AStar.Dev.Test.DbContext.Helpers.Fixtures;
using Shouldly;

namespace AStar.Dev.Infrastructure.FilesDb.Tests.Integration;

/// <summary>
/// </summary>
public class FileDetailSearchTypeExtensionsShould (FilesContextFixture filesContextFixture) : IClassFixture<FilesContextFixture>
{
    [Fact]
    public void ShouldReturnExpectedFileDetailsWhenSearchTypeIsSetToAll()
    {
        var sut = filesContextFixture.Sut;

        var result = sut.FileDetails.OfSearchType(SearchType.All).ToList();

        result.Count.ShouldBe(3000);
        result[0].FileName.Value.ShouldBe("file.txt");
        result[0].FileSize.ShouldBe(91801L);
    }

    [Fact]
    public void ShouldReturnExpectedFileDetailsWhenSearchTypeIsSetToImages()
    {
        var sut = filesContextFixture.Sut;

        var result = sut.FileDetails.OfSearchType(SearchType.Images).ToList();

        result.Count.ShouldBe(2314);
        result[0].FileName.Value.ShouldBe(@"\some\file.gif");
        result[0].FileSize.ShouldBe(22081L);
    }

    [Fact]
    public void ShouldReturnExpectedFileDetailsWhenSearchTypeIsSetToDuplicateImages()
    {
        var sut = filesContextFixture.Sut;

        var result = sut.FileDetails.OfSearchType(SearchType.DuplicateImages).ToList();

        result.Count.ShouldBe(1546);
        result[0].FileName.Value.ShouldBe(@"\some\file.bmp");
        result[0].FileSize.ShouldBe(20033L);
    }

    [Fact]
    public void ShouldReturnExpectedFileDetailsWhenSearchTypeIsSetToDuplicates()
    {
        var sut = filesContextFixture.Sut;

        var result = sut.FileDetails.OfSearchType(SearchType.Duplicates).ToList();

        result.Count.ShouldBe(2013);
        result[0].FileName.Value.ShouldBe(@"\some\file.txt");
        result[0].FileSize.ShouldBe(45197L);
    }
}
