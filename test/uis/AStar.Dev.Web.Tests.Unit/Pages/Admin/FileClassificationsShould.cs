using AStar.Dev.Web.Components.Pages.Admin;
using Microsoft.AspNetCore.Authorization;

namespace AStar.Dev.Web.Tests.Unit.Pages.Admin;

public class FileClassificationsShould
{
    [Fact]
    public void HaveTheAuthorizeAttribute()
        => typeof(FileClassifications)
            .GetCustomAttributes(typeof(AuthorizeAttribute), true)
            .ShouldNotBeEmpty();

    [Fact]
    public void HaveTheCorrectAdminPolicy()
        => typeof(FileClassifications)
            .GetCustomAttributes(typeof(AuthorizeAttribute), true).OfType<AuthorizeAttribute>().First(p => p.Policy != null).Policy
            .ShouldBe("AdminOnly");
}
