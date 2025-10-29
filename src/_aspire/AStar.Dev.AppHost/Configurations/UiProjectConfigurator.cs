using AStar.Dev.Aspire.Common;
using Projects;

namespace AStar.Dev.AppHost.Configurations;

public static class UiProjectConfigurator
{
    private const string HealthEndpoint1 = "/health";

    public record UiProjectConfig(string ProjectName, string HealthEndpoint);

    public static UiProjectConfig GetConfig() => new(AspireConstants.Ui, HealthEndpoint1);

    public static void Configure(
        IDistributedApplicationBuilder builder,
        IResourceBuilder<RabbitMQServerResource> rabbitMq)
    {
        var config = GetConfig();
        _ = builder.AddProject<AStar_Dev_Web>(config.ProjectName)
            .WithExternalHttpEndpoints()
            .WithHttpHealthCheck(config.HealthEndpoint)
            .WithReference(rabbitMq)
            .WaitFor(rabbitMq);
    }
}
