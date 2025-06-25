using AStar.Dev.Files.Api.Client.SDK.Models;

namespace AStar.Dev.Files.Api.Client.Sdk.Models;

public sealed class FileDetailShould
{
    [Fact(Skip = "Doesn't work...")]
    public void ReturnTheExpectedToString() =>
        new FileDetail().ToString().ShouldMatchApproved();
}
