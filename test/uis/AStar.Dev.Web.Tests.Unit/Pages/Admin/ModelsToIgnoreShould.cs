using AStar.Dev.Web.Components.Pages.Admin;
using Microsoft.AspNetCore.Authorization;

namespace AStar.Dev.Web.Tests.Unit.Pages.Admin;

public class ModelsToIgnoreShould
{
    [Fact]
    public void HaveTheAuthorizeAttribute()
        => typeof(ModelsToIgnore)
            .GetCustomAttributes(typeof(AuthorizeAttribute), true)
            .ShouldNotBeEmpty();

    [Fact]
    public void HaveTheCorrectAdminPolicy()
        => typeof(ModelsToIgnore)
            .GetCustomAttributes(typeof(AuthorizeAttribute), true).OfType<AuthorizeAttribute>().First(p => p.Policy != null).Policy
            .ShouldBe("AdminOnly");
}
