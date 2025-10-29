namespace AStar.Dev.AppHost.Configurations.Tests.Unit;

public class RabbitMqConfiguratorTests
{
    [Fact]
    public void GetConfigShouldReturnExpectedValues()
    {
        // Act
        var config = RabbitMqConfigurator.GetConfig();

        // Assert
        config.ServiceName.ShouldBe("astar-dev-messaging");
    }
}
