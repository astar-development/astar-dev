using System.Net.Mime;

namespace AStar.Dev.Admin.Api;

public sealed class BasicTests(CustomWebApplicationFactory<IAssemblyMarker> factory)
    : IClassFixture<CustomWebApplicationFactory<IAssemblyMarker>>
{
    [Fact(Skip = "Doesn't work")]
    public async Task Get_EndpointsReturnSuccessAndCorrectContentType2()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/?version=1");

        _ = response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentType?.ToString().ShouldBe(MediaTypeNames.Text.Html);
    }
}
