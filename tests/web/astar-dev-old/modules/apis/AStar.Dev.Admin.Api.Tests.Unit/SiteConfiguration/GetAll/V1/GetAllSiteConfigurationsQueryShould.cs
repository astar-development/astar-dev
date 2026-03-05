using System.Security.Claims;
using AStar.Dev.Admin.Api.SiteConfigurations.GetAll.V1;
using AStar.Dev.Api.Usage.Sdk.Metrics;
using AStar.Dev.Utilities;
using JetBrains.Annotations;

namespace AStar.Dev.Admin.Api.SiteConfiguration.GetAll.V1;

[TestSubject(typeof(GetAllSiteConfigurationsQuery))]
public sealed class GetAllSiteConfigurationsQueryTest
{
    [Fact]
    public void ContainTheExpectedValues()
    {
        var sut = new GetAllSiteConfigurationsQuery(new ClaimsPrincipal());

        sut.ToJson().ShouldMatchApproved();
    }

    [Fact]
    public void ImplementIEndpointNameInterface()
    {
        var sut = new GetAllSiteConfigurationsQuery(new ClaimsPrincipal());

        sut.ShouldBeAssignableTo<IEndpointName>();
    }
}
