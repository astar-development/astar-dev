using AStar.Dev.Aspire.Common;
using Projects;

namespace AStar.Dev.AppHost;

public static class DistributedApplicationBuilderExtensions
{
    private const string HealthEndpoint = "/health";

    public static void AddApplicationProjects(this IDistributedApplicationBuilder distributedApplicationBuilder, string sqlMountDirectory)
    {
        var sqlPassword = distributedApplicationBuilder.AddParameter(AspireConstants.Sql.SqlPasswordParameter, true);

        var sqlServer = distributedApplicationBuilder.AddSqlServer(AspireConstants.Sql.SqlServer, sqlPassword, 1433)
                                                     .WithLifetime(ContainerLifetime.Persistent)

                                                     //.WithDataBindMount(sqlMountDirectory)
                                                     .WithExternalHttpEndpoints();

        var adminDb = sqlServer.AddDatabase(AspireConstants.Sql.AdminDb);
        var filesDb = sqlServer.AddDatabase(AspireConstants.Sql.FilesDb);

        var migrations = distributedApplicationBuilder.AddProject<AStar_Dev_FilesDb_MigrationService>(AspireConstants.Services.FileMigrations)
                                                      .WithReference(filesDb)
                                                      .WaitFor(filesDb)
                                                      .WithParentRelationship(sqlServer);

        var rabbitMq  = AddRabbitMq(distributedApplicationBuilder);
        var adminApi  = AddAdminApi(distributedApplicationBuilder, adminDb, rabbitMq);
        var filesApi  = AddFilesApi(distributedApplicationBuilder, filesDb, migrations, rabbitMq);
        var imagesApi = AddImagesApi(distributedApplicationBuilder, rabbitMq);
        var usageApi  = AddUsageApi(distributedApplicationBuilder, rabbitMq);

        distributedApplicationBuilder.AddProject<AStar_Dev_Database_Updater>(AspireConstants.Services.DatabaseUpdater)
                                     .WithReference(migrations)
                                     .WaitFor(migrations)
                                     .WithReference(filesDb)
                                     .WaitFor(filesDb)
                                     .WithParentRelationship(sqlServer);

        AddUi(distributedApplicationBuilder, adminApi, filesApi, imagesApi, usageApi, rabbitMq);
    }

    private static void AddUi(IDistributedApplicationBuilder           distributedApplicationBuilder,
                              IResourceBuilder<ProjectResource>        adminApi,
                              IResourceBuilder<ProjectResource>        filesApi,
                              IResourceBuilder<ProjectResource>        imagesApi,
                              IResourceBuilder<ProjectResource>        usageApi,
                              IResourceBuilder<RabbitMQServerResource> rabbitMq) =>
        distributedApplicationBuilder.AddProject<AStar_Dev_Web>(AspireConstants.Ui)
                                     .WithExternalHttpEndpoints()
                                     .WithHttpHealthCheck(HealthEndpoint)
                                     .WithReference(rabbitMq)
                                     .WaitFor(rabbitMq)
                                     .WithReference(adminApi)
                                     .WaitFor(adminApi)
                                     .WithReference(filesApi)
                                     .WaitFor(filesApi)
                                     .WithReference(imagesApi)
                                     .WaitFor(imagesApi)
                                     .WithReference(usageApi)
                                     .WaitFor(usageApi);

    private static IResourceBuilder<ProjectResource> AddUsageApi(IDistributedApplicationBuilder distributedApplicationBuilder, IResourceBuilder<RabbitMQServerResource> rabbitMq) =>
        distributedApplicationBuilder.AddProject<AStar_Dev_Usage_Logger>(AspireConstants.Apis.UsageApi)
                                     .WithReference(rabbitMq)
                                     .WaitFor(rabbitMq)
                                     .WithHttpHealthCheck(HealthEndpoint);

    private static IResourceBuilder<ProjectResource> AddImagesApi(IDistributedApplicationBuilder distributedApplicationBuilder, IResourceBuilder<RabbitMQServerResource> rabbitMq) =>
        distributedApplicationBuilder.AddProject<AStar_Dev_Images_Api>(AspireConstants.Apis.ImagesApi)
                                     .WithReference(rabbitMq)
                                     .WaitFor(rabbitMq)
                                     .WithHttpHealthCheck(HealthEndpoint);

    private static IResourceBuilder<ProjectResource> AddFilesApi(IDistributedApplicationBuilder           distributedApplicationBuilder, IResourceBuilder<SqlServerDatabaseResource> filesDb,
                                                                 IResourceBuilder<ProjectResource>        migrations,
                                                                 IResourceBuilder<RabbitMQServerResource> rabbitMq) =>
        distributedApplicationBuilder.AddProject<AStar_Dev_Files_Api>(AspireConstants.Apis.FilesApi)
                                     .WithReference(filesDb)
                                     .WaitFor(filesDb)
                                     .WithReference(migrations)
                                     .WaitFor(migrations)
                                     .WithReference(rabbitMq)
                                     .WaitFor(rabbitMq)
                                     .WithHttpHealthCheck(HealthEndpoint);

    private static IResourceBuilder<ProjectResource> AddAdminApi(IDistributedApplicationBuilder           distributedApplicationBuilder, IResourceBuilder<SqlServerDatabaseResource> adminDb,
                                                                 IResourceBuilder<RabbitMQServerResource> rabbitMq) =>
        distributedApplicationBuilder.AddProject<AStar_Dev_Admin_Api>(AspireConstants.Apis.AdminApi)
                                     .WithReference(adminDb)
                                     .WaitFor(adminDb)
                                     .WithReference(rabbitMq)
                                     .WaitFor(rabbitMq)
                                     .WithHttpHealthCheck(HealthEndpoint);

    private static IResourceBuilder<RabbitMQServerResource> AddRabbitMq(IDistributedApplicationBuilder distributedApplicationBuilder) =>
        distributedApplicationBuilder.AddRabbitMQ(AspireConstants.Services.AstarMessaging)
                                     .WithLifetime(ContainerLifetime.Persistent)
                                     .WithManagementPlugin();
}
