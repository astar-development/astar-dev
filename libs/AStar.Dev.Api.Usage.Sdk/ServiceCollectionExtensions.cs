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
    /// <returns></returns>
    public static IServiceCollection AddUsageServices(this IServiceCollection services, ConfigurationManager configurationManager)
    {
        _ = services.AddSingleton<Send>();

        _ = services
           .AddOptions<ApiUsageConfiguration>()
           .Bind(configurationManager.GetSection(ApiUsageConfiguration.ConfigurationSectionName));

        return services;
    }
}
