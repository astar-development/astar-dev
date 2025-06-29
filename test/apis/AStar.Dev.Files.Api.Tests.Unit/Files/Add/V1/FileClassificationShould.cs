using AStar.Dev.Utilities;
using JetBrains.Annotations;

namespace AStar.Dev.Files.Api.Files.Add.V1;

[TestSubject(typeof(FileClassification))]
public class FileClassificationShould
{
    [Fact]
    public void ContainTheAddTheExpectedProperties()
        => new FileClassification().ToJson().ShouldMatchApproved();
}
