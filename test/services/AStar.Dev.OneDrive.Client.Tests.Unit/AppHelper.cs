using Avalonia;
using Avalonia.Headless;

namespace AStar.Dev.OneDrive.Client.Tests.Unit;

public class AppHelper
{
    internal static void EnsureHeadlessAvaloniaApp()
    {
        if (Application.Current is not null) return;

        try
        {
            AppBuilder.Configure<App>()
                .UseSkia()
                .UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false })
                .SetupWithoutStarting();
        }
        catch (InvalidOperationException)
        {
            // Setup may already have been called by another test; safe to ignore.
        }
    }
}
