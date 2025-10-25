namespace AStar.Dev.AppHost.Configurations.Tests;

public class ImagesApiProjectConfiguratorTests
{
    [Fact]
    public void GetConfig_Should_Return_Expected_Values()
    {
        // Act
        var config = ImagesApiProjectConfigurator.GetConfig();

        // Assert
        config.ProjectName.ShouldBe("images-api");
        config.HealthEndpoint.ShouldBe("/health");
    }
}
