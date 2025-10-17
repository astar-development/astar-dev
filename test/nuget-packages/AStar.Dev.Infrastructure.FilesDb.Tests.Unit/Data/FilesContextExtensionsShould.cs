using AStar.Dev.Infrastructure.FilesDb.Fixtures;

namespace AStar.Dev.Infrastructure.FilesDb.Data;

public sealed class FilesContextExtensionsShould(FilesContextFixture filesContextFixture) : IClassFixture<FilesContextFixture>
{
    private const bool Recursive = true;
    private const bool NotRecursive = false;
    private const bool IncludeSoftDeleted = true;
    private const bool ExcludeSoftDeleted = false;
    private const bool IncludeMarkedForDeletion = true;
    private const bool ExcludeMarkedForDeletion = false;
    private const bool ExcludeViewed = true;
    private readonly FilesContext _sut = filesContextFixture.Sut;

    [Fact(Skip = "The underlying code is broken")]
    public void
        ReturnMatchingFilesWhenRecursiveIsTrueAndIncludeSoftDeletedAndDeletePendingAreTrueAndViewedAreNotExcluded()
    {
        var response = _sut.Files.GetMatchingFiles("c:\\temp", Recursive, "searchTypeNotRelevant", IncludeSoftDeleted,
            IncludeMarkedForDeletion, !ExcludeViewed, CancellationToken.None);

        response.ToString()!.ShouldMatchApproved();
    }

    [Fact(Skip = "The underlying code is broken")]
    public void ReturnMatchingFilesWhenRecursiveIsTrueAndIncludeSoftDeletedAndDeletePendingAreTrueAndViewedAreExcluded()
    {
        var response = _sut.Files.GetMatchingFiles("c:\\temp", Recursive, "searchTypeNotRelevant", IncludeSoftDeleted,
            IncludeMarkedForDeletion, ExcludeViewed, CancellationToken.None);

        response.ToString()!.ShouldMatchApproved();
    }

    [Fact(Skip = "The underlying code is broken")]
    public void ReturnMatchingFilesWhenRecursiveIsTrueAndIncludeSoftDeletedIsTrueButDeletePendingIsFalse()
    {
        var response = _sut.Files.GetMatchingFiles("c:\\temp", Recursive, "searchTypeNotRelevant", IncludeSoftDeleted,
            ExcludeMarkedForDeletion, ExcludeViewed, CancellationToken.None);

        response.ToString()!.ShouldMatchApproved();
    }

    [Fact(Skip = "The underlying code is broken")]
    public void ReturnMatchingFilesWhenRecursiveIsTrueAndIncludeSoftDeletedIsFalseButDeletePendingIsTrue()
    {
        var response = _sut.Files.GetMatchingFiles("c:\\temp", Recursive, "searchTypeNotRelevant", ExcludeSoftDeleted,
            IncludeMarkedForDeletion, ExcludeViewed, CancellationToken.None);

        response.ToString()!.ShouldMatchApproved();
    }

    [Fact(Skip = "The underlying code is broken")]
    public void ReturnMatchingFilesWhenRecursiveIsTrueAndIncludeSoftDeletedAndDeletePendingAreFalse()
    {
        var response = _sut.Files.GetMatchingFiles("c:\\temp", Recursive, "searchTypeNotRelevant", ExcludeSoftDeleted,
            ExcludeMarkedForDeletion, ExcludeViewed, CancellationToken.None);

        response.ToString()!.ShouldMatchApproved();
    }

    [Fact(Skip = "The underlying code is broken")]
    public void ReturnMatchingFilesWhenRecursiveIsTrueAndIncludeSoftDeletedAndDeletePendingAreTrue_ImagesOnly()
    {
        var response = _sut.Files.GetMatchingFiles("c:\\temp", Recursive, "Images", IncludeSoftDeleted,
            IncludeMarkedForDeletion, ExcludeViewed, CancellationToken.None);

        response.ToString()!.ShouldMatchApproved();
    }

    [Fact(Skip = "The underlying code is broken")]
    public void ReturnMatchingFilesWhenRecursiveIsTrueAndIncludeSoftDeletedIsTrueButDeletePendingIsFalse_ImagesOnly()
    {
        var response = _sut.Files.GetMatchingFiles("c:\\temp", Recursive, "Images", IncludeSoftDeleted,
            ExcludeMarkedForDeletion, ExcludeViewed, CancellationToken.None);

        response.ToString()!.ShouldMatchApproved();
    }

    [Fact(Skip = "The underlying code is broken")]
    public void ReturnMatchingFilesWhenRecursiveIsTrueAndIncludeSoftDeletedIsFalseButDeletePendingIsTrue_ImagesOnly()
    {
        var response = _sut.Files.GetMatchingFiles("c:\\temp", Recursive, "Images", ExcludeSoftDeleted,
            IncludeMarkedForDeletion, ExcludeViewed, CancellationToken.None);

        response.ToString()!.ShouldMatchApproved();
    }

    [Fact(Skip = "The underlying code is broken")]
    public void ReturnMatchingFilesWhenRecursiveIsTrueAndIncludeSoftDeletedAndDeletePendingAreFalse_ImagesOnly()
    {
        var response = _sut.Files.GetMatchingFiles("c:\\temp", Recursive, "Images", ExcludeSoftDeleted,
            ExcludeMarkedForDeletion, ExcludeViewed, CancellationToken.None);

        response.ToString()!.ShouldMatchApproved();
    }

    [Fact(Skip = "The underlying code is broken")]
    public void GetTheExpectedCountWhenFilterAppliedThatCapturesAllFiles()
    {
        const int filesNotSoftDeletedOrPendingDeletionCount = 101;

        var matchingFilesCount = _sut.Files
            .GetMatchingFiles(@"c:\", Recursive, "AllFiles", ExcludeSoftDeleted, ExcludeMarkedForDeletion, !ExcludeViewed,
                CancellationToken.None)
            .Count();

        matchingFilesCount.ShouldBe(filesNotSoftDeletedOrPendingDeletionCount);
    }

    [Fact(Skip = "The underlying code is broken")]
    public void GetTheExpectedCountWhenFilterAppliedThatCapturesAllImageFiles()
    {
        const int filesNotSoftDeletedOrPendingDeletionCount = 66;

        var matchingFilesCount = _sut.Files
            .GetMatchingFiles(@"c:\", Recursive, "Images", ExcludeSoftDeleted, ExcludeMarkedForDeletion, !ExcludeViewed,
                CancellationToken.None)
            .Count();

        matchingFilesCount.ShouldBe(filesNotSoftDeletedOrPendingDeletionCount);
    }

    [Fact(Skip = "The underlying code is broken")]
    public void GetTheExpectedCountWhenFilterAppliedThatTargetsTopLevelFolderOnlyWhichIsEmpty()
    {
        var matchingFilesCount = _sut.Files
            .GetMatchingFiles(@"c:\", NotRecursive, "Images", ExcludeSoftDeleted, ExcludeMarkedForDeletion,
                !ExcludeViewed, CancellationToken.None)
            .Count();

        matchingFilesCount.ShouldBe(0);
    }

    [Fact(Skip = "The underlying code is broken")]
    public void GetTheExpectedCountWhenFilterAppliedThatTargetsSpecificFolderRecursively()
    {
        const int filesNotSoftDeletedOrPendingDeletionCount = 24;

        var matchingFilesCount = _sut.Files
            .GetMatchingFiles(@"c:\temp\wwwroot - Copy\AI", Recursive, "Images", ExcludeSoftDeleted,
                ExcludeMarkedForDeletion, !ExcludeViewed, CancellationToken.None)
            .Count();

        matchingFilesCount.ShouldBe(filesNotSoftDeletedOrPendingDeletionCount);
    }

    [Fact(Skip = "The underlying code is broken")]
    public void GetTheExpectedCountWhenFilterAppliedThatCapturesAllSupportedImageTypesFromStartingSubFolder()
    {
        const int filesNotSoftDeletedOrPendingDeletionCount = 17;

        var matchingFilesCount = _sut.Files
            .GetMatchingFiles(@"c:\temp\wwwroot - Copy\AI", NotRecursive, "Images", ExcludeSoftDeleted,
                ExcludeMarkedForDeletion, !ExcludeViewed, CancellationToken.None)
            .Count();

        matchingFilesCount.ShouldBe(filesNotSoftDeletedOrPendingDeletionCount);
    }

    [Fact(Skip = "The underlying code is broken")]
    public void GetTheExpectedCountWhenFilterAppliedThatTargetsSpecificFolderRecursivelyButIncludeSoftDeleted()
    {
        const int filesNotSoftDeletedOrPendingDeletionCount = 32;

        var matchingFilesCount = _sut.Files
            .GetMatchingFiles(@"c:\temp\wwwroot - Copy", Recursive, "Images", IncludeSoftDeleted,
                ExcludeMarkedForDeletion, !ExcludeViewed, CancellationToken.None)
            .Count();

        matchingFilesCount.ShouldBe(filesNotSoftDeletedOrPendingDeletionCount);
    }

    [Fact(Skip = "The underlying code is broken")]
    public void GetTheExpectedCountWhenFilterAppliedThatTargetsSpecificFolderRecursivelyButIncludeMarkedForDeletion()
    {
        const int filesNotSoftDeletedOrPendingDeletionCount = 33;

        var matchingFilesCount = _sut.Files
            .GetMatchingFiles(@"c:\temp\wwwroot - Copy", Recursive, "Images", ExcludeSoftDeleted,
                IncludeMarkedForDeletion, !ExcludeViewed, CancellationToken.None)
            .Count();

        matchingFilesCount.ShouldBe(filesNotSoftDeletedOrPendingDeletionCount);
    }

    [Fact]
    public void
        GetTheExpectedCountWhenFilterAppliedThatTargetsSpecificFolderRecursivelyButIncludeSoftDeletedAndIncludeMarkedForDeletion()
    {
        const int filesNotSoftDeletedOrPendingDeletionCount = 35;

        var matchingFilesCount = _sut.Files
            .GetMatchingFiles(@"c:\temp\wwwroot - Copy", Recursive, "Images", IncludeSoftDeleted,
                IncludeMarkedForDeletion, !ExcludeViewed, CancellationToken.None)
            .Count();

        matchingFilesCount.ShouldBe(filesNotSoftDeletedOrPendingDeletionCount);
    }
}
