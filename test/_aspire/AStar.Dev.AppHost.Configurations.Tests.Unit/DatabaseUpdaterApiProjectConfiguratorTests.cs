namespace AStar.Dev.AppHost.Configurations.Tests;

public class DatabaseUpdaterApiProjectConfiguratorTests
{
    [Fact]
    public void GetConfig_Should_Return_Expected_Values()
    {
        // Act
        var config = DatabaseUpdaterApiProjectConfigurator.GetConfig();

        // Assert
        config.ProjectName.ShouldBe("database-updater");
    }
}
