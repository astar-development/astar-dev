using JetBrains.Annotations;

namespace AStar.Dev.Admin.Api;

[TestSubject(typeof(EndpointConstants))]
public sealed class EndpointConstantsShould
{
    [Fact]
    public void ContainTheSiteConfigurationWithTheExpectedValue()
        => EndpointConstants.SiteConfigurationsEndpoint.ShouldBe("/site-configurations");

    [Fact]
    public void ContainTheSiteConfigurationTagWithTheExpectedValue()
        => EndpointConstants.SiteConfigurationTag.ShouldBe("Site Configurations");

    [Fact]
    public void ContainTheSiteConfigurationGroupNameWithTheExpectedValue()
        => EndpointConstants.SiteConfigurationGroupName.ShouldBe("SiteConfigurations");
}
