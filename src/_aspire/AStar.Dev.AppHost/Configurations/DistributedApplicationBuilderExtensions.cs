using AStar.Dev.Aspire.Common;

namespace AStar.Dev.AppHost.Configurations;

public static class DistributedApplicationBuilderExtensions
{
    public static void AddApplicationProjects(this IDistributedApplicationBuilder builder)
    {
        var sqlSaUserPassword    = builder.AddParameter(AspireConstants.Sql.SqlSaUserPasswordParameter,    true);
        var sqlAdminUserPassword = builder.AddParameter(AspireConstants.Sql.SqlAdminUserPasswordParameter, true);
        var sqlFilesUserPassword = builder.AddParameter(AspireConstants.Sql.SqlFilesUserPasswordParameter, true);
        var sqlUsageUserPassword = builder.AddParameter(AspireConstants.Sql.SqlUsageUserPasswordParameter, true);
        var sqlServer            = SqlServerConfigurator.Configure(builder, sqlSaUserPassword);
        var astarDb              = sqlServer.AddDatabase(AspireConstants.Sql.AStarDb);
        var migrations           = MigrationsConfigurator.Configure(builder, astarDb, sqlServer, sqlSaUserPassword, sqlAdminUserPassword, sqlFilesUserPassword, sqlUsageUserPassword);
        var rabbitMq             = RabbitMqConfigurator.Configure(builder);

        var adminApi               = AdminApiProjectConfigurator.Configure(builder, astarDb, rabbitMq);
        var filesApi               = FilesApiProjectConfigurator.Configure(builder, astarDb, migrations, rabbitMq);
        var fileClassificationsApi = FileClassificationsApiProjectConfigurator.Configure(builder, astarDb, migrations, rabbitMq);
        var imagesApi              = ImagesApiProjectConfigurator.Configure(builder, rabbitMq);
        var usageApi               = UsageApiProjectConfigurator.Configure(builder, rabbitMq);
        DatabaseUpdaterApiProjectConfigurator.Configure(builder, astarDb, migrations, sqlServer, fileClassificationsApi, rabbitMq);

        UiProjectConfigurator.Configure(builder, adminApi, filesApi, imagesApi, usageApi, fileClassificationsApi, rabbitMq);
    }
}
