using AStar.Dev.Api.HealthChecks;

namespace AStar.Dev.Files.Api.Client.Sdk.Models;

public sealed class HealthStatusResponseShould
{
    [Fact]
    public void ReturnTheExpectedToString() =>
        new HealthStatusResponse().ToString()!.ShouldMatchApproved();
}
