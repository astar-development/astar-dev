namespace AStar.Dev.AppHost.Configurations.Tests;

public class SqlServerConfiguratorTests
{
    [Fact]
    public void GetConfig_Should_Return_Expected_Values()
    {
        // Act
        var config = SqlServerConfigurator.GetConfig();

        // Assert
        config.ServerName.ShouldBe("sql1");
        config.Port.ShouldBe(1433);
    }
}
