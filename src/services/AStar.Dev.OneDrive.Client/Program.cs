using AStar.Dev.OneDrive.Client.Services;
using AStar.Dev.OneDrive.Client.Theme;
using AStar.Dev.OneDrive.Client.ViewModels;
using AStar.Dev.OneDrive.Client.Views;
using Avalonia;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace AStar.Dev.OneDrive.Client;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        // Create a minimal bootstrap logger in case host config references logging during startup
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        using IHost host = CreateHostBuilder(args).Build();

        try
        {
            host.Start();

            // expose the provider to the Avalonia App
            App.Services = host.Services;

            // Start the UI (this call blocks until the app exits)
            _ = BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        finally
        {
            // Stop the host and flush logs
            host.StopAsync().GetAwaiter().GetResult();
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
        => Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((ctx, cfg) =>
            {
                // Load project-local config first, then fallback to a centralized config at repo/src/appsettings.json
                _ = cfg.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
                _ = cfg.AddJsonFile("../../appsettings.json", optional: true, reloadOnChange: false);
                _ = cfg.AddUserSecrets<App>(optional: true);
            })
            .UseSerilog((ctx, services, loggerConfig) =>
            {
                // Configure Serilog from configuration (centralized in repo `src/appsettings.json`)
                _ = loggerConfig
                    .ReadFrom.Configuration(ctx.Configuration)
                    .Enrich.FromLogContext()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .WriteTo.Console();

                // Add a per-user rolling file sink (cross-platform: AppData on Windows, XDG or ~/.config on Linux/macOS)
                try
                {
                    string baseDir;

                    if(OperatingSystem.IsWindows())
                        baseDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    else
                    {
                        // Prefer XDG_STATE_HOME, then XDG_CONFIG_HOME, then ~/.config
                        baseDir = Environment.GetEnvironmentVariable("XDG_STATE_HOME")
                                  ?? Environment.GetEnvironmentVariable("XDG_CONFIG_HOME")
                                  ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config");
                    }

                    var logDir = Path.Combine(baseDir, "astar-dev", "astar-dev-onedrive-client", "logs");
                    _ = Directory.CreateDirectory(logDir);
                    var logPath = Path.Combine(logDir, "astar-dev-onedrive-client-.log");

                    _ = loggerConfig.WriteTo.File(logPath, rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Debug);
                }
                catch
                {
                    // ignore failures creating log dir or adding file sink
                }
            })
            .ConfigureServices((ctx, services) =>
            {
                IConfiguration config = ctx.Configuration;
                AppSettings appSettings = config.GetSection("Azure").Get<AppSettings>()!;

                _ = services.AddSingleton(appSettings);
                _ = services.AddSingleton<ILoginService, LoginService>();
                _ = services.AddSingleton<MainWindowViewModel>();
                _ = services.AddSingleton<UserSettings.UserPreferences>();
                _ = services.AddSingleton<OneDriveService>();
                _ = services.AddSingleton<ThemeService>();
                _ = services.AddSingleton<UserSettings. UserPreferencesService>();
                _ = services.AddSingleton<MainWindow>();
            });

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    //.UseReactiveUI();
}
