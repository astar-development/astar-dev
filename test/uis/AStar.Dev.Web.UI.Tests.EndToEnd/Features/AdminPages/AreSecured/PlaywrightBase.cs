using Microsoft.Extensions.Options;
using Xunit.Abstractions;

namespace AStar.Dev.Web.UI.Features.AdminPages.AreSecured;

public class PlaywrightBase(IOptions<ApplicationSettings> applicationSettings, ITestOutputHelper output)
{
    public readonly ApplicationSettings AppSettings = applicationSettings.Value;
    public readonly ITestOutputHelper OutputHelper = output;

}
