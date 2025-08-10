using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Test.DbContext.Helpers.Fixtures;
using Shouldly;
using SortOrder = AStar.Dev.Infrastructure.FilesDb.Models.SortOrder;

namespace AStar.Dev.Infrastructure.FilesDb.Tests.Integration;

/// <summary>
/// </summary>
public class FileDetailOrderingExtensionsShould (FilesContextFixture filesContextFixture) : IClassFixture<FilesContextFixture>
{
    [Fact]
    public void ShouldReturnExpectedFileDetailsOrderedByNameAscending()
    {
        var sut = filesContextFixture.Sut;

        var result = sut.FileDetails.OrderResultsBy(SortOrder.NameAscending).ToList();

        result.Count.ShouldBe(3000);
        result[0].FileName.Value.ShouldBe(@"\some\file.bmp");
        result[0].FileSize.ShouldBe(91451L);
    }

    [Fact]
    public void ShouldReturnExpectedFileDetailsOrderedByNameDescending()
    {
        var sut = filesContextFixture.Sut;

        var result = sut.FileDetails.OrderResultsBy(SortOrder.NameDescending).ToList();

        result.Count.ShouldBe(3000);
        result[0].FileName.Value.ShouldBe(@"file.txt");
        result[0].FileSize.ShouldBe(91801L);
    }

    [Fact]
    public void ShouldReturnExpectedFileDetailsOrderedBySizeAscending()
    {
        var sut = filesContextFixture.Sut;

        var result = sut.FileDetails.OrderResultsBy(SortOrder.SizeAscending).ToList();

        result.Count.ShouldBe(3000);
        result[0].FileName.Value.ShouldBe(@"\some\file.png");
        result[0].FileSize.ShouldBe(20024L);
    }

    [Fact]
    public void ShouldReturnExpectedFileDetailsOrderedBySizeDescending()
    {
        var sut = filesContextFixture.Sut;

        var result = sut.FileDetails.OrderResultsBy(SortOrder.SizeDescending).ToList();

        result.Count.ShouldBe(3000);
        result[0].FileName.Value.ShouldBe(@"\some\file.txt");
        result[0].FileSize.ShouldBe(99889L);
    }
}
