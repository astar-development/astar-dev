using AStar.Dev.Aspire.Common;
using AStar.Dev.FilesDb.MigrationService;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.ServiceDefaults;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddHostedService<DatabaseMigrationService>();
builder.AddSqlServerDbContext<FilesContext>(AspireConstants.Sql.AStarDb);

var host = builder.Build();
await host.RunAsync();
