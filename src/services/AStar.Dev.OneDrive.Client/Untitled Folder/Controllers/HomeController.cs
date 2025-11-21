using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspnetCore_Changed_Files.Models;
using AspnetCore_Changed_Files.Helpers;
using System.Security.Claims;
using Microsoft.Graph;
using Microsoft.Identity.Web;

namespace AspnetCore_Changed_Files.Controllers;

public class HomeController : Controller
{
    [AuthorizeForScopes(Scopes = ["Files.ReadWrite.All"])] // Add this attribute
    public async Task<IActionResult> Index(
        [FromServices]GraphServiceClient client,
        string deltaToken
    )
    {
        var viewModel = new IndexViewModel(){Items = null!, DeltaToken = string.Empty};

        if (User.Identity!.IsAuthenticated)
        {
            // No try-catch here. Let the attribute handle auth errors.
            DeltaFiles? deltaFiles = await GetRootFilesAsync(client, deltaToken);

            viewModel.Items = deltaFiles.Files;
            viewModel.DeltaToken = deltaFiles.DeltaToken;
        }

        return View(viewModel);
    }

    private async Task<DeltaFiles> GetRootFilesAsync(GraphServiceClient client, string deltaToken) => await client.GetRootFilesAsync(deltaToken);

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
