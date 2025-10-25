namespace AStar.Dev.AppHost.Configurations.Tests;

public class UiProjectConfiguratorTests
{
    [Fact]
    public void GetConfig_Should_Return_Expected_Values()
    {
        // Act
        var config = UiProjectConfigurator.GetConfig();

        // Assert
        config.ProjectName.ShouldBe("ui");
        config.HealthEndpoint.ShouldBe("/health");
    }
}
