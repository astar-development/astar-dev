using System.ComponentModel;
using AStar.Dev.OneDrive.Client.Scrolling;
using AStar.Dev.OneDrive.Client.Theme;
using AStar.Dev.OneDrive.Client.User;
using AStar.Dev.OneDrive.Client.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AStar.Dev.OneDrive.Client.Views;

public partial class MainWindow : Window
{
    private readonly IAutoScrollService _autoScrollService = new AutoScrollService();
    private readonly ThemeService _themeService = new();
    private readonly UserPreferenceService? _userPreferenceService;
    private readonly MainWindowViewModel? _vm;
    private readonly MainWindowCoordinator? _windowCoordinator;

    private bool _autoScrollEnabled = true;

    /// <summary>
    ///     Designer-friendly parameterless ctor
    /// </summary>
    public MainWindow() => InitializeComponent();

    public MainWindow(MainWindowViewModel vm, UserPreferenceService userPreferenceService, ThemeService themeService)
    {
        InitializeComponent();
        _vm = vm;
        _userPreferenceService = userPreferenceService;
        _themeService = themeService;
        _windowCoordinator = new MainWindowCoordinator(_userPreferenceService, _themeService);
        DataContext = _vm;

        ProgressList.TemplateApplied += (_, __) => UpdateProgressList();

        PostInitialize();
    }

    private void UpdateProgressList()
    {
        ScrollViewer? scrollViewer = ProgressList.GetVisualDescendants()
            .OfType<ScrollViewer>()
            .FirstOrDefault();
        if (scrollViewer == null)
            return;

        scrollViewer.ScrollChanged += (_, e) =>
        {
            var offset = scrollViewer.GetValue(ScrollViewer.OffsetProperty).Y;
            var extent = scrollViewer.GetValue(ScrollViewer.ExtentProperty).Height;
            var viewport = scrollViewer.GetValue(ScrollViewer.ViewportProperty).Height;

            _autoScrollEnabled = _autoScrollService.IsAtOrNearBottom(offset, extent, viewport) || extent <= viewport + 1;
        };

        _vm?.ProgressMessages.CollectionChanged += (s, e) => ScrollMessageDisplay(scrollViewer);
    }

    private void ScrollMessageDisplay(ScrollViewer? scrollViewer)
    {
        if (_userPreferenceService is null || _vm is null)
            return;

        if (_autoScrollService.ShouldScroll(_autoScrollEnabled, _vm.UserPreferences.UiSettings.FollowLog, _vm.ProgressMessages.Count))
            _ = Dispatcher.UIThread.InvokeAsync(() =>
            {
                var extent = scrollViewer?.GetValue(ScrollViewer.ExtentProperty).Height ?? 0;
                var viewport = scrollViewer?.GetValue(ScrollViewer.ViewportProperty).Height ?? 0;
                var bottom = _autoScrollService.GetBottomOffset(extent, viewport);
                _ = scrollViewer?.Offset = new Vector(0, bottom);
            }, DispatcherPriority.Render);
    }

    private void PostInitialize()
    {
        if (_userPreferenceService is null || _vm is null)
            return;

        _windowCoordinator?.Initialize(this, _vm);

        DataContext = _vm;

        Closing += (_, __) => PersistUserPreferences();
        _vm.PropertyChanged += (_, e) => UpdateTheLastAction(e);

        SetThemeBasedOnUserSettings(_vm.UserPreferences);
    }

    private void SetThemeBasedOnUserSettings(UserPreferences userPreferences)
    {
        try
        {
            ComboBox? combo = this.FindControl<ComboBox>("ThemeSelector");
            combo?.SelectedIndex = _windowCoordinator?.MapThemeToIndex(userPreferences.UiSettings.Theme) ?? 0;
        }
        catch
        {
            // ignored
        }
    }

    private void UpdateTheLastAction(PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(MainWindowViewModel.Status) || _userPreferenceService is null || _vm is null) return;
        PersistUserPreferences();
    }

    private void UpdateWindowDimensionsAndPosition(UserPreferences userPreferences)
    {
        try
        {
            if (userPreferences is { WindowSettings.WindowWidth: > 0, WindowSettings.WindowHeight: > 0 })
            {
                Width = userPreferences.WindowSettings.WindowWidth;
                Height = userPreferences.WindowSettings.WindowHeight;
            }

            if (userPreferences is { WindowSettings.WindowX: > 0, WindowSettings.WindowY: > 0 })
                Position = new PixelPoint(userPreferences.WindowSettings.WindowX, userPreferences.WindowSettings.WindowY);
        }
        catch
        {
            // ignored
        }
    }

    private void PersistUserPreferences()
    {
        try
        {
            if (_userPreferenceService is null || _vm is null) return;
            _windowCoordinator?.PersistUserPreferences(this, _vm);
        }
        catch
        {
            // ignored
        }
    }

    private void ThemeSelector_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox cb || _vm is null) return;
        _vm.UserPreferences.UiSettings.Theme = _windowCoordinator?.MapIndexToTheme(cb.SelectedIndex) ?? "Auto";
        _themeService.ApplyThemePreference(_vm.UserPreferences);
    }
}
