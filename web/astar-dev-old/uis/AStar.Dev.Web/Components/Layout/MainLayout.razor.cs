namespace AStar.Dev.Web.Components.Layout;

public partial class MainLayout
{
    private bool _settingsPanelOpen;

    private void OpenSettingsDialog() => _settingsPanelOpen = true;

    private void CloseSettingsPanel() => _settingsPanelOpen = false;
}
