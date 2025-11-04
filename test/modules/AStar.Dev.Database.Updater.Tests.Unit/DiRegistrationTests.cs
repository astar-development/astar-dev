using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
namespace AStar.Dev.Database.Updater.Tests.Unit;

public class DiRegistrationTests
{
    [Fact]
    public async Task AllServicesCanBeResolvedAsync()
    {
        var hostBuilder = Host.CreateApplicationBuilder().ConfigureApplicationServices();

        await using var provider = hostBuilder.Services.BuildServiceProvider(true);

        var count = 0;

        using var scope = provider.CreateScope();
        var sp = scope.ServiceProvider;

        foreach(var descriptor in hostBuilder.Services)
        {
            if(!descriptor.ServiceType.Namespace?.StartsWith("AStar.Dev") ?? true)
            {
                continue;
            }

            try
            {
                _ = sp.GetRequiredService(descriptor.ServiceType);
            }
            catch(Exception e) when(e is InvalidOperationException && e.Message.StartsWith("ConnectionString is missing."))
            {
                //
            }

            count++;
        }

        count.ShouldBeGreaterThanOrEqualTo(12);
    }
}
