using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using AStar.Dev.Infrastructure.FilesDb.Tests.Unit.Fixtures;
using AStar.Dev.Utilities;

namespace AStar.Dev.Infrastructure.FilesDb.Tests.Unit.Data;

public sealed class FilesContextExtensionsShould(FilesContextFixture filesContextFixture) : IClassFixture<FilesContextFixture>
{
    private const bool Recursive = true;
    private const bool NotRecursive = false;
    private const bool IncludeSoftDeleted = true;
    private const bool ExcludeSoftDeleted = false;
    private const bool IncludeMarkedForDeletion = true;
    private const bool ExcludeMarkedForDeletion = false;
    private const bool ExcludeViewed = true;
    private const bool IncludeViewed = false;
    private readonly FilesContext _sut = filesContextFixture.Sut;

    [Fact]
    public void ReturnMatchingFilesWhenRecursiveIsTrueAndAllInclusionOrExclusionsAreSetToInclude()
    {
        var response = _sut.Files
            .GetMatchingFiles(new("/"), Recursive, "searchTypeNotRelevant", IncludeSoftDeleted, IncludeMarkedForDeletion, !ExcludeViewed, CancellationToken.None);

        response.ToJson().ShouldMatchApproved();
    }

    [Fact]
    public void ReturnMatchingFilesWhenRecursiveIsTrueAndIncludeSoftDeletedAndDeletePendingAreTrueAndViewedAreExcluded()
    {
        var response = _sut.Files
            .GetMatchingFiles(new("/"), Recursive, "searchTypeNotRelevant", IncludeSoftDeleted, IncludeMarkedForDeletion, ExcludeViewed, CancellationToken.None);

        response.ToJson().ShouldMatchApproved();
    }

    [Fact]
    public void ReturnMatchingFilesWhenRecursiveIsTrueAndIncludeSoftDeletedIsTrueButDeletePendingIsFalse()
    {
        var response = _sut.Files
            .GetMatchingFiles(new("/"), Recursive, "searchTypeNotRelevant", IncludeSoftDeleted, ExcludeMarkedForDeletion, ExcludeViewed, CancellationToken.None);

        response.ToJson().ShouldMatchApproved();
    }

    [Fact]
    public void ReturnMatchingFilesWhenRecursiveIsTrueAndIncludeSoftDeletedIsFalseButDeletePendingIsTrue()
    {
        var response = _sut.Files
            .GetMatchingFiles(new("/"), Recursive, "searchTypeNotRelevant", ExcludeSoftDeleted, IncludeMarkedForDeletion, ExcludeViewed, CancellationToken.None);

        response.ToJson().ShouldMatchApproved();
    }

    [Fact]
    public void ReturnMatchingFilesWhenRecursiveIsTrueAndIncludeSoftDeletedAndDeletePendingAreFalse()
    {
        var response = _sut.Files
            .GetMatchingFiles(new("/"), Recursive, "searchTypeNotRelevant", ExcludeSoftDeleted, ExcludeMarkedForDeletion, ExcludeViewed, CancellationToken.None);

        response.ToJson().ShouldMatchApproved();
    }

    [Fact]
    public void ReturnMatchingFilesWhenRecursiveIsTrueAndIncludeSoftDeletedAndDeletePendingAreTrue_ImagesOnly()
    {
        var response = _sut.Files
            .GetMatchingFiles(new("/"), Recursive, "Images", IncludeSoftDeleted, IncludeMarkedForDeletion, ExcludeViewed, CancellationToken.None);

        response.ToJson().ShouldMatchApproved();
    }

    [Fact]
    public void ReturnMatchingFilesWhenRecursiveIsTrueAndIncludeSoftDeletedIsTrueButDeletePendingIsFalse_ImagesOnly()
    {
        var response = _sut.Files
            .GetMatchingFiles(new("/"), Recursive, "Images", IncludeSoftDeleted, ExcludeMarkedForDeletion, ExcludeViewed, CancellationToken.None);

        response.ToJson().ShouldMatchApproved();
    }

    [Fact]
    public void ReturnMatchingFilesWhenRecursiveIsTrueAndIncludeSoftDeletedIsFalseButDeletePendingIsTrue_ImagesOnly()
    {
        var response = _sut.Files
            .GetMatchingFiles(new("/"), Recursive, "Images", ExcludeSoftDeleted, IncludeMarkedForDeletion, ExcludeViewed, CancellationToken.None);

        response.ToJson().ShouldMatchApproved();
    }

    [Fact]
    public void ReturnMatchingFilesWhenRecursiveIsTrueAndIncludeSoftDeletedAndDeletePendingAreFalse_ImagesOnly()
    {
        var response = _sut.Files
            .GetMatchingFiles(new("/"), Recursive, "Images", ExcludeSoftDeleted, ExcludeMarkedForDeletion, ExcludeViewed, CancellationToken.None);

        response.ToJson().ShouldMatchApproved();
    }

    [Fact]
    public void GetTheExpectedCountWhenFilterAppliedThatCapturesAllFiles()
    {
        const int filesNotSoftDeletedOrPendingDeletionCount = 10_000;

        var matchingFilesCount = _sut.Files
            .GetMatchingFiles(new("/"), Recursive, "AllFiles", IncludeSoftDeleted, IncludeMarkedForDeletion, IncludeViewed, CancellationToken.None)
            .Count;

        matchingFilesCount.ShouldBe(filesNotSoftDeletedOrPendingDeletionCount);
    }

    [Fact]
    public void GetTheExpectedCountWhenFilterAppliedThatCapturesAllImageFiles()
    {
        const int filesNotSoftDeletedOrPendingDeletionCount = 3955;

        var matchingFilesCount = _sut.Files
            .GetMatchingFiles(new("/"), Recursive, "Images",IncludeSoftDeleted, IncludeMarkedForDeletion, IncludeViewed, CancellationToken.None)
            .Count;

        matchingFilesCount.ShouldBe(filesNotSoftDeletedOrPendingDeletionCount);
    }

    [Fact]
    public void GetTheExpectedCountWhenFilterAppliedThatTargetsTopLevelFolderOnlyForAllFiles()
    {
        const int expectedCount = 180;
        
        var matchingFilesCount = _sut.Files
            .GetMatchingFiles(new("/home"), NotRecursive, "AllFiles", IncludeSoftDeleted, IncludeMarkedForDeletion, IncludeViewed, CancellationToken.None)
            .Count;

        matchingFilesCount.ShouldBe(expectedCount);
    }

    [Fact]
    public void GetTheExpectedCountWhenFilterAppliedThatTargetsTopLevelFolderOnlyAndImagesOnly()
    {
        const int expectedCount = 61;
        
        var matchingFilesCount = _sut.Files
            .GetMatchingFiles(new("/home"), NotRecursive, "Images", IncludeSoftDeleted, IncludeMarkedForDeletion, IncludeViewed, CancellationToken.None)
            .Count;

        matchingFilesCount.ShouldBe(expectedCount);
    }

    [Fact]
    public void GetTheExpectedCountWhenFilterAppliedThatTargetsSpecificFolderRecursively()
    {
        const int filesNotSoftDeletedOrPendingDeletionCount = 24;

        var matchingFilesCount = _sut.Files
            .GetMatchingFiles(new("/home"), Recursive, "Images", ExcludeSoftDeleted, ExcludeMarkedForDeletion, !ExcludeViewed, CancellationToken.None)
            .Count;

        matchingFilesCount.ShouldBe(filesNotSoftDeletedOrPendingDeletionCount);
    }

    [Fact]
    public void GetTheExpectedCountWhenFilterAppliedThatCapturesAllSupportedImageTypesFromStartingSubFolder()
    {
        const int filesNotSoftDeletedOrPendingDeletionCount = 17;

        var matchingFilesCount = _sut.Files
            .GetMatchingFiles(new("/home"), NotRecursive, "Images", ExcludeSoftDeleted, ExcludeMarkedForDeletion, !ExcludeViewed, CancellationToken.None)
            .Count;

        matchingFilesCount.ShouldBe(filesNotSoftDeletedOrPendingDeletionCount);
    }

    [Fact]
    public void GetTheExpectedCountWhenFilterAppliedThatTargetsSpecificFolderRecursivelyButIncludeSoftDeleted()
    {
        const int filesNotSoftDeletedOrPendingDeletionCount = 32;

        var matchingFilesCount = _sut.Files
            .GetMatchingFiles(new("/home"), Recursive, "Images", IncludeSoftDeleted, ExcludeMarkedForDeletion, !ExcludeViewed, CancellationToken.None)
            .Count;

        matchingFilesCount.ShouldBe(filesNotSoftDeletedOrPendingDeletionCount);
    }

    [Fact]
    public void GetTheExpectedCountWhenFilterAppliedThatTargetsSpecificFolderRecursivelyButIncludeMarkedForDeletion()
    {
        const int filesNotSoftDeletedOrPendingDeletionCount = 200;

        var matchingFilesCount = _sut.Files
            .GetMatchingFiles(new("/home"), Recursive, "Images", ExcludeSoftDeleted, IncludeMarkedForDeletion, !ExcludeViewed, CancellationToken.None)
            .Count;

        matchingFilesCount.ShouldBe(filesNotSoftDeletedOrPendingDeletionCount);
    }

    [Fact]
    public void
        GetTheExpectedCountWhenFilterAppliedThatTargetsSpecificFolderRecursivelyButIncludeSoftDeletedAndIncludeMarkedForDeletion()
    {
        const int filesNotSoftDeletedOrPendingDeletionCount = 202;

        var matchingFilesCount = _sut.Files
            .GetMatchingFiles(new("/home"), Recursive, "Images", IncludeSoftDeleted, IncludeMarkedForDeletion, !ExcludeViewed, CancellationToken.None)
            .Count;

        matchingFilesCount.ShouldBe(filesNotSoftDeletedOrPendingDeletionCount);
    }
}
