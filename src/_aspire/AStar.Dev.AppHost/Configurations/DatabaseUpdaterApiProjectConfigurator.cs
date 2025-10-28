using AStar.Dev.Aspire.Common;
using Projects;

namespace AStar.Dev.AppHost.Configurations;

public static class DatabaseUpdaterApiProjectConfigurator
{
    public record DatabaseUpdaterApiProjectConfig(string ProjectName);

    public static DatabaseUpdaterApiProjectConfig GetConfig() => new(AspireConstants.Services.DatabaseUpdater);

    public static void Configure(
        IDistributedApplicationBuilder builder,
        IResourceBuilder<SqlServerDatabaseResource> filesDb,
        IResourceBuilder<ProjectResource> migrations,
        IResourceBuilder<SqlServerServerResource> sqlServer,
        IResourceBuilder<ProjectResource> fileClassificationsApi,
        IResourceBuilder<RabbitMQServerResource> rabbitMq)
    {
        var config = GetConfig();
        builder.AddProject<AStar_Dev_Database_Updater>(config.ProjectName)
            .WithReference(filesDb)
            .WaitFor(filesDb)
            .WithReference(migrations)
            .WaitForCompletion(migrations)
            .WithReference(fileClassificationsApi)
            .WaitFor(fileClassificationsApi)
            .WithReference(rabbitMq)
            .WaitFor(rabbitMq)
            .WithParentRelationship(sqlServer);
    }
}
