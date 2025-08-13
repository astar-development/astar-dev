using AStar.Dev.Utilities;

namespace AStar.Dev.Files.Api.Endpoints.Add.V1;

[TestSubject(typeof(FileClassification))]
public class ImageDetailsShould
{
    [Fact]
    public void ContainTheAddTheExpectedProperties()
        => new ImageDetails().ToJson().ShouldMatchApproved();
}
