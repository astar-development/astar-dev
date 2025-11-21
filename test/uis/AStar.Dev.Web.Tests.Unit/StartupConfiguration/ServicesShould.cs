// using AStar.Dev.Admin.Api.Client.Sdk.AdminApi;
// using AStar.Dev.AspNet.Extensions;
// using AStar.Dev.Files.Api.Client.SDK.FilesApi;
// using AStar.Dev.Images.Api.Client.SDK.ImagesApi;
// using AStar.Dev.Web.StartupConfiguration;
// using JetBrains.AStar.Dev.Annotations;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Options;
// using Microsoft.Identity.Web;
// using NSubstitute;
//
// namespace AStar.Dev.Web.UI.StartupConfiguration;
//
// [TestSubject(typeof(IServiceCollection))]
// public sealed class ServicesShould
// {
//     private readonly ConfigurationManager configurationManager;
//     private readonly IServiceCollection   services;
//
//     public ServicesShould()
//     {
//         var tokenAcquisition = Substitute.For<ITokenAcquisition>();
//         services = new ServiceCollection();
//         services.AddScoped<ITokenAcquisition>(_ => tokenAcquisition);
//         configurationManager = new();
//         configurationManager.AddJsonFile("StartupConfiguration/appSettings.json");
//     }
//
//     [Fact]
//     public void AddTheApiConfigurationOptions()
//     {
//         _ = services.AddApplicationServices(configurationManager);
//
//         var service = services.BuildServiceProvider().GetService<IOptions<ApiConfiguration>>();
//
//         service.ShouldNotBeNull();
//     }
//
//     [Fact]
//     public void AddTheFilesApiConfigurationOptions()
//     {
//         _ = services.AddApplicationServices(configurationManager);
//         var service = services.BuildServiceProvider().GetService<IOptions<FilesApiConfiguration>>();
//
//         service.ShouldNotBeNull();
//     }
//
//     [Fact]
//     public void AddTheImagesApiConfigurationOptions()
//     {
//         _ = services.AddApplicationServices(configurationManager);
//
//         var service = services.BuildServiceProvider().GetService<IOptions<ImagesApiConfiguration>>();
//
//         service.ShouldNotBeNull();
//     }
//
//     [Fact]
//     public void AddTheAdminApiConfigurationOptions()
//     {
//         _ = services.AddApplicationServices(configurationManager);
//
//         var service = services.BuildServiceProvider().GetService<IOptions<AdminApiConfiguration>>();
//
//         service.ShouldNotBeNull();
//     }
//
//     [Fact]
//     public void AddTheAdminApiClient()
//     {
//         _ = services.AddApplicationServices(configurationManager);
//
//         var apiClient = services.BuildServiceProvider().GetService<AdminApiClient>();
//
//         apiClient.ShouldNotBeNull();
//     }
//
//     [Fact]
//     public void AddTheFilesApiClient()
//     {
//         _ = services.AddApplicationServices(configurationManager);
//
//         var apiClient = services.BuildServiceProvider().GetService<FilesApiClient>();
//
//         apiClient.ShouldNotBeNull();
//     }
//
//     [Fact]
//     public void AddTheImagesApiClient()
//     {
//         _ = services.AddApplicationServices(configurationManager);
//
//         var apiClient = services.BuildServiceProvider().GetService<ImagesApiClient>();
//
//         apiClient.ShouldNotBeNull();
//     }
// }

