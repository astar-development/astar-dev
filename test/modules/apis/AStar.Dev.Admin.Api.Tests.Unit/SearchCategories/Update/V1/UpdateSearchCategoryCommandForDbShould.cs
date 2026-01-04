using System.Security.Claims;
using AStar.Dev.Api.Usage.Sdk.Metrics;
using AStar.Dev.Utilities;
using JetBrains.Annotations;

namespace AStar.Dev.Admin.Api.SearchCategories.Update.V1;

[TestSubject(typeof(UpdateSearchCategoryCommandForDb))]
public sealed class UpdateSearchCategoryCommandForDbShould
{
    [Fact]
    public void ContainTheExpectedValues()
    {
        var sut = new UpdateSearchCategoryCommandForDb(Guid.Empty, new ClaimsPrincipal(), new UpdateSearchCategoryCommand());

        sut.ToJson().ShouldMatchApproved();
    }

    [Fact]
    public void ImplementIEndpointNameInterface()
    {
        var sut = new UpdateSearchCategoryCommandForDb(Guid.Empty, new ClaimsPrincipal(), new UpdateSearchCategoryCommand());

        sut.ShouldBeAssignableTo<IEndpointName>();
    }
}
