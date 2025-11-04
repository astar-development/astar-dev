using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using AStar.Dev.Infrastructure.FilesDb.Tests.Unit.Fixtures;
using Microsoft.Extensions.Time.Testing;

namespace AStar.Dev.Infrastructure.FilesDb.Tests.Unit.Data;

public sealed class FilesContextExtensionsShould(FilesContextFixture filesContextFixture) : IClassFixture<FilesContextFixture>
{
    private readonly FilesContext _sut = filesContextFixture.Sut;

    [Theory]
    [InlineData("/home", false, "AllFiles", false, false, 0, 227)]
    [InlineData("/home", false, "AllFiles", false, false, 7, 227)]
    [InlineData("/home", false, "AllFiles", false, true, 0, 234)]
    [InlineData("/home", false, "AllFiles", false, true, 7, 234)]
    [InlineData("/home", false, "AllFiles", true, false, 0, 228)]
    [InlineData("/home", false, "AllFiles", true, false, 7, 228)]
    [InlineData("/home", false, "AllFiles", true, true, 0, 235)]
    [InlineData("/home", false, "AllFiles", true, true, 7, 235)]
    [InlineData("/home", false, "Images", false, false, 0, 94)]
    [InlineData("/home", false, "Images", false, false, 7, 94)]
    [InlineData("/home", false, "Images", false, true, 0, 93)]
    [InlineData("/home", false, "Images", false, true, 7, 93)]
    [InlineData("/home", false, "Images", true, false, 0, 93)]
    [InlineData("/home", false, "Images", true, false, 7, 93)]
    [InlineData("/home", false, "Images", true, true, 0, 94)]
    [InlineData("/home", false, "Images", true, true, 7, 94)]
    [InlineData("/home", true, "AllFiles", false, false, 0, 706)]
    [InlineData("/home", true, "AllFiles", false, false, 7, 706)]
    [InlineData("/home", true, "AllFiles", false, true, 0, 729)]
    [InlineData("/home", true, "AllFiles", false, true, 7, 729)]
    [InlineData("/home", true, "AllFiles", true, false, 0, 719)]
    [InlineData("/home", true, "AllFiles", true, false, 7, 719)]
    [InlineData("/home", true, "AllFiles", true, true, 0, 742)]
    [InlineData("/home", true, "AllFiles", true, true, 7, 742)]
    [InlineData("/home", true, "Images", false, false, 0, 271)]
    [InlineData("/home", true, "Images", false, false, 7, 271)]
    [InlineData("/home", true, "Images", false, true, 0, 276)]
    [InlineData("/home", true, "Images", false, true, 7, 276)]
    [InlineData("/home", true, "Images", true, false, 0, 275)]
    [InlineData("/home", true, "Images", true, false, 7, 275)]
    [InlineData("/home", true, "Images", true, true, 0, 280)]
    [InlineData("/home", true, "Images", true, true, 7, 280)]
    public void GetTheExpectedCountWhenSpecificFilterApplied(string startingDirectory, bool recursive, string searchType, bool includeSoftDeleted, bool includeMarkedForDeletion,
        int excludeViewedWithin,
        int expectedCount)
    {
        var fakeTime = new FakeTimeProvider(new DateTime(2025, 1, 2, 3, 4, 5, DateTimeKind.Utc));

        var matchingFilesCount = _sut.Files
            .GetMatchingFiles(new DirectoryName(startingDirectory), recursive, searchType, includeSoftDeleted, includeMarkedForDeletion, excludeViewedWithin, fakeTime, CancellationToken.None)
            .Count;

        matchingFilesCount.ShouldBe(expectedCount);
    }

    [Theory]
    [InlineData("/home", false, "AllFiles", false, true, 0, 247)]
    [InlineData("/home", false, "AllFiles", false, true, 7, 136)]
    [InlineData("/home", false, "AllFiles", true, true, 0, 249)]
    [InlineData("/home", false, "AllFiles", true, true, 7, 136)]
    [InlineData("/home", false, "Images", false, true, 0, 99)]
    [InlineData("/home", false, "Images", false, true, 7, 52)]
    [InlineData("/home", false, "Images", true, true, 0, 100)]
    [InlineData("/home", false, "Images", true, true, 7, 52)]
    [InlineData("/home", true, "AllFiles", false, true, 0, 718)]
    [InlineData("/home", true, "AllFiles", false, true, 7, 380)]
    [InlineData("/home", true, "AllFiles", true, true, 0, 730)]
    [InlineData("/home", true, "AllFiles", true, true, 7, 384)]
    [InlineData("/home", true, "Images", false, true, 0, 301)]
    [InlineData("/home", true, "Images", false, true, 7, 155)]
    [InlineData("/home", true, "Images", true, true, 0, 308)]
    [InlineData("/home", true, "Images", true, true, 7, 156)]
    public void GetTheExpectedCountWhenSpecificFilterAppliedWithActualValues(string startingDirectory, bool recursive, string searchType, bool includeSoftDeleted, bool includeMarkedForDeletion,
        int excludeViewedWithin,
        int expectedCount)
    {
        var fakeTime = new FakeTimeProvider(new DateTime(2025, 1, 2, 3, 4, 5, DateTimeKind.Utc));

        var matchingFilesCount = _sut.Files
            .GetMatchingFiles(new DirectoryName(startingDirectory), recursive, searchType, includeSoftDeleted, includeMarkedForDeletion, excludeViewedWithin, fakeTime, CancellationToken.None)
            .Count;

        matchingFilesCount.ShouldBe(expectedCount);
    }
}
