using AStar.Dev.Files.Api.Client.SDK.Models;

namespace AStar.Dev.Files.Api.Client.Sdk.Tests.Unit.Models;

public sealed class FileAccessDetailShould
{
    [Fact]
    public void ReturnTheExpectedToString()
        => new FileAccessDetail().ToString()
            .ShouldMatchApproved();
}
