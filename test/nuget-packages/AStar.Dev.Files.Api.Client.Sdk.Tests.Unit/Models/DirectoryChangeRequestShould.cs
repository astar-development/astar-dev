using AStar.Dev.Files.Api.Client.SDK.Models;

namespace AStar.Dev.Files.Api.Client.Sdk.Models;

public sealed class DirectoryChangeRequestShould
{
    [Fact]
    public void ReturnTheExpectedToString() =>
        new DirectoryChangeRequest { FileName = "MockFileName", NewDirectoryName = "NewMockDirectoryName", OldDirectoryName = "MockOldDirectoryName" }.ToString().ShouldMatchApproved();
}
