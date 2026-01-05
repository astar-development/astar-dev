using AStar.Dev.FilesDb.MigrationService;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Web.Aspire.Common;
using AStar.Dev.Web.ServiceDefaults;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();
builder.AddSqlServerDbContext<FilesContext>(AspireConstants.Sql.FilesDb);
builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
       .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

IHost host = builder.Build();
await host.RunAsync();
