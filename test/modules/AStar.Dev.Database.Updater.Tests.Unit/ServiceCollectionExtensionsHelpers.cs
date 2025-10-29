using Microsoft.Extensions.DependencyInjection;

namespace AStar.Dev.Database.Updater.Tests.Unit;

public static class ServiceCollectionExtensionsHelpers
{
    public static void ShouldContainServiceType(this IServiceCollection services, Type serviceType)
        => services.Any(s => s.ServiceType == serviceType).ShouldBeTrue($"Service collection should contain service of type {serviceType.Name}");
}
