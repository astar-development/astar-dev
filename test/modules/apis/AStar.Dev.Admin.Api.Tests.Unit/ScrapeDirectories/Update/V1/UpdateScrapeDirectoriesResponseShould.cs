// using AStar.Dev.Utilities;
// using JetBrains.AStar.Dev.Annotations;
//
// namespace AStar.Dev.Admin.Api.ScrapeDirectories.Update.V1;
//
// [TestSubject(typeof(UpdateScrapeDirectoriesResponse))]
// public sealed class UpdateScrapeDirectoriesResponseShould
// {
//     [Fact]
//     public void ContainTheExpectedValues()
//     {
//         var sut = new UpdateScrapeDirectoriesResponse(new()
//     {
//         UpdatedBy = "UpdatedBy",
//         UpdatedOn = new(2001, 1, 1),
//         BaseDirectory       = "BaseDirectory",
//         BaseDirectoryFamous = "BaseDirectoryFamous",
//         BaseSaveDirectory   = "BaseSaveDirectory",
//         RootDirectory       = "RootDirectory",
//         SubDirectoryName    = "SubDirectoryName",
//         ScrapeDirectoryId   = Guid.Empty,
//         Id        = 1,
//     });
//
//         sut.ToJson().ShouldMatchApproved();
//     }
// }

