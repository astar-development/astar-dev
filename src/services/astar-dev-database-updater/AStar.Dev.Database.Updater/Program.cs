using AStar.Dev.Database.Updater;
using AStar.Dev.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
