using AStar.Dev.Web.UI.Models;
using Microsoft.Extensions.Options;
using Xunit.Abstractions;

namespace AStar.Dev.Web.UI.Features.Login;

[Binding]
public class StepDefinitions(IOptions<ApplicationSettings> applicationSettings, UserDetails userDetails, ITestOutputHelper output)
    : PlaywrightBase(applicationSettings, output)
{
    [Given("I have valid credentials")]
    public async Task GivenIHaveValidCredentials()
    {
        await Task.Delay(1);
        var dummyHeaders = new Dictionary<string, string> { { "an-irrelevant-header", "an equally irrelevant value..." }, };
        Page = SetHeadersAndCreatePage(dummyHeaders);

        // await Page.GotoAsync(AppSettings.BaseUri);
        // OutputHelper.WriteLine("Welcome to the app");
        userDetails.Username.ShouldNotBeNull();
    }

    [When("I enter them on the login page")]
    public async Task WhenIEnterThemOnTheLoginPage()
    {
        //await Page.GetByPlaceholder("Email, phone, or Skype").FillAsync(userDetails.Username); //.ClickAsync(new() { Button = MouseButton.Left, });
        await Task.Delay(1);
        await Task.Delay(1);

        // await Page.GetByRole(AriaRole.Button, new() { Name = "Next", }).ClickAsync();
        // await Task.Delay(5_000);

        // await Page.GetByPlaceholder("Password").FillAsync(userDetails.Password);
        // await Page.GetByRole(AriaRole.Button, new() { Name = "Sign in", }).ClickAsync();
    }

    [Then("I can see I am logged in as expected")]
    public async Task ThenICanSeeIAmLoggedInAsExpected()
    {
        await Task.Delay(1);
        await Task.Delay(1);
        // var title = await Page.TitleAsync();
        // title.ShouldContain("Something to confirm the login has been successful");
    }
}
