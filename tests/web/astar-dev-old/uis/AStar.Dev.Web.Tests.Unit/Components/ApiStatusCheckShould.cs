// using AStar.Dev.Web.Components;
// using JetBrains.AStar.Dev.Annotations;
// using Microsoft.Identity.Web;
// using NSubstitute;
// using Constants = AStar.Dev.Web.Infrastructure.Constants;
//
// namespace AStar.Dev.Web.UI.Components;
//
// #pragma warning disable BL0005
// [TestSubject(typeof(ApiStatusCheck))]
// public sealed class ApiStatusCheckShould
// {
//     private readonly MicrosoftIdentityConsentAndConditionalAccessHandler consentHandler;
//
//     public ApiStatusCheckShould()
//     {
//         var serviceProvider = Substitute.For<IServiceProvider>();
//         consentHandler = new(serviceProvider);
//     }
//
//     [Theory]
//     [InlineData("Healthy",        Constants.ApiNames.AdminApi)]
//     [InlineData("Failed",         Constants.ApiNames.FilesApi)]
//     [InlineData("Status that proves the Api Name isn't using the Status field LOL", Constants.ApiNames.ImagesApi)]
//     public async Task SetTheCorrectStatusApiName(string statusToReturn, string apiName)
//     {
//         var sut = new MockApiStatusCheck(consentHandler) { ApiClient = new MockApiClient(statusToReturn), ApiName = apiName, };
//
//         await sut.RunTheOnAfterRenderAsync();
//
//         sut.ApiName.ShouldBe(apiName);
//     }
//
//     [Theory]
//     [InlineData("Healthy",            Constants.ApiNames.AdminApi,  "assets/ai-generated-8001027_640.jpg")]
//     [InlineData("Failed",             Constants.ApiNames.FilesApi,  "assets/ai-generated-8165290_640.jpg")]
//     [InlineData("Status that proves the Image Source isn't using the Status field LOL", Constants.ApiNames.ImagesApi, "assets/cyborg-8829739_640.png")]
//     public async Task SetTheCorrectImageSource(string statusToReturn, string apiName, string expectedImageSource)
//     {
//         var sut = new MockApiStatusCheck(consentHandler) { ApiClient = new MockApiClient(statusToReturn), ApiName = apiName, };
//
//         await sut.RunTheOnAfterRenderAsync();
//
//         sut.ImageSource.ShouldBe(expectedImageSource);
//     }
//
//     [Theory]
//     [InlineData("Healthy",          Constants.ApiNames.AdminApi, "Healthy")]
//     [InlineData("Failed",           Constants.ApiNames.FilesApi, "Failed")]
//     [InlineData("Status that proves the Health Status is using the Status field LOL", Constants.ApiNames.ImagesApi,
//       "Status that proves the Health Status is using the Status field LOL")]
//     public async Task SetTheCorrectHealthStatus(string statusToReturn, string apiName, string expectedApiHealthStatus)
//     {
//         var sut = new MockApiStatusCheck(consentHandler) { ApiClient = new MockApiClient(statusToReturn), ApiName = apiName, };
//
//         await sut.RunTheOnAfterRenderAsync();
//
//         sut.ApiHealthStatus.ShouldBe(expectedApiHealthStatus);
//     }
//
//     [Theory]
//     [InlineData("Healthy",            Constants.ApiNames.AdminApi,  "alert alert-success status")]
//     [InlineData("Failed",             Constants.ApiNames.FilesApi,  "alert alert-danger status")]
//     [InlineData("Status that proves the Api FileClassification isn't using the Status field LOL", Constants.ApiNames.ImagesApi, "alert alert-danger status")]
//     public async Task SetTheCorrectHApiHealthCheckClass(string statusToReturn, string apiName, string expectedApiHealthCheckClass)
//     {
//         var sut = new MockApiStatusCheck(consentHandler) { ApiClient = new MockApiClient(statusToReturn), ApiName = apiName, };
//
//         await sut.RunTheOnAfterRenderAsync();
//
//         sut.ApiHealthCheckClass.ShouldBe(expectedApiHealthCheckClass);
//     }
// }
// #pragma warning restore BL0005
//
// public sealed class MockApiStatusCheck(MicrosoftIdentityConsentAndConditionalAccessHandler microsoftIdentityConsentAndConditionalAccessHandler)
//     : ApiStatusCheck(microsoftIdentityConsentAndConditionalAccessHandler)
// {
//     public async Task RunTheOnAfterRenderAsync() =>
//         await OnAfterRenderAsync(true);
// }

