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

        DatabaseUpdaterApiProjectConfigurator.Configure(builder, astarDb, migrations, sqlServer, rabbitMq);

        UiProjectConfigurator.Configure(builder, rabbitMq);
    }
}
