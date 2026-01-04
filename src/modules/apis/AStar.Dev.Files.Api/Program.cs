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
using AStar.Dev.Files.Api;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Logging.Extensions;
using AStar.Dev.Web.Aspire.Common;
using AStar.Dev.Web.ServiceDefaults;
using Microsoft.AspNetCore.Http.Json;
using Serilog;

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

    builder.AddSqlServerDbContext<FilesContext>(AspireConstants.Sql.FilesDb);

    _ = services.AddUsageServices(builder.Configuration, typeof(IAssemblyMarker).Assembly);

    _ = services.AddScoped<IFileSystem, FileSystem>();
    _ = services.AddScoped<JwtEvents>();

    // #pragma warning disable ASP0000
    // var buildServiceProvider = services.BuildServiceProvider();
    // var send                 = buildServiceProvider.GetRequiredService<Send>();
    // var events               = buildServiceProvider.GetRequiredService<JwtEvents>();
    // #pragma warning restore ASP0000

    // builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    //        .AddJwtBearer("Bearer", jwtOptions =>
    //                                {
    //                                    jwtOptions.MetadataAddress = "https://login.microsoftonline.com/bb7d94aa-36a9-4a59-a0c1-54a757c47ddf/v2.0/.well-known/openid-configuration";
    //
    //                                    jwtOptions.TokenValidationParameters = new()
    //                                                                           {
    //                                                                               ValidIssuer      = "https://sts.windows.net/bb7d94aa-36a9-4a59-a0c1-54a757c47ddf/",
    //                                                                               ValidateIssuer   = true,
    //                                                                               ValidateAudience = true,
    //                                                                               ValidAudiences =
    //                                                                               [
    //                                                                                   "api://11cbc21c-c65d-436e-951e-6b3158357be6",
    //                                                                                   "api://2ca26585-5929-4aae-86a7-a00c3fc2d061",
    //                                                                               ],
    //                                                                               ValidateIssuerSigningKey = true,
    //                                                                               ValidateLifetime         = true,
    //                                                                               ClockSkew                = TimeSpan.FromMinutes(3),
    //                                                                           };
    //
    //                                    jwtOptions.MapInboundClaims = false;
    //                                    jwtOptions.Validate();
    //                                    jwtOptions.Events = events.AddJwtEvents(send, applicationName);
    //                                });

    JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

    _ = services.Configure<JsonOptions>(options =>
                                    {
                                        options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                                        options.SerializerOptions.PropertyNameCaseInsensitive = true;
                                    });

    // services.AddAuthorization();
    builder.AddRabbitMQClient(AspireConstants.Common.AstarMessaging);

    _ = services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

    WebApplication app = builder.Build()
                                .UseApiServices();

    app.ConfigureRootPage(applicationName.Replace(".", " "))
       .UseMetrics();

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
