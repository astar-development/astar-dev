using AStar.Dev.Utilities;
using JetBrains.Annotations;

namespace AStar.Dev.Files.Api.Files.Add.V1;

[TestSubject(typeof(FileClassification))]
public class ImageDetailsShould
{
    [Fact]
    public void ContainTheAddTheExpectedProperties()
        => new ImageDetails().ToJson().ShouldMatchApproved();
}
