namespace AStar.Dev.AppHost.Configurations.Tests.Unit;

public class SqlServerConfiguratorTests
{
    [Fact]
    public void GetConfigShouldReturnExpectedValues()
    {
        // Act
        var config = SqlServerConfigurator.GetConfig();

        // Assert
        config.ServerName.ShouldBe("sql1");
        config.Port.ShouldBe(1433);
    }
}
