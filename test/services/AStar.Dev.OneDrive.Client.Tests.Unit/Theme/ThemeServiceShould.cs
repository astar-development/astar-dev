using AStar.Dev.OneDrive.Client.Theme;
using AStar.Dev.OneDrive.Client.User;
using Avalonia;
using Avalonia.Styling;

namespace AStar.Dev.OneDrive.Client.Tests.Unit.Theme;

[Collection("AvaloniaApp")]
public class ThemeServiceShould
{
    static ThemeServiceShould() => AppHelper.EnsureHeadlessAvaloniaApp();

    [Fact]
    public void ThemeServiceShouldApplyLightThemeWhenPreferenceIsLight()
    {
        // Arrange
        AppHelper.EnsureHeadlessAvaloniaApp();
        var sut = new ThemeService();
        var prefs = new UserPreferences { UiSettings = new UiSettings { Theme = "Light" } };

        // Act & Assert on UI thread
        sut.ApplyThemePreference(prefs);
        Application.Current.ShouldNotBeNull();
        var app = (App)Application.Current!;
        app.RequestedThemeVariant.ShouldBe(ThemeVariant.Light);
    }

    [Fact]
    public void ThemeServiceShouldApplyDarkThemeWhenPreferenceIsDark()
    {
        // Arrange
        AppHelper.EnsureHeadlessAvaloniaApp();
        var sut = new ThemeService();
        var prefs = new UserPreferences { UiSettings = new UiSettings { Theme = "Dark" } };

        // Act & Assert on UI thread
        sut.ApplyThemePreference(prefs);
        Application.Current.ShouldNotBeNull();
        var app = (App)Application.Current!;
        app.RequestedThemeVariant.ShouldBe(ThemeVariant.Dark);
    }

    [Fact]
    public void ThemeServiceShouldApplyDefaultThemeWhenPreferenceIsAutoOrUnknown()
    {
        // Arrange
        AppHelper.EnsureHeadlessAvaloniaApp();
        var sut = new ThemeService();
        var prefsAuto = new UserPreferences { UiSettings = new UiSettings { Theme = "Auto" } };
        var prefsUnknown = new UserPreferences { UiSettings = new UiSettings { Theme = "SomethingElse" } };

        sut.ApplyThemePreference(prefsAuto);
        var app = (App)Application.Current!;
        app.RequestedThemeVariant.ShouldBe(ThemeVariant.Default);

        sut.ApplyThemePreference(prefsUnknown);
        // Assert again remains/defaults to Default for unknown
        app.RequestedThemeVariant.ShouldBe(ThemeVariant.Default);
    }
}
