using AStar.Dev.Aspire.Common;
using Projects;

namespace AStar.Dev.AppHost.Configurations;

public static class UiProjectConfigurator
{
    private const string HealthEndpoint = "/health";

    public record UiProjectConfig(string ProjectName, string HealthEndpoint);

    public static UiProjectConfig GetConfig() => new(AspireConstants.Ui, HealthEndpoint);

    public static void Configure(
        IDistributedApplicationBuilder builder,
        IResourceBuilder<ProjectResource> adminApi,
        IResourceBuilder<ProjectResource> filesApi,
        IResourceBuilder<ProjectResource> imagesApi,
        IResourceBuilder<ProjectResource> usageApi,
        IResourceBuilder<ProjectResource> fileClassificationsApi,
        IResourceBuilder<RabbitMQServerResource> rabbitMq)
    {
        var config = GetConfig();
        builder.AddProject<AStar_Dev_Web>(config.ProjectName)
            .WithExternalHttpEndpoints()
            .WithHttpHealthCheck(config.HealthEndpoint)
            .WithReference(rabbitMq)
            .WaitFor(rabbitMq)
            .WithReference(adminApi)
            .WaitFor(adminApi)
            .WithReference(filesApi)
            .WaitFor(filesApi)
            .WithReference(fileClassificationsApi)
            .WaitFor(fileClassificationsApi)
            .WithReference(imagesApi)
            .WaitFor(imagesApi)
            .WithReference(usageApi)
            .WaitFor(usageApi);
    }
}
