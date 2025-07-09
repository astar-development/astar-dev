using AStar.Dev.Utilities;
using JetBrains.Annotations;

namespace AStar.Dev.Infrastructure.FilesDb.Models;

[TestSubject(typeof(ImageDetail))]
public class ImageDetailsShould
{
    [Fact]
    public void ContainTheExpectedProperties()
        => new ImageDetail(1234, 5678).ToJson().ShouldMatchApproved();

    [Fact]
    public void ReturnTheExpectedToString()
        => new ImageDetail(1234, 5678).ToString().ShouldMatchApproved();
}
