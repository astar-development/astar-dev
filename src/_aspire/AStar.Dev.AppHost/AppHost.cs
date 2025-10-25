using AStar.Dev.AppHost.Configurations;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddApplicationProjects();

builder.Build().Run();

namespace AStar.Dev.AppHost
{
    public class AppHost;
}
