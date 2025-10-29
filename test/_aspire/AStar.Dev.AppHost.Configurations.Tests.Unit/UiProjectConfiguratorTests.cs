namespace AStar.Dev.AppHost.Configurations.Tests.Unit;

public class UiProjectConfiguratorTests
{
    [Fact]
    public void GetConfigShouldReturnExpectedValues()
    {
        // Act
        var config = UiProjectConfigurator.GetConfig();

        // Assert
        config.ProjectName.ShouldBe("ui");
        config.HealthEndpoint.ShouldBe("/health");
    }
}
