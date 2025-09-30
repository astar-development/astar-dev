using AStar.Dev.Utilities;

namespace AStar.Dev.Files.Api.Endpoints.Add.V1;

[TestSubject(typeof(FileClassification))]
public class FileClassificationShould
{
    [Fact]
    public void ContainTheAddTheExpectedProperties() => new FileClassification().ToJson().ShouldMatchApproved();
}
