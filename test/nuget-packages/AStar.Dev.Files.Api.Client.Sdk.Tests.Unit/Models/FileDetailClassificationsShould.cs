using AStar.Dev.Utilities;
using JetBrains.Annotations;

namespace AStar.Dev.Files.Api.Client.Sdk.Models;

[TestSubject(typeof(FileDetailClassifications))]
public class FileDetailClassificationsShould
{
    [Fact]
    public void ContainTheExpectedProperties()
        => new FileDetailClassifications { ClassificationId = 1, Name = "MockName" }.ToJson().ShouldMatchApproved();
}
