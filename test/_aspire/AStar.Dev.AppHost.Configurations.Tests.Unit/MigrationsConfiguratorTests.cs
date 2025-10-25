namespace AStar.Dev.AppHost.Configurations.Tests;

public class MigrationsConfiguratorTests
{
    [Fact]
    public void GetConfig_Should_Return_Expected_Values()
    {
        // Act
        var config = MigrationsConfigurator.GetConfig();

        // Assert
        config.ProjectName.ShouldBe("file-migrations");
    }
}
