namespace AStar.Dev.AppHost.Configurations.Tests;

public class AdminApiProjectConfiguratorTests
{
    [Fact]
    public void GetConfig_Should_Return_Expected_Values()
    {
        // Act
        var config = AdminApiProjectConfigurator.GetConfig();

        // Assert
        config.ProjectName.ShouldBe("admin-api");
        config.HealthEndpoint.ShouldBe("/health");
    }
}
