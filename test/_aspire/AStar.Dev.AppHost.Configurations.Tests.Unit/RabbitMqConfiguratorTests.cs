namespace AStar.Dev.AppHost.Configurations.Tests;

public class RabbitMqConfiguratorTests
{
    [Fact]
    public void GetConfig_Should_Return_Expected_Values()
    {
        // Act
        var config = RabbitMqConfigurator.GetConfig();

        // Assert
        config.ServiceName.ShouldBe("astar-dev-messaging");
    }
}
