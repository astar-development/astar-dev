namespace AStar.Dev.AppHost.Configurations.Tests.Unit;

public class FileClassificationsApiProjectConfiguratorTests
{
    [Fact]
    public void GetConfigShouldReturnExpectedValues()
    {
        // Act
        var config = FileClassificationsApiProjectConfigurator.GetConfig();

        // Assert
        config.ProjectName.ShouldBe("file-classifications-api");
        config.HealthEndpoint.ShouldBe("/health");
    }
}
