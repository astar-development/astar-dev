using AStar.Dev.Test.DbContext.Helpers.Fixtures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AStar.Dev.Test.DbContext.Helpers;

/// <summary>
/// </summary>
public static class TestSetup
{
    static TestSetup()
    {
        var serviceCollection = new ServiceCollection();

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(
                "appsettings.json",
                true,
                true)
            .AddUserSecrets<FilesContextFixture>()
            .Build();

        _ = serviceCollection.AddSingleton<IConfiguration>(configuration);

        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    /// <summary>
    /// </summary>
    public static ServiceProvider ServiceProvider { get; private set; }
}
