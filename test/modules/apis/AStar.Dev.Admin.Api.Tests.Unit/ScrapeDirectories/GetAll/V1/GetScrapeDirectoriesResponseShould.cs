// using AStar.Dev.Infrastructure.AdminDb.Models;
// using AStar.Dev.Utilities;
// using JetBrains.AStar.Dev.Annotations;
//
// namespace AStar.Dev.Admin.Api.ScrapeDirectories.GetAll.V1;
//
// [TestSubject(typeof(GetAllScrapeDirectoriesQueryResponse))]
// public sealed class GetScrapeDirectoriesResponseShould
// {
//     [Fact]
//     public void ContainTheExpectedValues()
//     {
//         var scrapeDirectories = new ScrapeDirectory
//                       {
//                                     UpdatedBy           = "UpdatedBy",
//                                     UpdatedOn           = new(2001, 1, 1),
//                                     BaseDirectory       = "BaseDirectory",
//                                     BaseDirectoryFamous = "BaseDirectoryFamous",
//                                     BaseSaveDirectory   = "BaseSaveDirectory",
//                                     RootDirectory       = "RootDirectory",
//                                     SubDirectoryName    = "SubDirectoryName",
//                                     ScrapeDirectoryId   = Guid.Empty,
//                                     Id                  = 1,
//                                 };
//
//         var sut = new GetAllScrapeDirectoriesQueryResponse(scrapeDirectories);
//
//         sut.ToJson().ShouldMatchApproved();
//     }
// }

