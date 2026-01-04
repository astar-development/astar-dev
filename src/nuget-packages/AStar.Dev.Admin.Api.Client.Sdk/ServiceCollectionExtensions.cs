using AStar.Dev.Admin.Api.Client.Sdk.AdminApi;
using AStar.Dev.Api.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;

namespace AStar.Dev.Admin.Api.Client.Sdk;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static IServiceCollection AddAdminApiClient(this IServiceCollection services, IConfiguration configuration, ILogger<AdminApiClient> logger)
    {
        IConfigurationSection configurationSection = configuration.GetSection(AdminApiConfiguration.SectionLocation);

        _ = services.AddOptions<AdminApiConfiguration>()
                    .Bind(configurationSection)
                    .ValidateDataAnnotations()
                    .ValidateOnStart();

        services.AddScoped<IApiClient, AdminApiClient>();

        // _ = services.AddHttpClient<AdminApiClient>()
        //             .ConfigureHttpClient((serviceProvider, client) =>
        //                                  {
        //                                      client.BaseAddress = serviceProvider
        //                                                           .GetRequiredService<IOptions<AdminApiConfiguration>>().Value
        //                                                           .BaseUrl;
        //
        //                                      client.DefaultRequestHeaders.Accept.Add(
        //                                                                              new
        //                                                                                  MediaTypeWithQualityHeaderValue(MediaTypeNames.Application
        //                                                                                                                                .Json));
        //                                  });

        services.AddDownstreamApi(nameof(AdminApiClient), configuration.GetSection(AdminApiConfiguration.SectionLocation));

        return services;
    }
}
