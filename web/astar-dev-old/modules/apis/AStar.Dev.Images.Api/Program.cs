using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.IO.Abstractions;
using System.Text.Json.Serialization;
using AStar.Dev.Api.Usage.Sdk;
using AStar.Dev.Api.Usage.Sdk.Metrics;
using AStar.Dev.AspNet.Extensions.PipelineExtensions;
using AStar.Dev.AspNet.Extensions.RootEndpoint;
using AStar.Dev.AspNet.Extensions.ServiceCollectionExtensions;
using AStar.Dev.AspNet.Extensions.WebApplicationBuilderExtensions;
using AStar.Dev.Auth.Extensions;
using AStar.Dev.Logging.Extensions;
using AStar.Dev.Web.Aspire.Common;
using AStar.Dev.Web.ServiceDefaults;
using Microsoft.AspNetCore.Http.Json;
using Serilog;

namespace AStar.Dev.Images.Api;

/// <summary>
///     The <see cref="Program" /> class used to start-up the API.
/// </summary>
[ExcludeFromCodeCoverage]
public static class Program
{
    /// <summary>
    ///     The Main method controls the startup process.
    /// </summary>
    /// <param name="args"></param>
    public static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        var applicationName = typeof(IAssemblyMarker).Assembly.GetName().Name!;

        try
        {
            _ = builder.AddServiceDefaults();

            _ = builder
               .DisableServerHeader()
               .AddSerilogLogging(Configuration.ExternalSettingsFile)
               .Services.AddApiConfiguration(builder.Configuration);

            // _ = builder.Configuration.AddUserSecrets<IAssemblyMarker>();
            Log.Information("Starting {AppName}", applicationName);

            IServiceCollection services = builder.Services;
            _ = services.AddScoped<JwtEvents>();
            _ = services.AddUsageServices(builder.Configuration, typeof(IAssemblyMarker).Assembly);
            _ = ConfigureServices(services);

            // #pragma warning disable ASP0000
            // var buildServiceProvider = services.BuildServiceProvider();
            // var send                 = buildServiceProvider.GetRequiredService<Send>();
            // var events               = buildServiceProvider.GetRequiredService<JwtEvents>();
            // #pragma warning restore ASP0000

            // services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //         .AddJwtBearer("Bearer", jwtOptions =>
            //                                 {
            //                                     jwtOptions.MetadataAddress = "https://login.microsoftonline.com/bb7d94aa-36a9-4a59-a0c1-54a757c47ddf/v2.0/.well-known/openid-configuration";
            //
            //                                     jwtOptions.TokenValidationParameters = new()
            //                                                                            {
            //                                                                                ValidIssuer      = "https://sts.windows.net/bb7d94aa-36a9-4a59-a0c1-54a757c47ddf/",
            //                                                                                ValidateIssuer   = true,
            //                                                                                ValidateAudience = true,
            //                                                                                ValidAudiences =
            //                                                                                [
            //                                                                                    "api://11cbc21c-c65d-436e-951e-6b3158357be6",
            //                                                                                    "api://2ca26585-5929-4aae-86a7-a00c3fc2d061",
            //                                                                                ],
            //                                                                                ValidateIssuerSigningKey = true,
            //                                                                                ValidateLifetime         = true,
            //                                                                                ClockSkew                = TimeSpan.FromMinutes(3),
            //                                                                            };
            //
            //                                     jwtOptions.MapInboundClaims = false;
            //                                     jwtOptions.Validate();
            //                                     jwtOptions.Events = events.AddJwtEvents(send, applicationName);
            //                                 });

            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            _ = services.Configure<JsonOptions>(options =>
                                            {
                                                options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                                                options.SerializerOptions.PropertyNameCaseInsensitive = true;
                                            });

            // services.AddAuthorization();
            builder.AddRabbitMQClient(AspireConstants.Common.AstarMessaging);
            _ = services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            _ = services.AddSingleton<IFileSystem, FileSystem>();

            WebApplication app = builder.Build()
                                        .UseApiServices();

            Log.Information("Starting {AppName}", applicationName);
            _ = ConfigurePipeline(app, applicationName);

            app.AddApplicationEndpoints();
            _ = app.MapDefaultEndpoints();

            await app.RunAsync();
        }
        catch(Exception ex)
        {
            Log.Error(ex, "Fatal error occurred in {AppName}", applicationName);
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    /// <summary>
    ///     Additional configuration can be performed here
    /// </summary>
    private static IServiceCollection ConfigureServices(IServiceCollection services)
        => services;

    private static WebApplication ConfigurePipeline(WebApplication app, string applicationName)
    {
        app.ConfigureRootPage(applicationName.Replace(".", " "))
           .UseMetrics();

        return app;
    }
}
