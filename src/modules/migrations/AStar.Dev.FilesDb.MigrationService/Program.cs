using AStar.Dev.Aspire.Common;
using AStar.Dev.FilesDb.MigrationService;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.ServiceDefaults;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddHostedService<DatabaseMigrationService>();
builder.AddSqlServerDbContext<FilesContext>(AspireConstants.Sql.AStarDb);

IHost host = builder.Build();
await host.RunAsync();
