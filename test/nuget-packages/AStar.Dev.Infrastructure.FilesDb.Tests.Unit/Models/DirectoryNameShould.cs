using AStar.Dev.Utilities;

namespace AStar.Dev.Infrastructure.FilesDb.Models;

public class DirectoryNameShould
{
    [Fact]
    public void ContainTheExpectedProperties()
        => new DirectoryName("test")
           .ToJson()
           .ShouldMatchApproved();
}
