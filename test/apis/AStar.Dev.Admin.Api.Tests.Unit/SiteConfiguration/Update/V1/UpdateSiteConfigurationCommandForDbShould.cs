// using AStar.Dev.Admin.Api.SiteConfigurations.Update.V1;
// using AStar.Dev.Api.Usage.Sdk.Metrics;
// using AStar.Dev.Utilities;
// using JetBrains.Annotations;
//
// namespace AStar.Dev.Admin.Api.SiteConfiguration.Update.V1;
//
// [TestSubject(typeof(UpdateSiteConfigurationCommandForDb))]
// public sealed class UpdateSiteConfigurationCommandForDbShould
// {
//     [Fact]
//     public void ContainTheExpectedValues()
//     {
//         var sut = new UpdateSiteConfigurationCommandForDb("mock-slug", new(), new());
//
//         sut.ToJson().ShouldMatchApproved();
//     }
//
//     [Fact]
//     public void ImplementIEndpointNameInterface()
//     {
//         var sut = new UpdateSiteConfigurationCommandForDb("mock-slug", new(), new());
//
//         sut.ShouldBeAssignableTo<IEndpointName>();
//     }
// }

