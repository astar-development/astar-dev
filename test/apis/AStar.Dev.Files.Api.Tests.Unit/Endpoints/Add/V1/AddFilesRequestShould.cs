using AStar.Dev.Utilities;
using JetBrains.Annotations;

namespace AStar.Dev.Files.Api.Endpoints.Add.V1;

[TestSubject(typeof(AddFilesRequest))]
public class AddFilesRequestShould
{
    [Fact]
    public void ContainTheAddTheExpectedProperties()
        => new AddFilesRequest { FilesToAdd = new List<FileDetailToAdd> { new() } }.ToJson().ShouldMatchApproved();
}
