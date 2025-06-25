using AStar.Dev.Files.Api.Client.SDK.Models;

namespace AStar.Dev.Files.Api.Client.Sdk.Models;

public sealed class FileDimensionsWithSizeShould
{
    [Fact]
    public void ReturnTheExpectedToString() =>
        new FileDimensionsWithSize().ToString().ShouldMatchApproved();
}
