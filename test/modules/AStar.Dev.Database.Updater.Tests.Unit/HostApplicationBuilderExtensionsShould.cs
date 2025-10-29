// using System.IO.Abstractions;
// using System.Text.Json;
// using System.Text.Json.Serialization;
// using AStar.Dev.Database.Updater.Core;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;
// using Microsoft.Extensions.Logging;
// using Microsoft.Extensions.Options;
//
// namespace AStar.Dev.Database.Updater.Tests.Unit;
//
// [TestSubject(typeof(HostApplicationBuilderExtensions))]
// public class HostApplicationBuilderExtensionsShould
// {
//     [Fact]
//     public void ReturnTheSameHostApplicationBuilderInstance()
//     {
//         var builder = Host.CreateApplicationBuilder();
//
//         var result = builder.ConfigureApplicationServices();
//
//         result.ShouldBeSameAs(builder);
//     }
//
//     [Fact]
//     public void AddServiceDefaults()
//     {
//         var builder = Host.CreateApplicationBuilder();
//
//         builder.ConfigureApplicationServices();
//
//         builder.Services.BuildServiceProvider()
//                .GetService<IHostedService>()
//                .ShouldNotBeNull();
//     }
//
//     [Fact]
//     public void ConfigureJsonSerializerOptions()
//     {
//         var builder = Host.CreateApplicationBuilder();
//
//         builder.ConfigureApplicationServices();
//
//         var serviceProvider = builder.Services.BuildServiceProvider();
//         var options         = serviceProvider.GetRequiredService<IOptions<JsonSerializerOptions>>().Value;
//
//         options.PropertyNamingPolicy.ShouldBe(JsonNamingPolicy.CamelCase);
//         options.WriteIndented.ShouldBeTrue();
//         options.DefaultIgnoreCondition.ShouldBe(JsonIgnoreCondition.WhenWritingNull);
//     }
//
//     [Fact]
//     public void AddExternalSettingsFile()
//     {
//         var builder              = Host.CreateApplicationBuilder();
//         var initialConfigSources = builder.Configuration.Sources.Count;
//
//         builder.ConfigureApplicationServices();
//
//         builder.Configuration.Sources.Count.ShouldBeGreaterThan(initialConfigSources);
//     }
//
//     [Fact]
//     public void ConfigureDatabaseUpdaterOptions()
//     {
//         var builder = Host.CreateApplicationBuilder();
//
//         builder.ConfigureApplicationServices();
//
//         builder.Services.ShouldContainServiceType(typeof(IValidateOptions<DatabaseUpdaterConfiguration>));
//
//         var validator = builder.Services
//                                .FirstOrDefault(sd => sd.ServiceType == typeof(IValidateOptions<DatabaseUpdaterConfiguration>))
//                                ?.ImplementationType;
//
//         validator.ShouldBe(typeof(DatabaseUpdaterConfigurationValidator));
//     }
//
//     [Fact]
//     public void AddSerilogLogging()
//     {
//         var builder = Host.CreateApplicationBuilder();
//
//         builder.ConfigureApplicationServices();
//
//         var descriptor = builder.Services.FirstOrDefault(sd => sd.ServiceType           == typeof(ILoggerFactory) &&
//                                                                sd.ImplementationFactory != null);
//
//         descriptor.ShouldNotBeNull("Serilog logger factory should be registered");
//     }
//
//     [Fact]
//     public void RegisterRequiredServices()
//     {
//         var builder = Host.CreateApplicationBuilder();
//
//         builder.ConfigureApplicationServices();
//
//         builder.Services.ShouldContainServiceType(typeof(AddNewFilesService));
//         builder.Services.ShouldContainServiceType(typeof(IValidateOptions<DatabaseUpdaterConfiguration>));
//         builder.Services.ShouldContainServiceType(typeof(TimeDelay));
//         builder.Services.ShouldContainServiceType(typeof(IFileSystem));
//     }
//
//     [Fact]
//     public void RegisterBackgroundService()
//     {
//         var builder = Host.CreateApplicationBuilder();
//
//         builder.ConfigureApplicationServices();
//
//         var hasBackgroundService = builder.Services.Any(sd => sd.ServiceType        == typeof(IHostedService) &&
//                                                               sd.ImplementationType == typeof(NewFilesBackgroundService));
//
//         hasBackgroundService.ShouldBeTrue("NewFilesBackgroundService should be registered as a hosted service");
//     }
// }

