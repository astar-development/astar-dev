using AStar.Dev.Files.Api.Client.SDK.FilesApi;
using AStar.Dev.Files.Api.Client.SDK.Models;
using AStar.Dev.Web.Components.Pages.Shared;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using TestContext = Bunit.TestContext;

namespace AStar.Dev.Web.UI.Tests.Unit.Components.Pages.Shared;

public class SearchShould : TestContext
{
    private readonly FilesApiClient _mockFilesApiClient;

    public SearchShould()
    {
        var httpClient = Substitute.For<HttpClient>();
        var logger = Substitute.For<ILogger<FilesApiClient>>();
        _mockFilesApiClient = Substitute.For<FilesApiClient>(httpClient, logger);
    }

    [Fact(Skip = "UI is different now")]
    public void RenderFileClassificationSelectBoxes()
    {
        // Arrange
        var classifications = new List<FileClassification>
        {
            new() { Id = Guid.NewGuid(), Name = "Classification 1", IncludeInSearch = true, Celebrity = false },
            new() { Id = Guid.NewGuid(), Name = "Classification 2", IncludeInSearch = true, Celebrity = false }
        };

        _mockFilesApiClient.GetFileClassificationsAsync().Returns(Task.FromResult<IReadOnlyCollection<FileClassification>>(classifications));
        Services.AddSingleton(_mockFilesApiClient);

        // Act
        var cut = RenderComponent<Search>();

        // Assert
        cut.FindAll("fluent-select").Count.ShouldBeGreaterThan(0);
    }

    [Fact(Skip = "UI is different now")]
    public void RenderAndOrSwitch()
    {
        // Arrange
        var classifications = new List<FileClassification>();
        _mockFilesApiClient.GetFileClassificationsAsync().Returns(Task.FromResult<IReadOnlyCollection<FileClassification>>(classifications));
        Services.AddSingleton(_mockFilesApiClient);

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
        var classifications = new List<FileClassification>();
        _mockFilesApiClient.GetFileClassificationsAsync().Returns(Task.FromResult<IReadOnlyCollection<FileClassification>>(classifications));
        Services.AddSingleton(_mockFilesApiClient);

        // Act
        var cut = RenderComponent<Search>();

        // Assert
        cut.Markup.ShouldContain("AND");
    }

    [Fact(Skip = "UI is different now")]
    public async Task LoadFileClassificationsOnInitializationAsync()
    {
        // Arrange
        var classifications = new List<FileClassification>
        {
            new() { Id = Guid.NewGuid(), Name = "Documents", IncludeInSearch = true, Celebrity = false },
            new() { Id = Guid.NewGuid(), Name = "Images", IncludeInSearch = true, Celebrity = false },
            new() { Id = Guid.NewGuid(), Name = "Videos", IncludeInSearch = false, Celebrity = true }
        };

        _mockFilesApiClient.GetFileClassificationsAsync().Returns(Task.FromResult<IReadOnlyCollection<FileClassification>>(classifications));
        Services.AddSingleton(_mockFilesApiClient);

        // Act
        var cut = RenderComponent<Search>();

        // Assert
        await cut.InvokeAsync(async () =>
        {
            await _mockFilesApiClient.Received(1).GetFileClassificationsAsync();
        });

        cut.Markup.ShouldContain("Documents");
        cut.Markup.ShouldContain("Images");
        cut.Markup.ShouldContain("Videos");
    }

    [Fact(Skip = "UI is different now")]
    public void IncludePlaceholderInFileClassifications()
    {
        // Arrange
        var classifications = new List<FileClassification> { new() { Id = Guid.NewGuid(), Name = "Type1", IncludeInSearch = true, Celebrity = false } };

        _mockFilesApiClient.GetFileClassificationsAsync().Returns(Task.FromResult<IReadOnlyCollection<FileClassification>>(classifications));
        Services.AddSingleton(_mockFilesApiClient);

        // Act
        var cut = RenderComponent<Search>();

        // Assert
        cut.Markup.ShouldContain("-- Select --");
    }
}
