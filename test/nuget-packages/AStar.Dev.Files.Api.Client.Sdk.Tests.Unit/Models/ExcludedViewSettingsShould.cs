using AStar.Dev.Files.Api.Client.SDK.Models;

namespace AStar.Dev.Files.Api.Client.Sdk.Models;

public sealed class ExcludedViewSettingsShould
{
    [Fact]
    public void ReturnTheExpectedToString() =>
        new ExcludedViewSettings().ToString().ShouldMatchApproved();
}
