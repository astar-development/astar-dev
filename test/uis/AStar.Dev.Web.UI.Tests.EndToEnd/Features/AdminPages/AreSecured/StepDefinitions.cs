using AStar.Dev.Test.Helpers.EndToEnd;
using AStar.Dev.Test.Helpers.EndToEnd.Models;
using Microsoft.Extensions.Options;
using Xunit.Abstractions;

namespace AStar.Dev.Web.UI.Features.AdminPages.AreSecured;

[Binding]
public class StepDefinitions(IOptions<ApplicationSettings> applicationSettings, ITestOutputHelper output, ScenarioContext scenarioContext)
    : PlaywrightBase(applicationSettings, output)
{
    [Given("I am not logged in to the site")]
    public async Task GivenIAmNotLoggedInToTheSite() =>
        await Page.GotoAsync(AppSettings.BaseUri);

    [When("I visit any of the following pages:")]
    public void WhenIVisitAnyOfTheFollowingPages(Table table) =>
        scenarioContext["AdminPages"] = table.CreateSet<AdminPages>();

    [Then("I am asked to login")]
    public async Task ThenIAmAskedToLogin()
    {
        var adminPages = scenarioContext["AdminPages"] as List<AdminPages>;

        foreach (var url in adminPages!.Select(adminPage => $"{AppSettings.BaseUri}/admin/{adminPage.Url}"))
        {
            scenarioContext["Url"] = url;
            OutputHelper.WriteLine($"Redirecting to {url}");
            await Page.GotoAsync(url);
            var isVisible = await Page.GetByText("You need to Sign In!").IsVisibleAsync();
            isVisible.ShouldBeTrue();
        }
    }
}
