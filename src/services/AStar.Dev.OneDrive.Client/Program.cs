using Avalonia;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AStar.Dev.OneDrive.Client;

public static class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .With(ConfigureServices)
        .StartWithClassicDesktopLifetime(args);

    private static void ConfigureServices(IServiceCollection services)
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddUserSecrets<App>()
            .Build();
        
        AppSettings appSettings = config.GetSection("Azure").Get<AppSettings>()!;

        services.AddSingleton(appSettings);
        services.AddSingleton<ILoginService, LoginService>();
        services.AddTransient<MainWindowViewModel>();
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    //.UseReactiveUI();
}
