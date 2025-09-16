using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Components.Pages;

public partial class Counter : ComponentBase
{
    private int _currentCount;

    private void IncrementCount() => _currentCount++;
}
