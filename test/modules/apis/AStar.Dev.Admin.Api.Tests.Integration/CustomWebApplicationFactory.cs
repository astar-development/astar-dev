using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace AStar.Dev.Admin.Api.Tests.Integration;

public sealed class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram>
    where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _ = builder
            .ConfigureTestServices(services =>
                                   {
                                       // var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AdminContext>));
                                       //
                                       // services.Remove(dbContextDescriptor!);

                                       // services.AddScoped<AdminContext>(_ => new(
                                       //             $"Server=sql1;Database=AdminDb-{Guid.CreateVersion7()};User Id=sa;Password=;TrustServerCertificate=true;",
                                       //             new()));
                                   });

        _ = builder.UseEnvironment("Development");
    }
}
