using AStar.Dev.Utilities;

namespace AStar.Dev.Files.Api.Client.Sdk.Models;

[TestSubject(typeof(GetDuplicatesCountQueryResponse))]
public class GetDuplicatesCountQueryResponseShould
{
    [Fact]
    public void ContainTheExpectedProperties()
        => new GetDuplicatesCountQueryResponse(12).ToJson().ShouldMatchApproved();
}
