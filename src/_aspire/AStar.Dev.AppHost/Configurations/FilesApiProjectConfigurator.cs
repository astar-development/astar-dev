using AStar.Dev.Aspire.Common;
using Projects;

namespace AStar.Dev.AppHost.Configurations;

public static class FilesApiProjectConfigurator
{
    private const string HealthEndpoint = "/health";

    public record FilesApiProjectConfig(string ProjectName, string HealthEndpoint);

    public static FilesApiProjectConfig GetConfig() => new(AspireConstants.Apis.FilesApi, HealthEndpoint);

    public static IResourceBuilder<ProjectResource> Configure(
        IDistributedApplicationBuilder builder,
        IResourceBuilder<SqlServerDatabaseResource> filesDb,
        IResourceBuilder<ProjectResource> migrations,
        IResourceBuilder<RabbitMQServerResource> rabbitMq)
    {
        var config = GetConfig();
        return builder.AddProject<AStar_Dev_Files_Api>(config.ProjectName)
            .WithReference(filesDb)
            .WaitFor(filesDb)
            .WithReference(migrations)
            .WaitFor(migrations)
            .WithReference(rabbitMq)
            .WaitFor(rabbitMq)
            .WithHttpHealthCheck(config.HealthEndpoint);
    }
}
