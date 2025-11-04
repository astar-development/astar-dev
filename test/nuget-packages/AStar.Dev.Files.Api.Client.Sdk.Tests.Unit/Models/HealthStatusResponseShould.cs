using AStar.Dev.Api.HealthChecks;

namespace AStar.Dev.Files.Api.Client.Sdk.Tests.Unit.Models;

public sealed class HealthStatusResponseShould
{
    [Fact]
    public void ReturnTheExpectedToString()
        => new HealthStatusResponse().ToString()!.ShouldMatchApproved();
}
