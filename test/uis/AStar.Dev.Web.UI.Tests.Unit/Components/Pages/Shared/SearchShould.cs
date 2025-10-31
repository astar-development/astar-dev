using AStar.Dev.Web.Components.Pages.Shared;
using Bunit;
using TestContext = Bunit.TestContext;

namespace AStar.Dev.Web.UI.Tests.Unit.Components.Pages.Shared;

public class SearchShould : TestContext
{

    [Fact(Skip = "UI is different now")]
    public void RenderFileClassificationSelectBoxes()
    {
        // Arrange

        // Act
        var cut = RenderComponent<Search>();

        // Assert
        cut.FindAll("fluent-select").Count.ShouldBeGreaterThan(0);
    }

    [Fact(Skip = "UI is different now")]
    public void RenderAndOrSwitch()
    {
        // Arrange

        // Act
        var cut = RenderComponent<Search>();

        // Assert
        var switches = cut.FindAll("fluent-switch");
        switches.ShouldContain(s => s.TextContent.Contains("AND") || s.TextContent.Contains("OR"));
    }

    [Fact(Skip = "UI is different now")]
    public void DisplayAndWhenSwitchIsOn()
    {
        // Arrange

        // Act
        var cut = RenderComponent<Search>();

        // Assert
        cut.Markup.ShouldContain("AND");
    }

    [Fact(Skip = "UI is different now")]
    public async Task LoadFileClassificationsOnInitializationAsync()
    {
        // Arrange

        // Act
        var cut = RenderComponent<Search>();

        // Assert
        cut.Markup.ShouldContain("Documents");
        cut.Markup.ShouldContain("Images");
        cut.Markup.ShouldContain("Videos");
    }

    [Fact(Skip = "UI is different now")]
    public void IncludePlaceholderInFileClassifications()
    {
        // Arrange

        // Act
        var cut = RenderComponent<Search>();

        // Assert
        cut.Markup.ShouldContain("-- Select --");
    }
}
