using AStar.Dev.Aspire.Common;
using Projects;

namespace AStar.Dev.AppHost.Configurations;

public static class DatabaseUpdaterApiProjectConfigurator
{
    public static void Configure(
        IDistributedApplicationBuilder builder,
        IResourceBuilder<SqlServerDatabaseResource> filesDb,
        IResourceBuilder<ProjectResource> migrations,
        IResourceBuilder<SqlServerServerResource> sqlServer,
        IResourceBuilder<RabbitMQServerResource> rabbitMq) =>
        _ = builder.AddProject<AStar_Dev_Database_Updater>(AspireConstants.Services.DatabaseUpdater)
            .WithReference(filesDb)
            .WaitFor(filesDb)
            .WithReference(migrations)
            .WaitForCompletion(migrations)
            .WithReference(rabbitMq)
            .WaitFor(rabbitMq)
            .WithParentRelationship(sqlServer);
}
