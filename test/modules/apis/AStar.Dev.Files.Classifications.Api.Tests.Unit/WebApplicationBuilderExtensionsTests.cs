using AStar.Dev.Files.Classifications.Api.Endpoints.FileClassifications.V1;
using AStar.Dev.Files.Classifications.Api.Services;
using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.AspNetCore.Builder;

namespace AStar.Dev.Files.Classifications.Api.Tests.Unit;

public class WebApplicationBuilderExtensionsTests
{
    [Fact]
    public void AddFileClassificationsApplicationServices_Should_Register_Services()
    {
        var builder = WebApplication.CreateBuilder();

        builder.AddFileClassificationsApplicationServices();

        var services = builder.Services;

        services.Any(d => d.ServiceType == typeof(FilesContext)).ShouldBeTrue();

        services.Any(d => d.ServiceType == typeof(IFileClassificationsService2) &&
                          d.ImplementationType == typeof(FileClassificationsService2))
            .ShouldBeTrue();

        services.Any(d => d.ServiceType == typeof(GetFileClassificationsHandler))
            .ShouldBeTrue();
    }
}
