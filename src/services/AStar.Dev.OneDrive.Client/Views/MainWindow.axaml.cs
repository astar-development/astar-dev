using System.ComponentModel;
using AStar.Dev.OneDrive.Client.Services;
using AStar.Dev.OneDrive.Client.Theme;
using AStar.Dev.OneDrive.Client.UserSettings;
using AStar.Dev.OneDrive.Client.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AStar.Dev.OneDrive.Client.Views;

public partial class MainWindow : Window
{
    private readonly ThemeService _themeService = new();
    private readonly UserPreferencesService? _userPreferencesService;
    private readonly MainWindowViewModel? _vm;

    private bool _autoScrollEnabled = true;

    /// <summary>
    ///     Designer-friendly parameterless ctor
    /// </summary>
    public MainWindow() => InitializeComponent();

    public MainWindow(MainWindowViewModel vm, UserPreferencesService userPreferencesService, ThemeService themeService)
    {
        InitializeComponent();
        _vm = vm;
        _userPreferencesService = userPreferencesService;
        _themeService = themeService;
        DataContext = _vm;

        ProgressList.TemplateApplied += (_, __) => UpdateProgressList();

        PostInitialize();
    }

    private void UpdateProgressList()
    {
        ScrollViewer? scrollViewer = ProgressList.GetVisualDescendants()
            .OfType<ScrollViewer>()
            .FirstOrDefault();
        if(scrollViewer == null)
            return;

        scrollViewer.ScrollChanged += (_, e) =>
        {
            var offset = scrollViewer.GetValue(ScrollViewer.OffsetProperty).Y;
            var extent = scrollViewer.GetValue(ScrollViewer.ExtentProperty).Height;
            var viewport = scrollViewer.GetValue(ScrollViewer.ViewportProperty).Height;

            _autoScrollEnabled = extent <= viewport + 1 || offset >= extent - viewport - 1;
        };

        _vm?.ProgressMessages.CollectionChanged += (s, e) => ScrollMessageDisplay(scrollViewer);
    }

    private void ScrollMessageDisplay(ScrollViewer? scrollViewer)
    {
        if(_autoScrollEnabled && _vm!.UserPreferences.FollowLog && _vm.ProgressMessages.Count > 0)
        {
            _ = Dispatcher.UIThread.InvokeAsync(() =>
            {
                var extent = scrollViewer?.GetValue(ScrollViewer.ExtentProperty).Height;
                var viewport = scrollViewer?.GetValue(ScrollViewer.ViewportProperty).Height;
                _ = (scrollViewer?.Offset = new Vector(0, (extent - viewport) ?? 0));
            }, DispatcherPriority.Render);
        }
    }

    private void PostInitialize()
    {
        if(_userPreferencesService is null || _vm is null)
            return;

        UserSettings.UserPreferences userPreferences = _userPreferencesService.Load();
        _themeService.ApplyThemePreference(userPreferences);

        DataContext = _vm;
        _vm.UserPreferences = userPreferences;
        _vm.Status = userPreferences.LastAction ?? "Not signed in";

        SetThemeBasedOnUserSettings(userPreferences);

        UpdateWindowDimensionsAndPosition(userPreferences);

        Closing += (_, __) => PersistUserPreferences();

        _vm.PropertyChanged += (_, e) => UpdateTheLastAction(e);
    }

    private void SetThemeBasedOnUserSettings(UserSettings.UserPreferences userPreferences)
    {
        try
        {
            ComboBox? combo = this.FindControl<ComboBox>("ThemeSelector");
            _ = (combo?.SelectedIndex = userPreferences.Theme switch
            {
                "Light" => 1,
                "Dark" => 2,
                _ => 0
            });
        }
        catch
        {
            // ignored
        }
    }

    private void UpdateTheLastAction(PropertyChangedEventArgs e)
    {
        try
        {
            if(e.PropertyName != nameof(MainWindowViewModel.Status) || _userPreferencesService is null || _vm is null) return;
            UserSettings.UserPreferences s = _userPreferencesService.Load();
            s.LastAction = _vm.Status;
            _userPreferencesService.Save(s);
        }
        catch
        {
            // ignored
        }
    }

    private void UpdateWindowDimensionsAndPosition(UserSettings.UserPreferences userPreferences)
    {
        try
        {
            if(userPreferences is { WindowWidth: > 0, WindowHeight: > 0 })
            {
                Width = userPreferences.WindowWidth;
                Height = userPreferences.WindowHeight;
            }

            if(userPreferences is { WindowX: not null, WindowY: not null })
                Position = new PixelPoint(userPreferences.WindowX.Value, userPreferences.WindowY.Value);
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
            if(_userPreferencesService is null) return;
            UserSettings.UserPreferences s = _userPreferencesService.Load();
            s.WindowWidth = Width;
            s.WindowHeight = Height;
            s.WindowX = Position.X;
            s.WindowY = Position.Y;
            s.RememberMe = _vm?.UserPreferences.RememberMe ?? false;
            s.FollowLog = _vm?.UserPreferences.FollowLog ?? false;
            s.Theme = _vm?.UserPreferences.Theme ?? "Auto";
            s.DownloadFilesAfterSync = _vm?.UserPreferences.DownloadFilesAfterSync ?? false;
            _userPreferencesService.Save(s);
        }
        catch
        {
            // ignored
        }
    }

    private void ThemeSelector_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if(sender is ComboBox cb) 
        {
            _vm?.UserPreferences.Theme = cb.SelectedIndex switch
            {
                1 => "Light",
                2 => "Dark",
                _ => "Auto"
            };
        }
    }
}
