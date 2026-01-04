using AStar.Dev.Admin.Api.SiteConfigurations.GetAll.V1;
using JetBrains.Annotations;

namespace AStar.Dev.Admin.Api.SiteConfiguration.GetAll.V1;

[TestSubject(typeof(GetAllSiteConfigurationsResponse))]
public sealed class GetSiteConfigurationResponseShould
{
    [Fact]
    public void ContainTheExpectedValues()
    {
        var siteConfiguration = new Infrastructure.AdminDb.Models.SiteConfiguration
                                {
                                    SiteConfigurationSlug = "SiteConfigurationSlug",
                                    Password              = "siteConfiguration.Password",
                                    Username              = "siteConfiguration.Username",
                                    LoginEmailAddress     = "siteConfiguration.LoginEmailAddress",
                                    BaseUrl               = "siteConfiguration.BaseDirectory",
                                    LoginUrl              = "LoginUrl",
                                };

        var sut = new GetAllSiteConfigurationsResponse(siteConfiguration);

        sut.SiteConfigurationSlug.ShouldBe("SiteConfigurationSlug");
        sut.Password.ShouldBe("siteConfiguration.Password");
        sut.Username.ShouldBe("siteConfiguration.Username");
        sut.LoginEmailAddress.ShouldBe("siteConfiguration.LoginEmailAddress");
        sut.BaseUrl.ShouldBe("siteConfiguration.BaseDirectory");
        sut.LoginUrl.ShouldBe("LoginUrl");
    }
}
