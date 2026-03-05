using Microsoft.AspNetCore.Mvc.Testing;

namespace AStar.Dev.Web.Tests.Integration;

public class BasicTests(CustomWebApplicationFactory<Program> webApplicationFactory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    [Theory]
    [InlineData("/")]
    [InlineData("/Index")]
    [InlineData("/About")]
    [InlineData("/Privacy")]
    [InlineData("/Contact")]
    public async Task GetEndpointsRedirectAsAlPagesAreSecured(string url)
    {
        HttpClient client = webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false, });

        HttpResponseMessage response = await client.GetAsync(url, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        Assert.StartsWith("https://login.microsoftonline.com/", response.Headers.Location?.OriginalString);
    }
}
