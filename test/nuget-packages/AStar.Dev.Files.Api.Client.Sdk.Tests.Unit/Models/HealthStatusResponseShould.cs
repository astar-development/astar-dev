using AStar.Dev.Api.HealthChecks;

namespace AStar.Dev.Files.Api.Client.Sdk.Models;

public sealed class HealthStatusResponseShould
{
    [Fact]
    public void ReturnTheExpectedToString()
    {
        var mockDataObject = new { Id = 1, Name = "MockName" };

        new HealthStatusResponse
        {
            DurationInMilliseconds = 987,
            Description            = "MockDescription",
            Exception              = "MockException",
            Name                   = "MockName",
            Status                 = "MockStatus",
            Data                   = new Dictionary<string, object> { { "Id", mockDataObject } }
        }.ToString()!.ShouldMatchApproved();
    }
}
