using AStar.Dev.Aspire.Common;
using Projects;

namespace AStar.Dev.AppHost.Configurations;

public static class FileClassificationsApiProjectConfigurator
{
    private const string HealthEndpoint = "/health";

    public record FileClassificationsApiProjectConfig(string ProjectName, string HealthEndpoint);

    public static FileClassificationsApiProjectConfig GetConfig() => new(AspireConstants.Apis.FileClassificationsApi, HealthEndpoint);

    public static IResourceBuilder<ProjectResource> Configure(
        IDistributedApplicationBuilder builder,
        IResourceBuilder<SqlServerDatabaseResource> filesDb,
        IResourceBuilder<ProjectResource> migrations,
        IResourceBuilder<RabbitMQServerResource> rabbitMq)
    {
        var config = GetConfig();
        return builder.AddProject<AStar_Dev_Files_Classifications_Api>(config.ProjectName)
            .WithReference(filesDb)
            .WaitFor(filesDb)
            .WithReference(migrations)
            .WaitFor(migrations)
            .WithReference(rabbitMq)
            .WaitFor(rabbitMq)
            .WithHttpHealthCheck(config.HealthEndpoint);
    }
}
