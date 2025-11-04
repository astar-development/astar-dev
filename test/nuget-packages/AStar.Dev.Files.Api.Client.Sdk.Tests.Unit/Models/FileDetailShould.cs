using AStar.Dev.Files.Api.Client.SDK.Models;

namespace AStar.Dev.Files.Api.Client.Sdk.Tests.Unit.Models;

public sealed class FileDetailShould
{
    [Fact(Skip = "Doesn't work...")]
    public void ReturnTheExpectedToString()
        => new FileDetail().ToString().ShouldMatchApproved();
}
