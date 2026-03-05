using System.Security.Claims;
using AStar.Dev.Api.Usage.Sdk.Metrics;
using AStar.Dev.Utilities;
using JetBrains.Annotations;

namespace AStar.Dev.Admin.Api.SearchConfiguration.Update.V1;

[TestSubject(typeof(UpdateSearchConfigurationCommandForDb))]
public sealed class UpdateSearchConfigurationCommandForDbShould
{
    [Fact]
    public void ContainTheExpectedValues()
    {
        var sut = new UpdateSearchConfigurationCommandForDb("mock-slug", new ClaimsPrincipal(), new UpdateSearchConfigurationCommand());

        sut.ToJson().ShouldMatchApproved();
    }

    [Fact]
    public void ImplementIEndpointNameInterface()
    {
        var sut = new UpdateSearchConfigurationCommandForDb("mock-slug", new ClaimsPrincipal(), new UpdateSearchConfigurationCommand());

        sut.ShouldBeAssignableTo<IEndpointName>();
    }
}
