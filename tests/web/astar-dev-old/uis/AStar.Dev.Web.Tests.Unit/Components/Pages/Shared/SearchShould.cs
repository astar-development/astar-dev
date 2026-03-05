using AStar.Dev.Web.Components.Pages.Shared;
using Bunit;

namespace AStar.Dev.Web.Tests.Unit.Components.Pages.Shared;

public sealed class SearchShould : BunitContext
{

    [Fact(Skip = "UI is different now")]
    public void RenderFileClassificationSelectBoxes()
    {
        // Arrange

        // Act
        IRenderedComponent<Search> cut = Render<Search>();

        // Assert
        cut.FindAll("fluent-select").Count.ShouldBeGreaterThan(0);
    }

    [Fact(Skip = "UI is different now")]
    public void DisplayAndWhenSwitchIsOn()
    {
        // Arrange

        // Act
        IRenderedComponent<Search> cut = Render<Search>();

        // Assert
        cut.Markup.ShouldContain("AND");
    }

    [Fact(Skip = "UI is different now")]
    public Task LoadFileClassificationsOnInitializationAsync()
    {
        // Arrange

        // Act
        IRenderedComponent<Search> cut = Render<Search>();

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
        IRenderedComponent<Search> cut = Render<Search>();

        // Assert
        cut.Markup.ShouldContain("-- Select --");
    }
}
