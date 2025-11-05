using AStar.Dev.Aspire.Common;

namespace AStar.Dev.AppHost.Configurations;

public static class SqlServerConfigurator
{
    public record SqlServerConfig(string ServerName, int Port);

    public static SqlServerConfig GetConfig() => new(AspireConstants.Sql.SqlServer, 1433);

    public static IResourceBuilder<SqlServerServerResource> Configure(IDistributedApplicationBuilder builder, IResourceBuilder<ParameterResource> sqlPassword)
    {
        SqlServerConfig config = GetConfig();
        return builder.AddSqlServer(config.ServerName, sqlPassword, config.Port)
            .WithLifetime(ContainerLifetime.Persistent)
            .WithDataBindMount("/home/jason/databases")
            .WithExternalHttpEndpoints();
    }
}
