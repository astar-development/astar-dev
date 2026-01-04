using System.Security.Claims;
using AStar.Dev.Api.Usage.Sdk.Metrics;
using AStar.Dev.Utilities;
using JetBrains.Annotations;

namespace AStar.Dev.Admin.Api.SearchConfiguration.GetAll.V1;

[TestSubject(typeof(GetAllSearchConfigurationsQuery))]
public sealed class GetAllSearchConfigurationsQueryShould
{
    [Fact]
    public void ContainTheExpectedValues()
    {
        var sut = new GetAllSearchConfigurationsQuery(new ClaimsPrincipal());

        sut.ToJson().ShouldMatchApproved();
    }

    [Fact]
    public void ImplementIEndpointNameInterface()
    {
        var sut = new GetAllSearchConfigurationsQuery(new ClaimsPrincipal());

        sut.ShouldBeAssignableTo<IEndpointName>();
    }
}
