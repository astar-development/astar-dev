using AStar.Dev.Files.Api.Client.SDK.Models;

namespace AStar.Dev.Files.Api.Client.Sdk.Models;

public sealed class DuplicateGroupShould
{
    [Fact]
    public void ReturnTheExpectedToString() =>
        new DuplicateGroup().ToString().ShouldMatchApproved();
}
