namespace AStar.Dev.AppHost.Configurations.Tests;

public class FileClassificationsApiProjectConfiguratorTests
{
    [Fact]
    public void GetConfig_Should_Return_Expected_Values()
    {
        // Act
        var config = FileClassificationsApiProjectConfigurator.GetConfig();

        // Assert
        config.ProjectName.ShouldBe("file-classifications-api");
        config.HealthEndpoint.ShouldBe("/health");
    }
}
