using AStar.Dev.Test.Helpers.EndToEnd.Models;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using Xunit.Abstractions;

namespace AStar.Dev.Test.Helpers.EndToEnd;

/// <summary>
///     The abstract <see cref="PlaywrightBase" /> class supplies some default functionality that can be used by the tests within your project
/// </summary>
public abstract class PlaywrightBase
{
    private readonly IPlaywright playwright = Playwright.CreateAsync().Result;

    /// <summary>
    ///     The default constructor
    /// </summary>
    /// <param name="applicationSettings">An instance of <see cref="IOptions{TOptions}" /> that will be injected by the Reqnroll IoC</param>
    /// <param name="output">An instance of the <see cref="ITestOutputHelper" /> that, optionally, can be used to write debug information to the console during test runs</param>
    protected PlaywrightBase(IOptions<ApplicationSettings> applicationSettings, ITestOutputHelper output)
    {
        AppSettings  = applicationSettings.Value;
        OutputHelper = output;
        var browserType = GetRandomBrowser();

        Browser = browserType.LaunchAsync(new() { Headless = AppSettings.UseHeadless }).Result;
    }

    private IBrowser Browser { get; }

    /// <summary>
    ///     Gets an instance of <see cref="IPage" /> that can be used in the tests
    /// </summary>
    protected IPage Page { get; set; } = null!;

    /// <summary>
    ///     Gets an instance of the <see cref="ITestOutputHelper" /> that, optionally, can be used to write debug information to the console during test runs
    /// </summary>
    protected ITestOutputHelper OutputHelper { get; }

    /// <summary>
    ///     Gets an instance of the <see cref="ApplicationSettings" /> class - this is the 'value' from the injected IOptions instance from the constructor
    /// </summary>
    protected ApplicationSettings AppSettings { get; }

    /// <summary>
    ///     This optional method can be used to add extra headers to the <see cref="IPage" /> instance.
    ///     <para>
    ///         When called, as well as the supplied headers, the User-Agent will be set to 'Playwright Tests'
    ///     </para>
    /// </summary>
    /// <param name="additionalHeaders">The dictionary of string key/values to add to the <see cref="IPage" /></param>
    /// <returns>The updated <see cref="IPage" /> instance</returns>
    protected IPage SetHeadersAndCreatePage(Dictionary<string, string> additionalHeaders = null!) =>
        CreateTestHeaders().Merge([additionalHeaders]).SetExtraHttpHeaders(Browser);

    private static Dictionary<string, string> CreateTestHeaders() =>
        new() { { "User-Agent", "Playwright Tests" } };

    private IBrowserType GetRandomBrowser()
    {
        var       browserType       = playwright.Chromium;
        const int maxValueExclusive = 3;
        var       random            = new Random(DateTime.UtcNow.Millisecond).Next(maxValueExclusive);

        switch (random)
        {
            case 1:
                browserType = playwright.Firefox;
                WriteTheBrowserSelected("Randomly selected Firefox");

                break;

            case > 1:
                browserType = playwright.Webkit;
                WriteTheBrowserSelected("Randomly selected Webkit");

                break;

            default:
                WriteTheBrowserSelected("Randomly elected to use the default of Chromium");

                break;
        }

        return browserType;
    }

    private void WriteTheBrowserSelected(string browserMessage) =>
        OutputHelper.WriteLine(browserMessage);
}
