using AStar.Dev.Utilities;

namespace AStar.Dev.Files.Api.Endpoints.Add.V1;

[TestSubject(typeof(AddFilesResponse))]
public class AddFilesResponseShould
{
    [Fact]
    public void ContainTheAddTheExpectedProperties()
        => new AddFilesResponse().ToJson().ShouldMatchApproved();
}
