using AStar.Dev.Admin.Api.SiteConfigurations.GetBySlug.V1;
using JetBrains.Annotations;

namespace AStar.Dev.Admin.Api.SiteConfiguration.GetBySlug.V1;

[TestSubject(typeof(GetSiteConfigurationBySlugResponse))]
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

        var sut = new GetSiteConfigurationBySlugResponse(siteConfiguration);

        sut.Password.ShouldBe("siteConfiguration.Password");
        sut.Username.ShouldBe("siteConfiguration.Username");
        sut.LoginEmailAddress.ShouldBe("siteConfiguration.LoginEmailAddress");
        sut.BaseUrl.ShouldBe("siteConfiguration.BaseDirectory");
        sut.LoginUrl.ShouldBe("LoginUrl");
    }
}
