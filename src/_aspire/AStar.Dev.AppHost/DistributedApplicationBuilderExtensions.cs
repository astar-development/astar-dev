using AStar.Dev.Aspire.Common;
using Projects;

namespace AStar.Dev.AppHost;

public static class DistributedApplicationBuilderExtensions
{
    private const string HealthEndpoint = "/health";

    public static void AddApplicationProjects(this IDistributedApplicationBuilder distributedApplicationBuilder, string sqlMountDirectory)
    {
        var sqlPassword      = distributedApplicationBuilder.AddParameter("sql1-password", true);

        var sqlServer        = distributedApplicationBuilder.AddSqlServer(AspireConstants.Sql.SqlServer, sqlPassword, 1433)
                                                            .WithLifetime(ContainerLifetime.Persistent)
                                                            .WithExternalHttpEndpoints()
                                                            .WithDataBindMount(sqlMountDirectory);

        var adminDb = sqlServer.AddDatabase(AspireConstants.Sql.AdminDb);
        var filesDb = sqlServer.AddDatabase(AspireConstants.Sql.FilesDb);

        var rabbitmq  = AddRabbitMq(distributedApplicationBuilder);
        var adminApi  = AddAdminApi(distributedApplicationBuilder, adminDb, rabbitmq);
        var filesApi  = AddFilesApi(distributedApplicationBuilder, filesDb, rabbitmq);
        var imagesApi = AddImagesApi(distributedApplicationBuilder, rabbitmq);
        var usageApi  = AddUsageApi(distributedApplicationBuilder, rabbitmq);

        distributedApplicationBuilder.AddProject<AStar_Dev_FilesDb_MigrationService>(AspireConstants.Services.FileMigrations)
                                     .WithReference(filesDb)
                                     .WaitFor(filesDb)
                                     .WithParentRelationship(sqlServer);

        distributedApplicationBuilder.AddProject<AStar_Dev_Database_Updater>(AspireConstants.Services.DatabaseUpdater)
                                     .WithReference(filesDb)
                                     .WaitFor(filesDb)
                                     .WithParentRelationship(sqlServer);

        AddUi(distributedApplicationBuilder, adminApi, filesApi, imagesApi, usageApi, rabbitmq);
    }

    private static void AddUi(IDistributedApplicationBuilder    distributedApplicationBuilder,
                              IResourceBuilder<ProjectResource> adminApi,
                              IResourceBuilder<ProjectResource> filesApi,
                              IResourceBuilder<ProjectResource> imagesApi,
                              IResourceBuilder<ProjectResource> usageApi, IResourceBuilder<RabbitMQServerResource> rabbitmq) =>
        distributedApplicationBuilder.AddProject<AStar_Dev_Web>(AspireConstants.Ui)
                                     .WithExternalHttpEndpoints()
                                     .WithHttpHealthCheck(HealthEndpoint)
                                     .WithReference(rabbitmq)
                                     .WaitFor(rabbitmq)
                                     .WithReference(adminApi)
                                     .WaitFor(adminApi)
                                     .WithReference(filesApi)
                                     .WaitFor(filesApi)
                                     .WithReference(imagesApi)
                                     .WaitFor(imagesApi)
                                     .WithReference(usageApi)
                                     .WaitFor(usageApi);

    private static IResourceBuilder<ProjectResource> AddUsageApi(IDistributedApplicationBuilder distributedApplicationBuilder, IResourceBuilder<RabbitMQServerResource> rabbitmq) =>
        distributedApplicationBuilder.AddProject<AStar_Dev_Usage_Logger>(AspireConstants.Apis.UsageApi)
                                     .WithReference(rabbitmq)
                                     .WaitFor(rabbitmq)
                                     .WithHttpHealthCheck(HealthEndpoint);

    private static IResourceBuilder<ProjectResource> AddImagesApi(IDistributedApplicationBuilder distributedApplicationBuilder, IResourceBuilder<RabbitMQServerResource> rabbitmq) =>
        distributedApplicationBuilder.AddProject<AStar_Dev_Images_Api>(AspireConstants.Apis.ImagesApi)
                                     .WithReference(rabbitmq)
                                     .WaitFor(rabbitmq)
                                     .WithHttpHealthCheck(HealthEndpoint);

    private static IResourceBuilder<ProjectResource> AddFilesApi( IDistributedApplicationBuilder           distributedApplicationBuilder, IResourceBuilder<SqlServerDatabaseResource> filesDb,
                                                                  IResourceBuilder<RabbitMQServerResource> rabbitmq) =>
        distributedApplicationBuilder.AddProject<AStar_Dev_Files_Api>(AspireConstants.Apis.FilesApi)
                                     .WithReference(filesDb)
                                     .WaitFor(filesDb)
                                     .WithReference(rabbitmq)
                                     .WaitFor(rabbitmq)
                                     .WithHttpHealthCheck(HealthEndpoint);

    private static IResourceBuilder<ProjectResource> AddAdminApi( IDistributedApplicationBuilder           distributedApplicationBuilder, IResourceBuilder<SqlServerDatabaseResource> adminDb,
                                                                  IResourceBuilder<RabbitMQServerResource> rabbitmq) =>
        distributedApplicationBuilder.AddProject<AStar_Dev_Admin_Api>(AspireConstants.Apis.AdminApi)
                                     .WithReference(adminDb)
                                     .WaitFor(adminDb)
                                     .WithReference(rabbitmq)
                                     .WaitFor(rabbitmq)
                                     .WithHttpHealthCheck(HealthEndpoint);

    private static IResourceBuilder<RabbitMQServerResource> AddRabbitMq(IDistributedApplicationBuilder distributedApplicationBuilder) =>

        // var rabbitMqUsername = distributedApplicationBuilder.AddParameter("rabbitmq-username", true);
        // var rabbitMqPassword = distributedApplicationBuilder.AddParameter("rabbitmq-password", true);
        distributedApplicationBuilder.AddRabbitMQ(AspireConstants.Services.AstarMessaging)
                                     .WithLifetime(ContainerLifetime.Persistent)
                                     .WithManagementPlugin();
}
