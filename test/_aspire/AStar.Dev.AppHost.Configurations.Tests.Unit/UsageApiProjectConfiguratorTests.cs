namespace AStar.Dev.AppHost.Configurations.Tests;

public class UsageApiProjectConfiguratorTests
{
    [Fact]
    public void GetConfig_Should_Return_Expected_Values()
    {
        // Act
        var config = UsageApiProjectConfigurator.GetConfig();

        // Assert
        config.ProjectName.ShouldBe("usage-api");
        config.HealthEndpoint.ShouldBe("/health");
    }
}
