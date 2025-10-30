using AStar.Dev.Aspire.Common;
using Projects;

namespace AStar.Dev.AppHost.Configurations;

public static class MigrationsConfigurator
{
    public record MigrationsConfig(string ProjectName);

    public static MigrationsConfig GetConfig() => new(AspireConstants.Services.FileMigrations);

    public static IResourceBuilder<ProjectResource> Configure(
        IDistributedApplicationBuilder              builder,
        IResourceBuilder<SqlServerDatabaseResource> filesDb,
        IResourceBuilder<SqlServerServerResource>   sqlServer, IResourceBuilder<ParameterResource> sqlSaUserPassword, IResourceBuilder<ParameterResource> sqlAdminUserPassword, IResourceBuilder<ParameterResource> sqlFilesUserPassword,
        IResourceBuilder<ParameterResource>         sqlUsageUserPassword)
    {
        MigrationsConfig config = GetConfig();
        return builder.AddProject<AStar_Dev_FilesDb_MigrationService>(config.ProjectName)
                      .WithReference(filesDb)
                      .WithEnvironment(AspireConstants.Sql.SqlSaUserPasswordParameter,    sqlSaUserPassword)
                      .WithEnvironment(AspireConstants.Sql.SqlAdminUserPasswordParameter, sqlAdminUserPassword)
                      .WithEnvironment(AspireConstants.Sql.SqlFilesUserPasswordParameter, sqlFilesUserPassword)
                      .WithEnvironment(AspireConstants.Sql.SqlUsageUserPasswordParameter, sqlUsageUserPassword)
            .WaitFor(filesDb)
            .WithParentRelationship(sqlServer);
    }
}
