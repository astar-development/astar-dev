using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AStar.Dev.Api.Usage.Sdk;

/// <summary>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configurationManager"></param>
    /// <param name="executingAssembly"></param>
    /// <returns></returns>
    public static IServiceCollection AddUsageServices(this IServiceCollection services, ConfigurationManager configurationManager, Assembly executingAssembly)
    {
        // services.AddMediatR(cfg =>
        //                     {
        //                         cfg.RegisterServicesFromAssembly(executingAssembly)
        //                            .AddOpenBehavior(typeof(UsageMetricHandler<,>));
        //                     });

        services.AddSingleton<Send>();

        services
           .AddOptions<ApiUsageConfiguration>()
           .Bind(configurationManager.GetSection(ApiUsageConfiguration.ConfigurationSectionName));

        return services;
    }
}
