using AStar.Dev.Files.Api.Client.SDK.Models;
using AStar.Dev.Utilities;
using JetBrains.Annotations;

namespace AStar.Dev.Files.Api.Client.Sdk.Models;

[TestSubject(typeof(FileDetailClassification))]
public class FileDetailClassificationShould
{
    [Fact]
    public void ContainTheExpectedProperties()
        => new FileDetailClassification { ClassificationId = 123, FileDetail = new (), FileDetailClassifications = new (), FileId = 312 }
           .ToJson()
           .ShouldMatchApproved();
}
