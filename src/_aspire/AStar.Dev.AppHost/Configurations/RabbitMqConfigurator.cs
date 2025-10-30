using AStar.Dev.Aspire.Common;

namespace AStar.Dev.AppHost.Configurations;

public static class RabbitMqConfigurator
{
    public record RabbitMqConfig(string ServiceName);

    public static RabbitMqConfig GetConfig() => new(AspireConstants.Services.AstarMessaging);

    public static IResourceBuilder<RabbitMQServerResource> Configure(IDistributedApplicationBuilder builder)
    {
        RabbitMqConfig config = GetConfig();
        return builder.AddRabbitMQ(config.ServiceName)
            .WithLifetime(ContainerLifetime.Persistent)
            .WithManagementPlugin();
    }
}
