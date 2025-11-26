using AStar.Dev.OneDrive.Client.Services;
using AStar.Dev.OneDrive.Client.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AStar.Dev.OneDrive.Client.Views;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel? _vm;
    private readonly UserSettingsService? _userSettingsService;
    private bool _autoScrollEnabled = true;
    // Designer-friendly parameterless ctor
    public MainWindow() => InitializeComponent();

    public MainWindow(MainWindowViewModel vm, UserSettingsService userSettingsService)
    {
        InitializeComponent();
        _vm = vm;
        _userSettingsService = userSettingsService;
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
        if(_userSettingsService is null || _vm is null)
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

            if(userSettings is not null && userSettings.WindowX.HasValue && userSettings.WindowY.HasValue)
            {
                Position = new PixelPoint(userSettings.WindowX.Value, userSettings.WindowY.Value);
            }
        }
        catch { }

        // Persist window bounds on close
        Closing += (_, __) =>
        {
            try
            {
                if(_userSettingsService is not null)
                {
                    UserSettings s = _userSettingsService.Load();
                    s.WindowWidth = Width;
                    s.WindowHeight = Height;
                    s.WindowX = Position.X;
                    s.WindowY = Position.Y;
                    s.DownloadFilesAfterSync = _vm.DownloadFilesAfterSync;
                    _userSettingsService.Save(s);
                }
            }
            catch { }
        };

        // Update last account when ViewModel status changes (simple approach)
        _vm.PropertyChanged += (_, e) =>
        {
            try
            {
                if(e.PropertyName == nameof(MainWindowViewModel.Status) && _userSettingsService is not null && _vm is not null)
                {
                    UserSettings s = _userSettingsService.Load();
                    s.LastAccount = _vm.Status;
                    _userSettingsService.Save(s);
                }
            }
            catch { }
        };
    }
}
