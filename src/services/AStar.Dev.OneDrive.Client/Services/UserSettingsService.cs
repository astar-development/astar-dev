using System.Runtime.InteropServices;
using System.Text.Json;
using Azure.Identity;
using Microsoft.Identity.Client;

namespace AStar.Dev.OneDrive.Client.Services;

public sealed class UserSettings
{
    public string Theme { get; set; } = "Auto"; // Auto | Light | Dark
    public double WindowWidth { get; set; } = 800;
    public double WindowHeight { get; set; } = 600;
    public int? WindowX { get; set; }
    public int? WindowY { get; set; }
    public string? LastAccount { get; set; }

    // new property
    public bool DownloadFilesAfterSync { get; set; } = false;
    public bool RememberMe { get; set; } = true; // default to true

    // New: configurable max parallel downloads
    public int MaxParallelDownloads { get; set; } = 4;
    // New: configurable batch size for DB updates
    public int DownloadBatchSize { get; set; } = 100;
    public int CacheTag { get; set; } = 1; // used to version the cache name
}

public sealed class UserSettingsService
{
    private readonly string _filePath;

    public UserSettingsService()
    {
        var dir = GetSettingsDirectory();
        try
        {
            _ = Directory.CreateDirectory(dir);
        }
        catch { }

        _filePath = Path.Combine(dir, "AStar.OneDrive.Client.settings.json");
    }

    // Testable constructor: allow specifying custom file path (useful in unit tests)
    public UserSettingsService(string filePath) => _filePath = filePath;

    private static string GetSettingsDirectory()
    {
        if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AStarDev");
        }

        var xdg = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        if(!string.IsNullOrEmpty(xdg))
            return Path.Combine(xdg, "astar-dev");

        var home = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        // macOS and Linux default to ~/.config/astar-dev
        return Path.Combine(home, ".config", "astar-dev");
    }

    public UserSettings Load()
    {
        try
        {
            if(!File.Exists(_filePath))
                return new UserSettings();

            var json = File.ReadAllText(_filePath);
            UserSettings? userSettings = JsonSerializer.Deserialize<UserSettings>(json);
            return userSettings ?? new UserSettings();
        }
        catch
        {
            return new UserSettings();
        }
    }

    // Functional result-based loader
    public async Task<AStar.Dev.Functional.Extensions.Result<UserSettings, Exception>> LoadResultAsync() => await Functional.Extensions.Try.RunAsync(async () => Load());

    public void Save(UserSettings settings)
    {
        try
        {
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);

            // No explicit cache clearing API in Azure.Identity.
            // If RememberMe is false, next run will construct the credential without persistence,
            // so tokens won't survive app exit.
        }
        catch
        {
            // swallow exceptions for now, but consider logging
        }
    }

    // Functional result-based saver (returns the saved settings on success)
    public async Task<AStar.Dev.Functional.Extensions.Result<UserSettings, Exception>> SaveResultAsync(UserSettings settings)
        => await Functional.Extensions.Try.RunAsync(async () =>
                                                            {
                                                                Save(settings);
                                                                return settings;
                                                            });
}
