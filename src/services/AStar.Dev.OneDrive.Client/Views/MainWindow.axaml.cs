using System.Collections;
using AStar.Dev.OneDrive.Client.Services;
using AStar.Dev.OneDrive.Client.Theme;
using AStar.Dev.OneDrive.Client.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using System.Linq;

namespace AStar.Dev.OneDrive.Client.Views;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel? _vm;
    private readonly UserSettingsService? _userSettingsService;
    private readonly ThemeService? _themeService;

    private bool _autoScrollEnabled = true;

    // Designer-friendly parameterless ctor

    public MainWindow() => InitializeComponent();

    public MainWindow(MainWindowViewModel vm, UserSettingsService userSettingsService, ThemeService themeService)
    {
        InitializeComponent();
        _vm = vm;
        _userSettingsService = userSettingsService;
        _themeService = themeService;

        DataContext = _vm;

        ProgressList.TemplateApplied += (_, __) =>
        {
            ScrollViewer? scrollViewer = ProgressList.GetVisualDescendants()
                                       .OfType<ScrollViewer>()
                                       .FirstOrDefault();
            if(scrollViewer == null)
                return;

            // Track whether user is at bottom
            scrollViewer.ScrollChanged += (_, e) =>
            {
                var offset = scrollViewer.GetValue(ScrollViewer.OffsetProperty).Y;
                var extent = scrollViewer.GetValue(ScrollViewer.ExtentProperty).Height;
                var viewport = scrollViewer.GetValue(ScrollViewer.ViewportProperty).Height;

                // If content fits, always auto-scroll
                _autoScrollEnabled = extent <= viewport + 1 || offset >= extent - viewport - 1;
            };

            // Scroll when new items are added
            _vm.ProgressMessages.CollectionChanged += (s, e) =>
            {
                if(_autoScrollEnabled && _vm.FollowLog && _vm.ProgressMessages.Count > 0)
                {
                    _ = Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        // Scroll to bottom by setting offset directly
                        var extent = scrollViewer.GetValue(ScrollViewer.ExtentProperty).Height;
                        var viewport = scrollViewer.GetValue(ScrollViewer.ViewportProperty).Height;
                        scrollViewer.Offset = new Avalonia.Vector(0, extent - viewport);
                    }, DispatcherPriority.Render);
                }
            };
        };

        PostInitialize();
    }

    // Perform runtime-only initialization that requires injected services
    private void PostInitialize()
    {
        // Ensure injected services are present (this method is DI-only)
        if(_userSettingsService is null || _vm is null || _themeService is null)
            return;

        // Apply persisted theme
        UserSettings userSettings = _userSettingsService.Load();
        if(Application.Current is App app)
        {
            switch(userSettings.Theme)
            {
                case "Light":
                    app.SetTheme(ThemeVariant.Light);
                    break;
                case "Dark":
                    app.SetTheme(ThemeVariant.Dark);
                    break;
                default:
                    app.SetTheme(ThemeVariant.Default);
                    break;
            }
        }

        // Set DataContext from injected ViewModel
        DataContext = _vm;
        _vm.DownloadFilesAfterSync = userSettings.DownloadFilesAfterSync;
        _vm.RememberMe = userSettings.RememberMe;
        _vm.FollowLog = userSettings.FollowLog;
        _vm.Status = userSettings.LastAction ?? "Not signed in";
        _vm.Theme = userSettings.Theme;

        // Initialize theme selector state
        try
        {
            ComboBox? combo = this.FindControl<ComboBox>("ThemeSelector");
            if(combo is not null && userSettings is not null)
            {
                combo.SelectedIndex = userSettings.Theme switch
                {
                    "Light" => 1,
                    "Dark" => 2,
                    _ => 0,
                };
            }
        }
        catch { }

        // Restore window position/size if available
        try
        {
            if(userSettings is not null && userSettings.WindowWidth > 0 && userSettings.WindowHeight > 0)
            {
                Width = userSettings.WindowWidth;
                Height = userSettings.WindowHeight;
            }

            if(userSettings is not null && userSettings.WindowX.HasValue && userSettings.WindowY.HasValue) Position = new PixelPoint(userSettings.WindowX.Value, userSettings.WindowY.Value);
        }
        catch { }

        // Persist window bounds on close
        Closing += (_, __) =>
        {
            try
            {
                if(_userSettingsService is null) return;
                UserSettings s = _userSettingsService.Load();
                s.WindowWidth = Width;
                s.WindowHeight = Height;
                s.WindowX = Position.X;
                s.WindowY = Position.Y;
                s.DownloadFilesAfterSync = _vm.DownloadFilesAfterSync;
                s.RememberMe = _vm.RememberMe;
                s.FollowLog = _vm.FollowLog;
                s.Theme = _vm.Theme;
                _userSettingsService.Save(s);
            }
            catch { }
        };

        // Update last account when ViewModel status changes (simple approach)
        _vm.PropertyChanged += (_, e) =>
        {
            try
            {
                if(e.PropertyName != nameof(MainWindowViewModel.Status) || _userSettingsService is null || _vm is null) return;
                UserSettings s = _userSettingsService.Load();
                s.LastAction = _vm.Status;
                _userSettingsService.Save(s);
            }
            catch { }
        };
    }

    private void ThemeSelector_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if(sender is ComboBox cb && Application.Current is App app) _themeService?.PersistNewTheme(cb, app, _userSettingsService ?? new UserSettingsService());
    }
}
