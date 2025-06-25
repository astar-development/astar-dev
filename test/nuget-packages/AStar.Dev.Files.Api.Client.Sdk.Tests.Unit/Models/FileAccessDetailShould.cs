using AStar.Dev.Files.Api.Client.SDK.Models;

namespace AStar.Dev.Files.Api.Client.Sdk.Models;

public sealed class FileAccessDetailShould
{
    [Fact]
    public void ReturnTheExpectedToString() =>
        new FileAccessDetail().ToString()
                              .ShouldMatchApproved();
}
