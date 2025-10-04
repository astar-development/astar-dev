using AStar.Dev.AppHost;
using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var sqlMountDirectory = builder.Configuration.GetValue<string>("applicationConfiguration:sqlServerMountDirectory")!;
builder.AddApplicationProjects(sqlMountDirectory);

builder.Build().Run();

namespace AStar.Dev.AppHost
{
    public class AppHost;
}
