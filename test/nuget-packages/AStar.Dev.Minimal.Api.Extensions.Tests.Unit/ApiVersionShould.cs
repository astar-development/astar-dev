using AStar.Dev.Minimal.Api.Extensions;
using AStar.Dev.Utilities;

namespace AStar.Dev.AspNet.Minimal.Api.Extensions;

[TestSubject(typeof(ApiVersion))]
public sealed class ApiVersionShould
{
    [Fact]
    public void ContainTheExpectedProperties() =>
        new ApiVersion().ToJson().ShouldMatchApproved();
}
