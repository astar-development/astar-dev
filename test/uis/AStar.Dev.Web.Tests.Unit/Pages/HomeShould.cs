using AStar.Dev.Web.Components.Pages;
using Bunit;
using Microsoft.AspNetCore.Authorization;
using TestContext = Bunit.TestContext;

namespace AStar.Dev.Web.Tests.Unit.Pages;

public class HomeShould : TestContext
{
    [Fact]
    public void HaveTheAllowAnonymousAttribute()
        => typeof(Home)
            .GetCustomAttributes(typeof(AllowAnonymousAttribute), true)
            .ShouldNotBeEmpty();

    [Fact]
    public void ContainTheWelcomeHeading()
    {
        var cut = RenderComponent<Home>();

        var heading = cut.Find("h1");
        Assert.Equal("Welcome to the AStar Development website!", heading.TextContent);
    }

    [Fact]
    public void ContainTheFluentUiLink()
    {
        var cut = RenderComponent<Home>();

        var link = cut.Find("a[href='https://www.fluentui-blazor.net']");

        Assert.Equal("_blank", link.GetAttribute("target"));
        Assert.Equal("Microsoft Fluent UI Blazor library", link.TextContent);
    }

    [Fact]
    public void ContainTheEnhancementsList() => RenderComponent<Home>().FindAll("ul > li").Count.ShouldBeGreaterThanOrEqualTo(3);
}
