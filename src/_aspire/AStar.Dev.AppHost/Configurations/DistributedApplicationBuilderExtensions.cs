using AStar.Dev.Aspire.Common;

namespace AStar.Dev.AppHost.Configurations;

public static class DistributedApplicationBuilderExtensions
{
    public static void AddApplicationProjects(this IDistributedApplicationBuilder builder)
    {
        IResourceBuilder<ParameterResource> sqlSaUserPassword    = builder.AddParameter(AspireConstants.Sql.SqlSaUserPasswordParameter,    true);
        IResourceBuilder<ParameterResource> sqlAdminUserPassword = builder.AddParameter(AspireConstants.Sql.SqlAdminUserPasswordParameter, true);
        IResourceBuilder<ParameterResource> sqlFilesUserPassword = builder.AddParameter(AspireConstants.Sql.SqlFilesUserPasswordParameter, true);
        IResourceBuilder<ParameterResource> sqlUsageUserPassword = builder.AddParameter(AspireConstants.Sql.SqlUsageUserPasswordParameter, true);
        IResourceBuilder<SqlServerServerResource> sqlServer            = SqlServerConfigurator.Configure(builder, sqlSaUserPassword);
        IResourceBuilder<SqlServerDatabaseResource> astarDb              = sqlServer.AddDatabase(AspireConstants.Sql.AStarDb);
        IResourceBuilder<ProjectResource> migrations           = MigrationsConfigurator.Configure(builder, astarDb, sqlServer, sqlSaUserPassword, sqlAdminUserPassword, sqlFilesUserPassword, sqlUsageUserPassword);
        IResourceBuilder<RabbitMQServerResource> rabbitMq             = RabbitMqConfigurator.Configure(builder);
        DatabaseUpdaterApiProjectConfigurator.Configure(builder, astarDb, migrations, sqlServer, rabbitMq);

        UiProjectConfigurator.Configure(builder, astarDb, rabbitMq);
    }
}
