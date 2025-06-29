using AStar.Dev.Utilities;
using JetBrains.Annotations;

namespace AStar.Dev.Files.Api.Files.Add.V1;

[TestSubject(typeof(AddFilesResponse))]
public class AddFilesResponseShould
{
    [Fact]
    public void ContainTheAddTheExpectedProperties()
        => new AddFilesResponse().ToJson().ShouldMatchApproved();
}
