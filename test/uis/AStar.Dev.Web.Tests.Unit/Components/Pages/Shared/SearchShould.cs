using AngleSharp.Dom;
using AStar.Dev.Web.Components.Pages.Shared;
using Bunit;
using TestContext = Bunit.TestContext;

namespace AStar.Dev.Web.UI.Tests.Unit.Components.Pages.Shared;

public sealed class SearchShould : TestContext
{

    [Fact(Skip = "UI is different now")]
    public void RenderFileClassificationSelectBoxes()
    {
        // Arrange

        // Act
        IRenderedComponent<Search> cut = RenderComponent<Search>();

        // Assert
        cut.FindAll("fluent-select").Count.ShouldBeGreaterThan(0);
    }

    [Fact(Skip = "UI is different now")]
    public void RenderAndOrSwitch()
    {
        // Arrange

        // Act
        IRenderedComponent<Search> cut = RenderComponent<Search>();

        // Assert
        IRefreshableElementCollection<IElement> switches = cut.FindAll("fluent-switch");
        switches.ShouldContain(s => s.TextContent.Contains("AND") || s.TextContent.Contains("OR"));
    }

    [Fact(Skip = "UI is different now")]
    public void DisplayAndWhenSwitchIsOn()
    {
        // Arrange

        // Act
        IRenderedComponent<Search> cut = RenderComponent<Search>();

        // Assert
        cut.Markup.ShouldContain("AND");
    }

    [Fact(Skip = "UI is different now")]
    public Task LoadFileClassificationsOnInitializationAsync()
    {
        // Arrange

        // Act
        IRenderedComponent<Search> cut = RenderComponent<Search>();

        // Assert
        cut.Markup.ShouldContain("Documents");
        cut.Markup.ShouldContain("Images");
        cut.Markup.ShouldContain("Videos");
        return Task.CompletedTask;
    }

    [Fact(Skip = "UI is different now")]
    public void IncludePlaceholderInFileClassifications()
    {
        // Arrange

        // Act
        IRenderedComponent<Search> cut = RenderComponent<Search>();

        // Assert
        cut.Markup.ShouldContain("-- Select --");
    }
}
