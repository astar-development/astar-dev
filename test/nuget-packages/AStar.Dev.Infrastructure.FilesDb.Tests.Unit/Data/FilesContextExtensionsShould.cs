using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Tests.Unit.Fixtures;
using Microsoft.Extensions.Time.Testing;

namespace AStar.Dev.Infrastructure.FilesDb.Tests.Unit.Data;

public sealed class FilesContextExtensionsShould(FilesContextFixture filesContextFixture) : IClassFixture<FilesContextFixture>
{
    private readonly FilesContext _sut = filesContextFixture.Sut;

    [Theory]
    [InlineData("/home", false, "AllFiles", false, false, false, 227)]
    [InlineData("/home", false, "AllFiles", false, false, true, 227)]
    [InlineData("/home", false, "AllFiles", false, true, false, 234)]
    [InlineData("/home", false, "AllFiles", false, true, true, 234)]
    [InlineData("/home", false, "AllFiles", true, false, false, 228)]
    [InlineData("/home", false, "AllFiles", true, false, true, 228)]
    [InlineData("/home", false, "AllFiles", true, true, false, 235)]
    [InlineData("/home", false, "AllFiles", true, true, true, 235)]
    [InlineData("/home", false, "Images", false, false, false, 94)]
    [InlineData("/home", false, "Images", false, false, true, 94)]
    [InlineData("/home", false, "Images", false, true, false, 93)]
    [InlineData("/home", false, "Images", false, true, true, 93)]
    [InlineData("/home", false, "Images", true, false, false, 93)]
    [InlineData("/home", false, "Images", true, false, true, 93)]
    [InlineData("/home", false, "Images", true, true, false, 94)]
    [InlineData("/home", false, "Images", true, true, true, 94)]
    [InlineData("/home", true, "AllFiles", false, false, false, 706)]
    [InlineData("/home", true, "AllFiles", false, false, true, 706)]
    [InlineData("/home", true, "AllFiles", false, true, false, 729)]
    [InlineData("/home", true, "AllFiles", false, true, true, 729)]
    [InlineData("/home", true, "AllFiles", true, false, false, 719)]
    [InlineData("/home", true, "AllFiles", true, false, true, 719)]
    [InlineData("/home", true, "AllFiles", true, true, false, 742)]
    [InlineData("/home", true, "AllFiles", true, true, true, 742)]
    [InlineData("/home", true, "Images", false, false, false, 271)]
    [InlineData("/home", true, "Images", false, false, true, 271)]
    [InlineData("/home", true, "Images", false, true, false, 276)]
    [InlineData("/home", true, "Images", false, true, true, 276)]
    [InlineData("/home", true, "Images", true, false, false, 275)]
    [InlineData("/home", true, "Images", true, false, true, 275)]
    [InlineData("/home", true, "Images", true, true, false, 280)]
    [InlineData("/home", true, "Images", true, true, true, 280)]
    public void GetTheExpectedCountWhenSpecificFilterApplied(string startingDirectory, bool recursive, string searchType, bool includeSoftDeleted, bool includeMarkedForDeletion, bool includeViewed,
        int expectedCount)
    {
        var fakeTime = new FakeTimeProvider(new DateTime(2025, 1, 2, 3, 4, 5, DateTimeKind.Utc));

        var matchingFilesCount = _sut.Files
            .GetMatchingFiles(new(startingDirectory), recursive, searchType, includeSoftDeleted, includeMarkedForDeletion, includeViewed, fakeTime, CancellationToken.None)
            .Count;

        matchingFilesCount.ShouldBe(expectedCount);
    }
}
