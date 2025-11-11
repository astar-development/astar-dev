using AStar.Dev.Source.Generators.Tests.TestHelpers;
using Shouldly;

namespace AStar.Dev.Source.Generators.Tests;

public class ServiceCollectionExtensionsGeneratorTests
{
    [Fact]
    public void Generates_Register_AsSelf_As_Well_As_Interface_When_AsSelf_Is_True()
    {
        const string serviceSource = """
                                     using AStar.Dev.Annotations;
                                     namespace Demo;
                                     [RegisterService(Lifetime.Scoped, AsSelf = true)]
                                     public class DemoService : IDemoService {}
                                     public interface IDemoService;
                                     """;

        var generated =
            GeneratorHelpers.RunGeneratorAndGetGeneratedForType("Demo.DemoService",
                TestHelpers.Constants.AttributeSource, serviceSource);
        generated.ShouldContain("AddScoped<global::Demo.DemoService>");
        generated.ShouldContain("AddScoped<global::Demo.IDemoService, global::Demo.DemoService>");
    }

    [Theory]
    [InlineData("Lifetime.Singleton", "AddSingleton")]
    [InlineData("Lifetime.Scoped", "AddScoped")]
    [InlineData("Lifetime.Transient", "AddTransient")]
    public void Generates_Correct_Lifetime_Registration(string lifetimeSpecifier, string expectedMethod)
    {
        var serviceSource = $$"""
                              using AStar.Dev.Annotations;
                              namespace Demo;
                              [RegisterService({{lifetimeSpecifier}})]
                              public class DemoService {}
                              """;

        var generated =
            GeneratorHelpers.RunGeneratorAndGetGeneratedForType("Demo.DemoService",
                TestHelpers.Constants.AttributeSource, serviceSource);
        generated.IndexOf(expectedMethod, StringComparison.OrdinalIgnoreCase).ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Does_Not_Generate_Registration_For_Unannotated_Class()
    {
        const string serviceSource = """
                                     using AStar.Dev.Annotations;
                                     namespace Demo;
                                     // Note: no RegisterService attribute
                                     public class DemoService {}
                                     """;

        var generated =
            GeneratorHelpers.RunGeneratorAndGetGeneratedForType("Demo.DemoService",
                TestHelpers.Constants.AttributeSource, serviceSource);

        // Assert that the generated text does NOT contain any AddXxx registration for DemoService
        generated.IndexOf("AddSingleton<global::Demo.DemoService>", StringComparison.OrdinalIgnoreCase).ShouldBe(-1);
        generated.IndexOf("AddScoped<global::Demo.DemoService>", StringComparison.OrdinalIgnoreCase).ShouldBe(-1);
        generated.IndexOf("AddTransient<global::Demo.DemoService>", StringComparison.OrdinalIgnoreCase).ShouldBe(-1);

        generated.ShouldNotContain("ServiceCollectionExtensions");
    }
}