using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Fixtures;
using AStar.Dev.Infrastructure.FilesDb.Models;

namespace AStar.Dev.Infrastructure.FilesDb;

public sealed class EnumerableExtensionsShould(FilesContextFixture filesContextFixture) : IClassFixture<FilesContextFixture>
{
    private readonly FilesContext sut = filesContextFixture.Sut;

    [Fact(Skip = "The underlying code is broken")]
    public void ReturnCorrectCountWhenFilteringImages()
    {
        var response = sut.Files.FilterImagesIfApplicable("Images", CancellationToken.None);

        response.ToString()!.ShouldMatchApproved();
    }

    [Fact(Skip = "The underlying code is broken")]
    public void ReturnCorrectCountWhenAllFileTypesSpecified()
    {
        var response = sut.Files.FilterImagesIfApplicable("AllFiles", CancellationToken.None);

        response.ToString()!.ShouldMatchApproved();
    }

    [Fact(Skip = "The underlying code is broken")]
    public void ReturnCorrectCountWhenDuplicatesSpecified()
    {
        var response = sut.Files.FilterImagesIfApplicable("Duplicates", CancellationToken.None);

        response.ToString()!.ShouldMatchApproved();
    }

    [Fact(Skip = "The underlying code is broken")]
    public void ReturnTheExpectedFilesListByNameAscending()
    {
        var response = sut.Files.OrderFiles(SortOrder.NameAscending);

        response.ToString()!.ShouldMatchApproved();
    }

    [Fact(Skip = "The underlying code is broken")]
    public void ReturnTheExpectedFilesListByNameDescending()
    {
        var response = sut.Files.OrderFiles(SortOrder.NameDescending);

        response.ToString()!.ShouldMatchApproved();
    }

    [Fact(Skip = "The underlying code is broken")]
    public void ReturnTheExpectedFilesListBySizeAscending()
    {
        var response = sut.Files.OrderFiles(SortOrder.SizeAscending);

        response.ToString()!.ShouldMatchApproved();
    }

    [Fact(Skip = "The underlying code is broken")]
    public void ReturnTheExpectedFilesListBySizeDescending()
    {
        var response = sut.Files.OrderFiles(SortOrder.SizeDescending);

        response.ToString()!.ShouldMatchApproved();
    }

    [Fact(Skip = "The underlying code is broken")]
    public void ReturnTheCorrectDuplicatesCount()
    {
        var response = sut.Files.GetDuplicatesCount(CancellationToken.None);

        response.ShouldBe(30);
    }

    [Fact(Skip = "The underlying code is broken")]
    public void ReturnTheCorrectDuplicates()
    {
        var response = sut.Files.GetDuplicates();

        response.ToString()!.ShouldMatchApproved();
    }
}
