using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using AStar.Dev.OneDrive.Client.Services;

namespace AStar.Dev.OneDrive.Client.Tests.Unit;

public class UserSettingsServiceTests : IDisposable
{
    private readonly string _tempFile;

    public UserSettingsServiceTests()
    {
        _tempFile = Path.Combine(Path.GetTempPath(), $"AStar.Settings.Test.{Guid.NewGuid():N}.json");
        if (File.Exists(_tempFile)) File.Delete(_tempFile);
    }

    public void Dispose()
    {
        try { if (File.Exists(_tempFile)) File.Delete(_tempFile); } catch { }
    }

    [Fact]
    public async Task SaveResultAsync_Then_LoadResultAsync_Roundtrips()
    {
        var svc = new UserSettingsService(_tempFile);
        var settings = new UserSettings { Theme = "Dark", WindowWidth = 123.0, WindowHeight = 456.0, LastAccount = "test@local" };

        var saveResult = await svc.SaveResultAsync(settings);
        saveResult.Should().BeOfType<AStar.Dev.Functional.Extensions.Result<UserSettings, Exception>.Ok>();

        var loadResult = await svc.LoadResultAsync();
        loadResult.Should().BeOfType<AStar.Dev.Functional.Extensions.Result<UserSettings, Exception>.Ok>();

        var loaded = ((AStar.Dev.Functional.Extensions.Result<UserSettings, Exception>.Ok)loadResult).Value;
        loaded.Theme.Should().Be("Dark");
        loaded.WindowWidth.Should().BeApproximately(123.0, 0.01);
        loaded.WindowHeight.Should().BeApproximately(456.0, 0.01);
        loaded.LastAccount.Should().Be("test@local");
    }

    [Fact]
    public async Task LoadResultAsync_Returns_Default_When_FileMissing()
    {
        var svc = new UserSettingsService(_tempFile);
        // Ensure file doesn't exist
        if (File.Exists(_tempFile)) File.Delete(_tempFile);

        var loadResult = await svc.LoadResultAsync();
        loadResult.Should().BeOfType<AStar.Dev.Functional.Extensions.Result<UserSettings, Exception>.Ok>();
        var loaded = ((AStar.Dev.Functional.Extensions.Result<UserSettings, Exception>.Ok)loadResult).Value;
        loaded.Theme.Should().Be("Auto");
    }
}
