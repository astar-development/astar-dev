using AStar.Dev.Aspire.Common;

namespace AStar.Dev.AppHost.Configurations;

public static class DistributedApplicationBuilderExtensions
{
    public static void AddApplicationProjects(this IDistributedApplicationBuilder builder)
    {
        var sqlPassword = builder.AddParameter(AspireConstants.Sql.SqlPasswordParameter, true);
        var sqlServer = SqlServerConfigurator.Configure(builder, sqlPassword);
        var adminDb = sqlServer.AddDatabase(AspireConstants.Sql.AdminDb);
        var filesDb = sqlServer.AddDatabase(AspireConstants.Sql.FilesDb);
        var migrations = MigrationsConfigurator.Configure(builder, filesDb, sqlServer);
        var rabbitMq = RabbitMqConfigurator.Configure(builder);

        var adminApi = AdminApiProjectConfigurator.Configure(builder, adminDb, rabbitMq);
        var filesApi = FilesApiProjectConfigurator.Configure(builder, filesDb, migrations, rabbitMq);
        var fileClassificationsApi = FileClassificationsApiProjectConfigurator.Configure(builder, filesDb, migrations, rabbitMq);
        var imagesApi = ImagesApiProjectConfigurator.Configure(builder, rabbitMq);
        var usageApi = UsageApiProjectConfigurator.Configure(builder, rabbitMq);
        DatabaseUpdaterApiProjectConfigurator.Configure(builder, filesDb, migrations, sqlServer, fileClassificationsApi, rabbitMq);

        UiProjectConfigurator.Configure(builder, adminApi, filesApi, imagesApi, usageApi, fileClassificationsApi, rabbitMq);
    }
}
