using AStar.Dev.Utilities;

namespace AStar.Dev.Files.Api.Client.Sdk.Models;

[TestSubject(typeof(FileGrouping))]
public class FileGroupingShould
{
    [Fact]
    public void ContainTheExpectedProperties()
        => new FileGrouping { FileSize = 1234, Height = 123, Width = 456 }.ToJson().ShouldMatchApproved();
}
