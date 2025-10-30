using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;

namespace AStar.Dev.Web.Components.Layout;

public partial class SettingsPanel : ComponentBase
{
    private OfficeColor _officeColor = OfficeColor.Office;

    [Inject]
    public required ILogger<SettingsPanel> Logger { get; set; }

    [Inject]
    public required IJSRuntime JsRuntime { get; set; }

    [Parameter]
    public bool IsOpen { get; set; }

    [Parameter]
    public EventCallback OnClose { get; set; }

    public required DesignThemeModes Mode { get; set; }

    private OfficeColor OfficeColor
    {
        get => _officeColor;
        set
        {
            if(_officeColor != value)
            {
                _officeColor = value;
                _ = SaveOfficeColorToStorage();
                StateHasChanged();
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender) => await LoadOfficeColorFromStorage();

    private async Task LoadOfficeColorFromStorage()
    {
        try
        {
            var storedColor = await JsRuntime.InvokeAsync<string>("localStorage.getItem", "officeColor");

            if(!string.IsNullOrEmpty(storedColor) && Enum.TryParse<OfficeColor>(storedColor, out OfficeColor parsedColor))
            {
                _officeColor = parsedColor;
                StateHasChanged();
            }
        }
        catch(Exception ex)
        {
            Logger.LogError(ex, "Error loading OfficeColor from storage");
        }
    }

    private async Task SaveOfficeColorToStorage()
    {
        try
        {
            await JsRuntime.InvokeVoidAsync("localStorage.setItem", "officeColor", OfficeColor.ToString());
        }
        catch(Exception ex)
        {
            Logger.LogError(ex, "Error saving OfficeColor to storage");
        }
    }

    private void OnLoaded(LoadedEventArgs e) => Logger.LogInformation($"Theme Loaded: {(e.Mode == DesignThemeModes.System ? "System" : "")} {(e.IsDark ? "Dark" : "Light")}");

    private void OnLuminanceChanged(LuminanceChangedEventArgs e) => Logger.LogInformation($"Theme Changed: {(e.Mode == DesignThemeModes.System ? "System" : "")} {(e.IsDark ? "Dark" : "Light")}");

    private async Task OnCloseClicked() => await OnClose.InvokeAsync();
}
