using AStar.Dev.Web.UI.Models;
using Microsoft.Extensions.Options;
using Reqnroll.BoDi;

namespace AStar.Dev.Web.UI;

[Binding]
public sealed class Hooks
{
    public Hooks(IObjectContainer objectContainer)
    {
        var configuration = ConfigureConfiguration();
        var builder       = WebApplication.CreateBuilder();
        var services      = builder.Services;

        _ = services.AddHttpContextAccessor();
        services.AddOptions<UserDetails>().Bind(configuration.GetSection(UserDetails.ConfigurationSectionName));
        services.AddOptions<ApplicationSettings>().Bind(configuration.GetSection(ApplicationSettings.ConfigurationSectionName));
        var serviceProvider     = services.BuildServiceProvider();
        var userDetails         = serviceProvider.GetRequiredService<IOptions<UserDetails>>();
        var applicationSettings = serviceProvider.GetRequiredService<IOptions<ApplicationSettings>>();
        applicationSettings.Value.IsDevelopment = builder.Environment.IsDevelopment();
        objectContainer.RegisterInstanceAs(userDetails.Value);
        objectContainer.RegisterInstanceAs(applicationSettings);
    }

    // For additional details on Reqnroll hooks see https://go.reqnroll.net/doc-hooks
    private static IConfiguration ConfigureConfiguration()
    {
        var configurationBuilder = new ConfigurationBuilder();

        configurationBuilder
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appSettings.json")
           .AddUserSecrets<Hooks>();

        return configurationBuilder.Build();
    }

    [BeforeTestRun]
    public static void BeforeTestRun()
    {
        // runs before everything else - as the name / attribute suggests
    }

    [BeforeScenario("@tag1")]
    public void BeforeScenarioWithTag()
    {
        // Example of filtering hooks using tags. (in this case, this 'before scenario' hook will execute if the feature/scenario contains the tag '@tag1')
        // See https://go.reqnroll.net/doc-hooks#tag-scoping

        //TODO: implement logic that has to run before executing each scenario
    }

    [BeforeScenario(Order = 1)]
    public void FirstBeforeScenario()
    {
        // Example of ordering the execution of hooks
        // See https://go.reqnroll.net/doc-hooks#hook-execution-order

        //TODO: implement logic that has to run before executing each scenario
    }

    [AfterScenario]
    public void AfterScenario()
    {
        //TODO: implement logic that has to run after executing each scenario
    }
}
