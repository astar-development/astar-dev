using AStar.Dev.Test.DbContext.Helpers.Fixtures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AStar.Dev.Test.DbContext.Helpers;

public static class TestSetup
{
    static TestSetup()
    {
        var serviceCollection = new ServiceCollection();

        var configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile(
                                         "appsettings.json",
                                         true,
                                         true)
                            .AddUserSecrets<FilesContextFixture>()
                            .Build();

        serviceCollection.AddSingleton<IConfiguration>(configuration);

        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    public static ServiceProvider ServiceProvider { get; private set; }
}
