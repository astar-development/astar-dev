using AStar.Dev.Utilities;

namespace AStar.Dev.Files.Api.Endpoints.Get.V1;

public class GetFilesRequestShould
{
    [Fact]
    public void ContainTheExpectedProperties()
        => new GetFilesRequest().ToJson().ShouldMatchApproved();
}
