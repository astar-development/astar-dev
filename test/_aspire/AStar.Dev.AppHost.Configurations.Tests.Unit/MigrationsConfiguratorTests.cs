namespace AStar.Dev.AppHost.Configurations.Tests.Unit;

public class MigrationsConfiguratorTests
{
    [Fact]
    public void GetConfigShouldReturnExpectedValues()
    {
        // Act
        var config = MigrationsConfigurator.GetConfig();

        // Assert
        config.ProjectName.ShouldBe("file-migrations");
    }
}
