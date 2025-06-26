using AStar.Dev.Web.UI.Models;
using Microsoft.Extensions.Options;
using Xunit.Abstractions;

namespace AStar.Dev.Web.UI;

public abstract class PlaywrightBase
{
    private readonly IPlaywright      playwright = Playwright.CreateAsync().Result;

    protected PlaywrightBase(IOptions<ApplicationSettings> applicationSettings, ITestOutputHelper output)
    {
        AppSettings  = applicationSettings.Value;
        OutputHelper = output;
        var browserType = GetRandomBrowser();

        Browser = browserType.LaunchAsync(new() { Headless = AppSettings.UseHeadless, }).Result;
    }

    private          IBrowser         Browser      { get; }

    protected IPage               Page         { get; set; } = null!;
    protected ITestOutputHelper   OutputHelper { get; }
    protected ApplicationSettings AppSettings  { get; }

    public IPage SetHeadersAndCreatePage(Dictionary<string, string> additionalHeaders = null!) =>
        CreateTestHeaders().Merge([additionalHeaders,]).SetExtraHttpHeaders(Browser);

    private Dictionary<string, string> CreateTestHeaders() =>
        new() { { "User-Agent", "Playwright Tests" }, };

    private IBrowserType GetRandomBrowser()
    {
        while (true)
        {
            var       browserType       = playwright.Chromium;
            const int maxValueExclusive = 3;
            var       random            = new Random(DateTime.UtcNow.Millisecond).Next(maxValueExclusive);

            switch (random)
            {
                case 1:
                    if(AppSettings.IsDevelopment)
                    {
                        WriteTheBrowserSelected("Re-selecting..");

                        break;
                    }

                    browserType = playwright.Firefox;
                    WriteTheBrowserSelected("Randomly selected Firefox");

                    break;

                case > 1:
                    if(AppSettings.IsDevelopment)
                    {
                        WriteTheBrowserSelected("Re-selecting..");

                        break;
                    }

                    browserType = playwright.Webkit;
                    WriteTheBrowserSelected("Randomly selected Webkit");

                    break;

                default:
                    WriteTheBrowserSelected("Randomly elected to use the default of Chromium");

                    break;
            }

            return browserType;
        }
    }

    /// <summary>
    ///     Firefox doesn't like connecting to localhost, even over http... hence we have this method to see if we need to change the browser type
    /// </summary>
    /// <returns><c>true</c> when the URI is for the localhost, else <c>false</c> is returned</returns>
    private bool IsLocalHostUri() =>
        AppSettings.BaseUri.StartsWith("http", StringComparison.OrdinalIgnoreCase)
        && AppSettings.BaseUri.Contains("localhost", StringComparison.OrdinalIgnoreCase);

    private void WriteTheBrowserSelected(string browserMessage) =>
        OutputHelper.WriteLine(browserMessage);
}
