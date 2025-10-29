using System.Net.Mime;

namespace AStar.Dev.Admin.Api.Tests.Integration;

public sealed class BasicTests(CustomWebApplicationFactory<IAssemblyMarker> factory)
    : IClassFixture<CustomWebApplicationFactory<IAssemblyMarker>>
{
    [Fact(Skip = "Doesn't work")]
    public async Task GetEndpointsReturnSuccessAndCorrectContentType2Async()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/?version=1", TestContext.Current.CancellationToken);

        _ = response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentType?.ToString().ShouldBe(MediaTypeNames.Text.Html);
    }
}
