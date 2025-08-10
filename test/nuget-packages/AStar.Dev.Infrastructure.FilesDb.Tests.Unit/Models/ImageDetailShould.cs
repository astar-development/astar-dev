using AStar.Dev.Utilities;

namespace AStar.Dev.Infrastructure.FilesDb.Models;

public class ImageDetailShould
{
    [Fact]
    public void ContainTheExpectedProperties()
        => new ImageDetail (  123, 456)
           .ToJson()
           .ShouldMatchApproved();
}
