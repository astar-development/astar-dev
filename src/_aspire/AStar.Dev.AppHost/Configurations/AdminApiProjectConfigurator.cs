using AStar.Dev.Aspire.Common;
using Projects;

namespace AStar.Dev.AppHost.Configurations;

public static class AdminApiProjectConfigurator
{
    private const string HealthEndpoint = "/health";

    public record AdminApiProjectConfig(string ProjectName, string HealthEndpoint);

    // Pure logic: returns config

    public static AdminApiProjectConfig GetConfig() => new(AspireConstants.Apis.AdminApi, HealthEndpoint);

    // Orchestration: applies config to builder
    public static IResourceBuilder<ProjectResource> Configure(
        IDistributedApplicationBuilder builder,
        IResourceBuilder<SqlServerDatabaseResource> adminDb,
        IResourceBuilder<RabbitMQServerResource> rabbitMq)
    {
        var config = GetConfig();
        return builder.AddProject<AStar_Dev_Admin_Api>(config.ProjectName)
            .WithReference(adminDb)
            .WaitFor(adminDb)
            .WithReference(rabbitMq)
            .WaitFor(rabbitMq)
            .WithHttpHealthCheck(config.HealthEndpoint);
    }
}
