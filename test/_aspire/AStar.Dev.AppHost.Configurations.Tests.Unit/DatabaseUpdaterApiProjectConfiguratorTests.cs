namespace AStar.Dev.AppHost.Configurations.Tests.Unit;

public class DatabaseUpdaterApiProjectConfiguratorTests
{
    [Fact]
    public void GetConfigShouldReturnExpectedValues()
    {
        // Act
        var config = DatabaseUpdaterApiProjectConfigurator.GetConfig();

        // Assert
        config.ProjectName.ShouldBe("database-updater");
    }
}
