using AStar.Dev.Utilities;
using JetBrains.Annotations;

namespace AStar.Dev.Minimal.Api.Extensions.Tests.Unit;

[TestSubject(typeof(ApiVersion))]
public sealed class ApiVersionShould
{
    [Fact]
    public void ContainTheExpectedProperties()
        => new ApiVersion().ToJson().ShouldMatchApproved();
}
