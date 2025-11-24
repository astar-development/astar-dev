using AStar.Dev.Web.Components.Pages.Admin;
using Microsoft.AspNetCore.Authorization;

namespace AStar.Dev.Web.Tests.Unit.Pages.Admin;

public sealed class SiteConfigurationShould
{
    [Fact]
    public void HaveTheAuthorizeAttribute()
        => typeof(SiteConfiguration)
            .GetCustomAttributes(typeof(AuthorizeAttribute), true)
            .ShouldNotBeEmpty();

    [Fact]
    public void HaveTheCorrectAdminPolicy()
        => typeof(SiteConfiguration)
            .GetCustomAttributes(typeof(AuthorizeAttribute), true).OfType<AuthorizeAttribute>().First(p => p.Policy != null).Policy
            .ShouldBe("AdminOnly");
}
