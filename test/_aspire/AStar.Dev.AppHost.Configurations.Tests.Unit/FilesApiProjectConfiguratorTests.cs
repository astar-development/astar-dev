namespace AStar.Dev.AppHost.Configurations.Tests;

public class FilesApiProjectConfiguratorTests
{
    [Fact]
    public void GetConfig_Should_Return_Expected_Values()
    {
        // Act
        var config = FilesApiProjectConfigurator.GetConfig();

        // Assert
        config.ProjectName.ShouldBe("files-api");
        config.HealthEndpoint.ShouldBe("/health");
    }
}
