using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using AStar.Dev.Api.Usage.Sdk;
using AStar.Dev.Api.Usage.Sdk.Metrics;
using AStar.Dev.AspNet.Extensions.PipelineExtensions;
using AStar.Dev.AspNet.Extensions.RootEndpoint;
using AStar.Dev.AspNet.Extensions.ServiceCollectionExtensions;
using AStar.Dev.AspNet.Extensions.WebApplicationBuilderExtensions;
using AStar.Dev.Auth.Extensions;
using AStar.Dev.Infrastructure.Data;
using AStar.Dev.Infrastructure.UsageDb.Data;
using AStar.Dev.Logging.Extensions;
using AStar.Dev.Usage.Logger;
using AStar.Dev.Web.Aspire.Common;
using AStar.Dev.Web.ServiceDefaults;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

var applicationName = typeof(IAssemblyMarker).Assembly.GetName().Name!;

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    _ = builder.AddServiceDefaults();

    _ = builder
       .DisableServerHeader()
       .AddSerilogLogging(Configuration.ExternalSettingsFile)
       .Services.AddApiConfiguration(builder.Configuration);

    Log.Information("Starting {AppName}", applicationName);
    IServiceCollection services = builder.Services;

    var connectionString = builder.Configuration.GetConnectionString("sqlServer")!;

    _ = builder.Services.AddScoped<JwtEvents>();
    _ = builder.Services.AddSingleton(_ => new ApiUsageContext(new ConnectionString { Value = connectionString, }));
    _ = builder.Services.AddTransient<IPeriodicTimer, HalfSecondPeriodicTimer>();
    _ = builder.Services.Configure<HostOptions>(options => options.ServicesStartConcurrently = true);
    _ = builder.Services.AddHostedService<ProcessUsageEventsService>();

    _ = services.AddUsageServices(builder.Configuration, typeof(IAssemblyMarker).Assembly);

    JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

    _ = services.Configure<JsonOptions>(options =>
                                    {
                                        options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                                        options.SerializerOptions.PropertyNameCaseInsensitive = true;
                                    });

    builder.AddRabbitMQClient(AspireConstants.Common.AstarMessaging);

    _ = services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

    WebApplication app = builder.Build()
                                .UseApiServices();

    app.ConfigureRootPage(applicationName.Replace(".", " "))
       .UseMetrics();

    app.AddApplicationEndpoints();

    _ = app.MapGet("/process-events",
               async ([FromServices] ProcessUsageEventsService process, CancellationToken stoppingToken) =>
               {
                   await process.ProcessEventsAsync(stoppingToken);

                   return Results.Ok();
               })
       .WithName("ProcessEvents");

    _ = app.MapDefaultEndpoints();

    await app.RunAsync();
}
catch(Exception ex)
{
    Log.Fatal(ex, "Fatal error occurred in {AppName}", applicationName);
}
finally
{
    await Log.CloseAndFlushAsync();
}
