using AStar.Dev.AppHost;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddApplicationProjects();

builder.Build().Run();
