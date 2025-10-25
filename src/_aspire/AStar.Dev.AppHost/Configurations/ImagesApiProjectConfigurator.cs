using AStar.Dev.Aspire.Common;
using Projects;

namespace AStar.Dev.AppHost.Configurations;

public static class ImagesApiProjectConfigurator
{
    private const string HealthEndpoint = "/health";

    public record ImagesApiProjectConfig(string ProjectName, string HealthEndpoint);

    public static ImagesApiProjectConfig GetConfig() => new(AspireConstants.Apis.ImagesApi, HealthEndpoint);

    public static IResourceBuilder<ProjectResource> Configure(
        IDistributedApplicationBuilder builder,
        IResourceBuilder<RabbitMQServerResource> rabbitMq)
    {
        var config = GetConfig();
        return builder.AddProject<AStar_Dev_Images_Api>(config.ProjectName)
            .WithReference(rabbitMq)
            .WaitFor(rabbitMq)
            .WithHttpHealthCheck(config.HealthEndpoint);
    }
}
