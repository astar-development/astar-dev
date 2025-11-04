// using AStar.Dev.Utilities;
// using JetBrains.Annotations;
//
// namespace AStar.Dev.Admin.Api.SearchConfiguration.GetAll.V1;
//
// [TestSubject(typeof(GetAllSearchConfigurationsResponse))]
// public sealed class GetAllSearchConfigurationResponseShould
// {
//     [Fact]
//     public void ContainTheExpectedValues()
//     {
//         var searchConfiguration = new Infrastructure.AdminDb.Models.SearchConfiguration
//     {
//         SiteConfigurationSlug = "SiteConfigurationSlug",
//         UpdatedBy   = "UpdatedBy",
//         UpdatedOn   = new(2001, 1, 1),
//         Id          = 1,
//         Subscriptions         = "Subscriptions",
//         SearchString          = "SearchString",
//         SearchStringSuffix    = "SearchStringSuffix",
//         SearchStringPrefix    = "SearchStringPrefix",
//         SearchConfigurationId = Guid.NewGuid(),
//         TopWallpapers         = "TopWallpapers",
//         SubscriptionsTotalPages         = 1,
//         TotalPages  = 2,
//         StartingPageNumber    = 3,
//         SubscriptionsStartingPageNumber = 4,
//         TopWallpapersTotalPages         = 5,
//         TopWallpapersStartingPageNumber = 5,
//         UseHeadless           = true,
//         ImagePauseInSeconds   = 5,
//         SlowMotionDelayInMilliseconds   = 500,
//     };
//
//         var sut = new GetAllSearchConfigurationsResponse(searchConfiguration);
//
//         sut.ToJson().ShouldMatchApproved();
//     }
// }

