using System.Security.Claims;
using AStar.Dev.Api.Usage.Sdk.Metrics;
using AStar.Dev.Utilities;
using JetBrains.Annotations;

namespace AStar.Dev.Admin.Api.ScrapeDirectories.Update.V1;

[TestSubject(typeof(UpdateScrapeDirectoriesCommandForDb))]
public sealed class UpdateScrapeDirectoriesCommandForDbShould
{
    [Fact]
    public void ContainTheExpectedValues()
    {
        var sut = new UpdateScrapeDirectoriesCommandForDb(Guid.Empty, new ClaimsPrincipal(), new UpdateScrapeDirectoriesCommand());

        sut.ToJson().ShouldMatchApproved();
    }

    [Fact]
    public void ImplementIEndpointNameInterface()
    {
        var sut = new UpdateScrapeDirectoriesCommandForDb(Guid.Empty, new ClaimsPrincipal(), new UpdateScrapeDirectoriesCommand());

        sut.ShouldBeAssignableTo<IEndpointName>();
    }
}
