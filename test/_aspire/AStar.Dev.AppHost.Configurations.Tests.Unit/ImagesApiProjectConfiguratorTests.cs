namespace AStar.Dev.AppHost.Configurations.Tests.Unit;

public class ImagesApiProjectConfiguratorTests
{
    [Fact]
    public void GetConfigShouldReturnExpectedValues()
    {
        // Act
        var config = ImagesApiProjectConfigurator.GetConfig();

        // Assert
        config.ProjectName.ShouldBe("images-api");
        config.HealthEndpoint.ShouldBe("/health");
    }
}
