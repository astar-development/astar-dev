using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Tests.Unit.Fixtures;
using AStar.Dev.Utilities;
using Microsoft.Extensions.Time.Testing;

namespace AStar.Dev.Infrastructure.FilesDb.Tests.Unit.Data;

public sealed class FilesContextExtensionsApprovalsShould(FilesContextFixture filesContextFixture) : IClassFixture<FilesContextFixture>
{
    private const bool Recursive = true;
    private const bool IncludeSoftDeleted = true;
    private const bool ExcludeSoftDeleted = false;
    private const bool IncludeMarkedForDeletion = true;
    private const bool ExcludeMarkedForDeletion = false;
    private const bool ExcludeViewed = true;
    private readonly TimeProvider _fakeTime = new FakeTimeProvider(new DateTime(2025, 1, 2, 3, 4, 5, DateTimeKind.Utc));
    private readonly FilesContext _sut = filesContextFixture.Sut;

    [Fact]
    public void ReturnMatchingFilesWhenRecursiveIsTrueAndAllInclusionOrExclusionsAreSetToInclude()
    {
        var response = _sut.Files
            .GetMatchingFiles(new("/"), Recursive, "searchTypeNotRelevant", IncludeSoftDeleted, IncludeMarkedForDeletion, !ExcludeViewed, _fakeTime, CancellationToken.None);

        response.ToJson().ShouldMatchApproved();
    }

    [Fact]
    public void ReturnMatchingFilesWhenRecursiveIsTrueAndIncludeSoftDeletedAndDeletePendingAreTrueAndViewedAreExcluded()
    {
        var response = _sut.Files
            .GetMatchingFiles(new("/"), Recursive, "searchTypeNotRelevant", IncludeSoftDeleted, IncludeMarkedForDeletion, ExcludeViewed, _fakeTime, CancellationToken.None);

        response.ToJson().ShouldMatchApproved();
    }

    [Fact]
    public void ReturnMatchingFilesWhenRecursiveIsTrueAndIncludeSoftDeletedIsTrueButDeletePendingIsFalse()
    {
        var response = _sut.Files
            .GetMatchingFiles(new("/"), Recursive, "searchTypeNotRelevant", IncludeSoftDeleted, ExcludeMarkedForDeletion, ExcludeViewed, _fakeTime, CancellationToken.None);

        response.ToJson().ShouldMatchApproved();
    }

    [Fact]
    public void ReturnMatchingFilesWhenRecursiveIsTrueAndIncludeSoftDeletedIsFalseButDeletePendingIsTrue()
    {
        var response = _sut.Files
            .GetMatchingFiles(new("/"), Recursive, "searchTypeNotRelevant", ExcludeSoftDeleted, IncludeMarkedForDeletion, ExcludeViewed, _fakeTime, CancellationToken.None);

        response.ToJson().ShouldMatchApproved();
    }

    [Fact]
    public void ReturnMatchingFilesWhenRecursiveIsTrueAndIncludeSoftDeletedAndDeletePendingAreFalse()
    {
        var response = _sut.Files
            .GetMatchingFiles(new("/"), Recursive, "searchTypeNotRelevant", ExcludeSoftDeleted, ExcludeMarkedForDeletion, ExcludeViewed, _fakeTime, CancellationToken.None);

        response.ToJson().ShouldMatchApproved();
    }

    [Fact]
    public void ReturnMatchingFilesWhenRecursiveIsTrueAndIncludeSoftDeletedAndDeletePendingAreTrue_ImagesOnly()
    {
        var response = _sut.Files
            .GetMatchingFiles(new("/"), Recursive, "Images", IncludeSoftDeleted, IncludeMarkedForDeletion, ExcludeViewed, _fakeTime, CancellationToken.None);

        response.ToJson().ShouldMatchApproved();
    }

    [Fact]
    public void ReturnMatchingFilesWhenRecursiveIsTrueAndIncludeSoftDeletedIsTrueButDeletePendingIsFalse_ImagesOnly()
    {
        var response = _sut.Files
            .GetMatchingFiles(new("/"), Recursive, "Images", IncludeSoftDeleted, ExcludeMarkedForDeletion, ExcludeViewed, _fakeTime, CancellationToken.None);

        response.ToJson().ShouldMatchApproved();
    }

    [Fact]
    public void ReturnMatchingFilesWhenRecursiveIsTrueAndIncludeSoftDeletedIsFalseButDeletePendingIsTrue_ImagesOnly()
    {
        var response = _sut.Files
            .GetMatchingFiles(new("/"), Recursive, "Images", ExcludeSoftDeleted, IncludeMarkedForDeletion, ExcludeViewed, _fakeTime, CancellationToken.None);

        response.ToJson().ShouldMatchApproved();
    }

    [Fact]
    public void ReturnMatchingFilesWhenRecursiveIsTrueAndIncludeSoftDeletedAndDeletePendingAreFalse_ImagesOnly()
    {
        var response = _sut.Files
            .GetMatchingFiles(new("/"), Recursive, "Images", ExcludeSoftDeleted, ExcludeMarkedForDeletion, ExcludeViewed, _fakeTime, CancellationToken.None);

        response.ToJson().ShouldMatchApproved();
    }
}
