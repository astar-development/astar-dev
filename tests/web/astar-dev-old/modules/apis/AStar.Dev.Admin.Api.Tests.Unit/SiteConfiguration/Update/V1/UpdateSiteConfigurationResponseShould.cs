using AStar.Dev.Admin.Api.SiteConfigurations.Update.V1;
using AStar.Dev.Utilities;
using JetBrains.Annotations;

namespace AStar.Dev.Admin.Api.SiteConfiguration.Update.V1;

[TestSubject(typeof(UpdateSiteConfigurationResponse))]
public sealed class UpdateSiteConfigurationResponseTest
{
    [Fact]
    public void ContainTheExpectedValues()
    {
        var sut = new UpdateSiteConfigurationResponse();

        sut.ToJson().ShouldMatchApproved();
    }
}
