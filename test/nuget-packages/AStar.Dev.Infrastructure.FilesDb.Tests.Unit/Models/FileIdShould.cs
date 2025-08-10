using AStar.Dev.Utilities;

namespace AStar.Dev.Infrastructure.FilesDb.Models;

public class FileIdShould
{
    [Fact]
    public void ContainTheExpectedProperties()
        => new FileId { Value = 68 }
           .ToJson()
           .ShouldMatchApproved();
}
