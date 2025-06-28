using AStar.Dev.Aspire.Common;
using AStar.Dev.FilesDb.MigrationService;
using AStar.Dev.Infrastructure.FilesDb.Data;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();
builder.AddSqlServerDbContext<FilesContext>(AspireConstants.Sql.FilesDb);

var host = builder.Build();
host.Run();
