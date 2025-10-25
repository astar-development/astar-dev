// using System.Net;
//
// namespace AStar.Dev.Web.UI;
//
// public class BasicTests(CustomWebApplicationFactory<Program> webApplicationFactory) : IClassFixture<CustomWebApplicationFactory<Program>>
// {
//     [Theory]
//     [InlineData("/")]
//     [InlineData("/Index")]
//     [InlineData("/About")]
//     [InlineData("/Privacy")]
//     [InlineData("/Contact")]
//     public async Task GetEndpointsRedirectAsAlPagesAreSecured(string url)
//     {
//         var client = webApplicationFactory.CreateClient(new()  { AllowAutoRedirect = false, });
//
//         var response = await client.GetAsync(url);
//
//         Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
//
//         Assert.StartsWith("https://login.microsoftonline.com/", response.Headers.Location?.OriginalString);
//     }
// }

