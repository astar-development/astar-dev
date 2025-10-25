// using AStar.Dev.Infrastructure.FilesDb.Data;
// using AStar.Dev.Infrastructure.FilesDb.Fixtures;
// using AStar.Dev.Infrastructure.FilesDb.Models;
//
// namespace AStar.Dev.Infrastructure.FilesDb;
//
// public sealed class EnumerableExtensionsShould(FilesContextFixture filesContextFixture) : IClassFixture<FilesContextFixture>
// {
//     private readonly FilesContext sut = filesContextFixture.Sut;
//
//     [Fact]
//     public void ReturnCorrectCountWhenFilteringImages()
//     {
//         var response = sut.Files.FilterImagesIfApplicable("Images", CancellationToken.None);
//
//         response.ToJson().ToString()!.ShouldMatchApproved();
//     }
//
//     [Fact]
//     public void ReturnCorrectCountWhenAllFileTypesSpecified()
//     {
//         var response = sut.Files.FilterImagesIfApplicable("AllFiles", CancellationToken.None);
//
//         response.ToJson().ToString()!.ShouldMatchApproved();
//     }
//
//     [Fact]
//     public void ReturnCorrectCountWhenDuplicatesSpecified()
//     {
//         var response = sut.Files.FilterImagesIfApplicable("Duplicates", CancellationToken.None);
//
//         response.ToJson().ToString()!.ShouldMatchApproved();
//     }
//
//     [Fact]
//     public void ReturnTheExpectedFilesListByNameAscending()
//     {
//         var response = sut.Files.OrderFiles(SortOrder.NameAscending);
//
//         response.ToJson().ToString()!.ShouldMatchApproved();
//     }
//
//     [Fact]
//     public void ReturnTheExpectedFilesListByNameDescending()
//     {
//         var response = sut.Files.OrderFiles(SortOrder.NameDescending);
//
//         response.ToJson().ToString()!.ShouldMatchApproved();
//     }
//
//     [Fact]
//     public void ReturnTheExpectedFilesListBySizeAscending()
//     {
//         var response = sut.Files.OrderFiles(SortOrder.SizeAscending);
//
//         response.ToJson().ToString()!.ShouldMatchApproved();
//     }
//
//     [Fact]
//     public void ReturnTheExpectedFilesListBySizeDescending()
//     {
//         var response = sut.Files.OrderFiles(SortOrder.SizeDescending);
//
//         response.ToJson().ToString()!.ShouldMatchApproved();
//     }
//
//     [Fact]
//     public void ReturnTheCorrectDuplicatesCount()
//     {
//         var response = sut.Files.GetDuplicatesCount(CancellationToken.None);
//
//         response.ShouldBe(30);
//     }
//
//     [Fact]
//     public void ReturnTheCorrectDuplicates()
//     {
//         var response = sut.Files.GetDuplicates();
//
//         response.ToJson().ToString()!.ShouldMatchApproved();
//     }
// }

