using AStar.Dev.Test.Helpers.EndToEnd;
using AStar.Dev.Test.Helpers.EndToEnd.Models;
using Microsoft.Extensions.Options;
using Xunit.Abstractions;

namespace AStar.Dev.Web.UI.Features.HomePage;

[Binding]
public class StepDefinitions(IOptions<ApplicationSettings> applicationSettings, ITestOutputHelper output)
    : PlaywrightBase(applicationSettings, output)
{
    [Given("I have accessed the home page")]
    public async Task GivenIHaveAccessedTheHomePage()
    {
        var dummyHeaders = new Dictionary<string, string> { { "an-irrelevant-header", "an equally irrelevant value..." }, };
        Page = SetHeadersAndCreatePage(dummyHeaders);

        await Page.GotoAsync(AppSettings.BaseUri);
    }

    [When("I look at the page title")]
    public void WhenILookAtThePageTitle()
    {
        // NAR
    }

    [Then("I can see the expected title")]
    public async Task ThenICanSeeTheExpectedTitle()
    {
        var title = await Page.TitleAsync();

        title.ShouldContain("AStar Dev: Welcome!");
        await Page.ScreenshotAsync(new() { Path = "screenshot.png", FullPage = true, });
        await Task.Delay(5_000);
    }
}
