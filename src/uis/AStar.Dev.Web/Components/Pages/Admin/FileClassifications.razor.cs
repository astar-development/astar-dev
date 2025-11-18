using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Components.Pages.Admin;

[Authorize(Policy = "AdminOnly")]
public partial class FileClassifications : ComponentBase
{
}
