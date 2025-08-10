using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using AStar.Dev.Test.DbContext.Helpers.Fixtures;
using Shouldly;

namespace AStar.Dev.Infrastructure.FilesDb.Tests.Integration;

public class FileDetailDirectoryNameExtensionsShould(FilesContextFixture filesContextFixture) : IClassFixture<FilesContextFixture>
{
    [Fact]
    public void ShouldReturnExpectedFileDetailsWhenDirectoryNameSpecifiedAndRecursionIsFalse()
    {
        var searchDirectory = new DirectoryName(@"\one-level\");

        var sut = filesContextFixture.Sut;

        var result = sut.FileDetails.WhereDirectoryNameMatches(searchDirectory.Value, false).ToList();

        result.Count.ShouldBe(693);
    }

    [Fact]
    public void ShouldReturnExpectedFileDetailsWhenDirectoryNameSpecifiedAndRecursionIsTrue()
    {
        var searchDirectory = new DirectoryName(@"\with");

        var sut = filesContextFixture.Sut;

        var result = sut.FileDetails.WhereDirectoryNameMatches(searchDirectory.Value, true).ToList();

        result.Count.ShouldBe(1621);
    }
}
