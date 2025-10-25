using AStar.Dev.Files.Api.Client.SDK.Models;

namespace AStar.Dev.Files.Api.Client.Sdk.Tests.Unit.Models;

public sealed class DirectoryChangeRequestShould
{
    [Fact]
    public void ReturnTheExpectedToString()
        => new DirectoryChangeRequest().ToString().ShouldMatchApproved();
}
