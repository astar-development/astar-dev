using System.Security.Claims;
using AStar.Dev.Api.Usage.Sdk.Metrics;
using AStar.Dev.Utilities;
using JetBrains.Annotations;

namespace AStar.Dev.Admin.Api.SearchConfiguration.GetBySlug.V1;

[TestSubject(typeof(GetSearchConfigurationBySlugQuery))]
public sealed class GetSearchConfigurationQueryShould
{
    [Fact]
    public void ContainTheExpectedValues()
    {
        var sut = new GetSearchConfigurationBySlugQuery("doesn't matter", new ClaimsPrincipal());

        sut.ToJson().ShouldMatchApproved();
    }

    [Fact]
    public void ImplementIEndpointNameInterface()
    {
        var sut = new GetSearchConfigurationBySlugQuery("doesn't matter", new ClaimsPrincipal());

        sut.ShouldBeAssignableTo<IEndpointName>();
    }
}
