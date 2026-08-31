using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace AStarDev.Clock;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var splashWindow = new SplashWindow();
            desktop.MainWindow = splashWindow;

            splashWindow.Opened += async (_, _) =>
            {
                for (var seconds = 5; seconds >= 1; seconds--)
                {
                    splashWindow.SetCountdown(seconds);
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }

                var mainWindow = new MainWindow();
                desktop.MainWindow = mainWindow;
                mainWindow.Show();
                splashWindow.Close();
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}