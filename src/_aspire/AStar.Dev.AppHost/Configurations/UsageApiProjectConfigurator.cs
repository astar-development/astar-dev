using AStar.Dev.Aspire.Common;
using Projects;

namespace AStar.Dev.AppHost.Configurations;

public static class UsageApiProjectConfigurator
{
    private const string HealthEndpoint = "/health";

    public record UsageApiProjectConfig(string ProjectName, string HealthEndpoint);

    public static UsageApiProjectConfig GetConfig() => new(AspireConstants.Apis.UsageApi, HealthEndpoint);

    public static IResourceBuilder<ProjectResource> Configure(
        IDistributedApplicationBuilder builder,
        IResourceBuilder<RabbitMQServerResource> rabbitMq)
    {
        var config = GetConfig();
        return builder.AddProject<AStar_Dev_Usage_Logger>(config.ProjectName)
            .WithReference(rabbitMq)
            .WaitFor(rabbitMq)
            .WithHttpHealthCheck(config.HealthEndpoint);
    }
}
