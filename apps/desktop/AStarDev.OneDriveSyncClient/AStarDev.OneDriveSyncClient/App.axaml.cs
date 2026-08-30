using System;
using System.Threading.Tasks;
using AStarDev.OneDriveSyncClient.Home;
using AStarDev.OneDriveSyncClient.Persistence;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;

namespace AStarDev.OneDriveSyncClient;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        using var db = new AStarDevContext(new DbContextOptionsBuilder<AStarDevContext>()
            .UseSqlite("Data Source=astar-dev.db")
            .Options);

        db.Database.Migrate();

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