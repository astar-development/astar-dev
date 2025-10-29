namespace AStar.Dev.AppHost.Configurations.Tests.Unit;

public class FilesApiProjectConfiguratorTests
{
    [Fact]
    public void GetConfigShouldReturnExpectedValues()
    {
        // Act
        var config = FilesApiProjectConfigurator.GetConfig();

        // Assert
        config.ProjectName.ShouldBe("files-api");
        config.HealthEndpoint.ShouldBe("/health");
    }
}
