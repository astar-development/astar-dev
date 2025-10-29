namespace AStar.Dev.AppHost.Configurations.Tests.Unit;

public class UsageApiProjectConfiguratorTests
{
    [Fact]
    public void GetConfigShouldReturnExpectedValues()
    {
        // Act
        var config = UsageApiProjectConfigurator.GetConfig();

        // Assert
        config.ProjectName.ShouldBe("usage-api");
        config.HealthEndpoint.ShouldBe("/health");
    }
}
