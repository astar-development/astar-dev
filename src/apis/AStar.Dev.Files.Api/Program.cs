using System.IdentityModel.Tokens.Jwt;
using System.IO.Abstractions;
using System.Text.Json.Serialization;
using AStar.Dev.Api.Usage.Sdk;
using AStar.Dev.Api.Usage.Sdk.Metrics;
using AStar.Dev.Aspire.Common;
using AStar.Dev.AspNet.Extensions.PipelineExtensions;
using AStar.Dev.AspNet.Extensions.RootEndpoint;
using AStar.Dev.AspNet.Extensions.ServiceCollectionExtensions;
using AStar.Dev.AspNet.Extensions.WebApplicationBuilderExtensions;
using AStar.Dev.Auth.Extensions;
using AStar.Dev.Files.Api;
using AStar.Dev.Files.Api.Endpoints.Add.V1;
using AStar.Dev.Files.Api.Endpoints.Get.V1;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Logging.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Serilog;

var applicationName = typeof(IAssemblyMarker).Assembly.GetName().Name!;

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.AddServiceDefaults();

    _ = builder
        .DisableServerHeader()
        .AddSerilogLogging(Configuration.ExternalSettingsFile)
        .Services.AddApiConfiguration(builder.Configuration);

    Log.Information("Starting {AppName}", applicationName);
    var services = builder.Services;

    builder.AddSqlServerDbContext<FilesContext>(AspireConstants.Sql.FilesDb);

    services.AddUsageServices(builder.Configuration, typeof(IAssemblyMarker).Assembly);

    services.AddScoped<IFileSystem, FileSystem>();
    _ = services.AddScoped<JwtEvents>();

    builder.AddRabbitMQClient(AspireConstants.Services.AstarMessaging);
#pragma warning disable ASP0000
    var buildServiceProvider = services.BuildServiceProvider();
    var send                 = buildServiceProvider.GetRequiredService<Send>();
    var events               = buildServiceProvider.GetRequiredService<JwtEvents>();
#pragma warning restore ASP0000

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer("Bearer", jwtOptions =>
                                   {
                                       jwtOptions.MetadataAddress = "https://login.microsoftonline.com/bb7d94aa-36a9-4a59-a0c1-54a757c47ddf/v2.0/.well-known/openid-configuration";

                                       jwtOptions.TokenValidationParameters = new()
                                                                              {
                                                                                  ValidIssuer      = "https://sts.windows.net/bb7d94aa-36a9-4a59-a0c1-54a757c47ddf/",
                                                                                  ValidateIssuer   = true,
                                                                                  ValidateAudience = true,
                                                                                  ValidAudiences =
                                                                                  [
                                                                                      "api://11cbc21c-c65d-436e-951e-6b3158357be6",
                                                                                      "api://2ca26585-5929-4aae-86a7-a00c3fc2d061"
                                                                                  ],
                                                                                  ValidateIssuerSigningKey = true,
                                                                                  ValidateLifetime         = true,
                                                                                  ClockSkew                = TimeSpan.FromMinutes(3)
                                                                              };

                                       jwtOptions.MapInboundClaims = false;
                                       jwtOptions.Validate();
                                       jwtOptions.Events = events.AddJwtEvents(send, applicationName);
                                   });

    JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

    services.Configure<JsonOptions>(options =>
                                    {
                                        options.SerializerOptions.ReferenceHandler            = ReferenceHandler.IgnoreCycles;
                                        options.SerializerOptions.PropertyNameCaseInsensitive = true;
                                    });

    services.AddAuthorization();

    services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    services.AddScoped<IGetFilesHandler, GetFilesHandler>();

    var app = builder.Build()
                     .UseApiServices();

    app.ConfigureRootPage(applicationName.Replace(".", " "))
       .UseMetrics();

    app.MapDefaultEndpoints();

    app.MapFilesPostEndpoint();
    app.MapFilesGetEndpoint();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Error(ex, "Fatal error occurred in {AppName}", applicationName);
}
finally
{
    await Log.CloseAndFlushAsync();
}

namespace AStar.Dev.Files.Api
{
    internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
