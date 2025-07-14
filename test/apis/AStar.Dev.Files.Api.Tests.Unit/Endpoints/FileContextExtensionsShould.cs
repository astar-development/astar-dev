using AStar.Dev.Files.Api.Endpoints.Get.V1;
using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Files.Api.Endpoints;

public class FileContextExtensionsShould
{
    [Fact]
    public void SetTheDirectoryNameAndRecursionWhenRecursionIsTrue()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.FileDetails.WhereDirectoryNameMatches("MockDirectoryName", true);

        response.Expression
                .ToString()
                .ShouldStartWith("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression].Where(fileDetail => fileDetail.DirectoryName.StartsWith(");

        response.Expression
                .ToString()
                .ShouldEndWith(").directoryName))");
    }

    [Fact]
    public void SetTheDirectoryNameAndRecursionWhenRecursionIsFalse()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.FileDetails.WhereDirectoryNameMatches("MockDirectoryName", false);

        response.Expression
                .ToString()
                .ShouldStartWith("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression].Where(fileDetail => (fileDetail.DirectoryName ==");

        response.Expression
                .ToString()
                .ShouldEndWith(").directoryName))");
    }

    [Fact]
    public void SetTheDirectoryNameAndRecursionWhenRecursionIsTrueForDuplicateDetail()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.DuplicateDetails.WhereDirectoryNameMatches("MockDirectoryName", true);

        response.Expression
                .ToString()
                .ShouldStartWith("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression].Where(fileDetail => fileDetail.DirectoryName.StartsWith(");

        response.Expression
                .ToString()
                .ShouldEndWith(").directoryName))");
    }

    [Fact]
    public void SetTheDirectoryNameAndRecursionWhenRecursionIsFalseForDuplicateDetail()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.DuplicateDetails.WhereDirectoryNameMatches("MockDirectoryName", false);

        response.Expression
                .ToString()
                .ShouldStartWith("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression].Where(fileDetail => (fileDetail.DirectoryName ==");

        response.Expression
                .ToString()
                .ShouldEndWith(").directoryName))");
    }

    [Fact]
    public void IncludeViewedWhenExcludeViewedIsZero()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.FileDetails.ExcludeViewed(0);

        response.Expression
                .ToString()
                .ShouldBe("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression]");
    }

    [Fact]
    public void NotIncludeViewedWhenExcludeViewedIsNotZero()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.FileDetails.ExcludeViewed(7);

        response.Expression
                .ToString()
                .ShouldBe("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression].Where(fileDetail => (fileDetail.FileLastViewed < Convert(DateTimeOffset.UtcNow.AddDays(Convert(-value(AStar.Dev.Files.Api.Endpoints.FileContextExtensions+<>c__DisplayClass2_0).excludeViewedWithinDays, Double)), Nullable`1)))");
    }

    [Fact]
    public void IncludeViewedWhenExcludeViewedIsZeroForTheDuplicates()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.DuplicateDetails.ExcludeViewed(0);

        response.Expression
                .ToString()
                .ShouldBe("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression]");
    }

    [Fact]
    public void NotIncludeViewedWhenExcludeViewedIsNotZeroForTheDuplicates()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.DuplicateDetails.ExcludeViewed(7);

        response.Expression
                .ToString()
                .ShouldBe("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression].Where(fileDetail => (fileDetail.FileLastViewed < Convert(DateTimeOffset.UtcNow.AddDays(Convert(-value(AStar.Dev.Files.Api.Endpoints.FileContextExtensions+<>c__DisplayClass3_0).excludeViewedWithinDays, Double)), Nullable`1)))");
    }

    [Fact]
    public void IncludeMarkedForDeletionWhenRequested()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.FileDetails.IncludeMarkedForDeletion(true);

        response.Expression
                .ToString()
                .ShouldBe("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression]");
    }

    [Fact]
    public void NotIncludeMarkedForDeletionWhenNotRequested()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.FileDetails.IncludeMarkedForDeletion(false);

        response.Expression
                .ToString()
                .ShouldBe("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression].Where(fileDetail => (((fileDetail.DeletionStatus.HardDeletePending == null) AndAlso (fileDetail.DeletionStatus.SoftDeleted == null)) AndAlso (fileDetail.DeletionStatus.SoftDeletePending == null)))");
    }

    [Fact]
    public void IncludeMarkedForDeletionWhenRequestedForDuplicates()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.DuplicateDetails.IncludeMarkedForDeletion(true);

        response.Expression
                .ToString()
                .ShouldBe("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression]");
    }

    [Fact]
    public void NotIncludeMarkedForDeletionWhenNotRequestedForDuplicates()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.DuplicateDetails.IncludeMarkedForDeletion(false);

        response.Expression
                .ToString()
                .ShouldBe("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression].Where(fileDetail => (((fileDetail.HardDeletePending == null) AndAlso (fileDetail.SoftDeleted == null)) AndAlso (fileDetail.SoftDeletePending == null)))");
    }

    [Fact]
    public void NotSetTheSearchTextWhenNotSupplied()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.FileDetails.SelectFilesMatching(null);

        response.Expression
                .ToString()
                .ShouldBe("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression]");
    }

    [Fact]
    public void SetTheSearchTextWhenSupplied()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.FileDetails.SelectFilesMatching("Mock Text");

        response.Expression
                .ToString()
                .ShouldBe("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression].Where(fileDetail => (fileDetail.DirectoryName.Contains(value(AStar.Dev.Files.Api.Endpoints.FileContextExtensions+<>c__DisplayClass6_0).searchText) OrElse fileDetail.FileName.Contains(value(AStar.Dev.Files.Api.Endpoints.FileContextExtensions+<>c__DisplayClass6_0).searchText)))");
    }

    [Fact]
    public void NotSetTheSearchTextWhenNotSuppliedForDuplicates()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.DuplicateDetails.SelectFilesMatching(null);

        response.Expression
                .ToString()
                .ShouldBe("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression]");
    }

    [Fact]
    public void SetTheSearchTextWhenSuppliedForDuplicates()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.DuplicateDetails.SelectFilesMatching("Mock Text");

        response.Expression
                .ToString()
                .ShouldBe("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression].Where(fileDetail => (fileDetail.DirectoryName.Contains(value(AStar.Dev.Files.Api.Endpoints.FileContextExtensions+<>c__DisplayClass7_0).searchText) OrElse fileDetail.FileName.Contains(value(AStar.Dev.Files.Api.Endpoints.FileContextExtensions+<>c__DisplayClass7_0).searchText)))");
    }

    [Fact]
    public void SelectTheFirstPageWhenRequested()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.FileDetails.SelectRequestedPage(1, 10);

        response.Expression
                .ToString()
                .ShouldBe("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression].Skip(0).Take(10)");
    }

    [Fact]
    public void SelectTheNthPageWhenRequested()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.FileDetails.SelectRequestedPage(3, 10);

        response.Expression
                .ToString()
                .ShouldBe("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression].Skip(20).Take(10)");
    }

    [Fact]
    public void SetTheSortOrderAsNameAscendingWhenRequested()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.FileDetails.OrderAsRequested(SortOrder.NameAscending);

        response.Expression
                .ToString()
                .ShouldBe("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression].OrderBy(fileDetail => fileDetail.FileName)");
    }

    [Fact]
    public void SetTheSortOrderAsNameDescendingWhenRequested()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.FileDetails.OrderAsRequested(SortOrder.NameDescending);

        response.Expression
                .ToString()
                .ShouldBe("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression].OrderByDescending(fileDetail => fileDetail.FileName)");
    }

    [Fact]
    public void SetTheSortOrderAsSizeAscendingWhenRequested()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.FileDetails.OrderAsRequested(SortOrder.SizeAscending);

        response.Expression
                .ToString()
                .ShouldBe("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression].OrderBy(fileDetail => fileDetail.FileSize)");
    }

    [Fact]
    public void SetTheSortOrderAsSizeDescendingWhenRequested()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.FileDetails.OrderAsRequested(SortOrder.SizeDescending);

        response.Expression
                .ToString()
                .ShouldBe("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression].OrderByDescending(fileDetail => fileDetail.FileSize)");
    }

    [Fact]
    public void SetTheSortOrderAsNameAscendingWhenRequestedForDuplicates()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.DuplicateDetails.OrderAsRequested(SortOrder.NameAscending);

        response.Expression
                .ToString()
                .ShouldBe("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression].OrderBy(fileDetail => fileDetail.FileName)");
    }

    [Fact]
    public void SetTheSortOrderAsNameDescendingWhenRequestedForDuplicates()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.DuplicateDetails.OrderAsRequested(SortOrder.NameDescending);

        response.Expression
                .ToString()
                .ShouldBe("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression].OrderByDescending(fileDetail => fileDetail.FileName)");
    }

    [Fact]
    public void SetTheSortOrderAsSizeAscendingWhenRequestedForDuplicates()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.DuplicateDetails.OrderAsRequested(SortOrder.SizeAscending);

        response.Expression
                .ToString()
                .ShouldBe("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression].OrderBy(fileDetail => fileDetail.FileSize)");
    }

    [Fact]
    public void SetTheSortOrderAsSizeDescendingWhenRequestedForDuplicates()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.DuplicateDetails.OrderAsRequested(SortOrder.SizeDescending);

        response.Expression
                .ToString()
                .ShouldBe("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression].OrderByDescending(fileDetail => fileDetail.FileSize)");
    }

    [Fact]
    public void SetTheRequestedSearchTypeForAll()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.FileDetails.SetSearchType(SearchType.All);

        response.Expression
                .ToString()
                .ShouldBe("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression]");
    }

    [Fact]
    public void SetTheRequestedSearchTypeForDuplicateImages()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.FileDetails.SetSearchType(SearchType.DuplicateImages);

        response.Expression
                .ToString()
                .ShouldBe("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression].OrderByDescending(fileDetail => fileDetail.FileName)");
    }

    [Fact]
    public void SetTheRequestedSearchTypeForDuplicates()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.FileDetails.SetSearchType(SearchType.Duplicates);

        response.Expression
                .ToString()
                .ShouldBe("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression].OrderBy(fileDetail => fileDetail.FileSize)");
    }

    [Fact]
    public void SetTheRequestedSearchTypeForImages()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.FileDetails.SetSearchType(SearchType.Images);

        response.Expression
                .ToString()
                .ShouldBe("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression].OrderByDescending(fileDetail => fileDetail.FileSize)");
    }

    [Fact]
    public void SetTheRequestedSearchTypeForDuplicateImagesForDuplicates()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.DuplicateDetails.SetSearchType(SearchType.DuplicateImages);

        response.Expression
                .ToString()
                .ShouldBe("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression].OrderByDescending(fileDetail => fileDetail.FileName)");
    }

    [Fact]
    public void SetTheRequestedSearchTypeForDuplicatesForDuplicates()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.DuplicateDetails.SetSearchType(SearchType.Duplicates);

        response.Expression
                .ToString()
                .ShouldBe("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression].OrderBy(fileDetail => fileDetail.FileSize)");
    }

    [Fact]
    public void SelectTheFirstPageWhenRequestedForDuplicates()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.DuplicateDetails.SelectRequestedPage(1, 10);

        response.Expression
                .ToString()
                .ShouldBe("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression].Skip(0).Take(10)");
    }

    [Fact]
    public void SelectTheNthPageWhenRequestedForDuplicates()
    {
        var optionsBuilder = new DbContextOptionsBuilder<FilesContext>();
        optionsBuilder.UseSqlServer("");
        var sut = new FilesContext(optionsBuilder.Options);

        var response = sut.DuplicateDetails.SelectRequestedPage(3, 10);

        response.Expression
                .ToString()
                .ShouldBe("[Microsoft.EntityFrameworkCore.Query.EntityQueryRootExpression].Skip(20).Take(10)");
    }
}
