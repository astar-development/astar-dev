using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Test.DbContext.Helpers.Fixtures;
using Microsoft.Extensions.Time.Testing;
using Shouldly;

namespace AStar.Dev.Infrastructure.FilesDb.Tests.Integration;

public class FilesContextLastViewedExtensionsShould : IClassFixture<FilesContextFixture>
{
    private readonly FilesContextFixture filesContextFixture;
    private readonly FakeTimeProvider    mockTimeProvider = new();

    public FilesContextLastViewedExtensionsShould(FilesContextFixture filesContextFixture)
    {
        this.filesContextFixture = filesContextFixture;
        mockTimeProvider.SetUtcNow(new DateTime(2025, 07, 21, 0, 0, 0, DateTimeKind.Utc));
    }

    [Fact]
    public void ShouldReturnExpectedFileDetailsWhereLastViewedIsSetTo7Days()
    {
        var sut = filesContextFixture.Sut;

        var result = sut.FileDetails.WhereLastViewedIsOlderThan(7, mockTimeProvider).ToList();

        result.Count.ShouldBe(1524);
    }

    [Fact]
    public void ShouldReturnExpectedFileDetailsWhereLastViewedIsSetTo0Days()
    {
        var sut = filesContextFixture.Sut;

        var result = sut.FileDetails.WhereLastViewedIsOlderThan(0, mockTimeProvider).ToList();

        result.Count.ShouldBe(3000);
    }
}
