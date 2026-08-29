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
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}