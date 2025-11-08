using AStar.Dev.Source.Generators.Test.Unit.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AStar.Dev.Source.Generators.Test.Unit.ServiceCollectionExtensionsGenerator.TransientLifetime;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void CreateTheServiceCollectionExtensionsForServiceWithoutInterface()
    {
        var generator = new Generators.ServiceCollectionExtensionsGenerator();
        var syntaxTree = CSharpSyntaxTree.ParseText(EmbeddedResourceHelpers.GetResourceAsString("DemoServiceTransient.cs"));
        var compilation = CSharpCompilation.Create(
            nameof(CreateTheServiceCollectionExtensionsForServiceWithoutInterface),
            [syntaxTree],
            [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)]);
        var driver = CSharpGeneratorDriver.Create(generator)
            .RunGenerators(compilation);
        var result = driver.GetRunResult();
        var output = result.GeneratedTrees.Single(t =>
            t.FilePath.EndsWith("ServiceCollectionExtensions.g.cs")).ToString();

        output.ShouldMatchApproved();
    }
    [Fact]
    public void CreateTheServiceCollectionExtensionsForServiceWithInterface()
    {
        var generator = new Generators.ServiceCollectionExtensionsGenerator();
        var syntaxTree = CSharpSyntaxTree.ParseText(EmbeddedResourceHelpers.GetResourceAsString("DemoServiceWithInterfaceTransient.cs"));
        var compilation = CSharpCompilation.Create(
            nameof(CreateTheServiceCollectionExtensionsForServiceWithInterface),
            [syntaxTree],
            [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)]);
        var driver = CSharpGeneratorDriver.Create(generator)
            .RunGenerators(compilation);
        var result = driver.GetRunResult();
        var output = result.GeneratedTrees.Single(t =>
            t.FilePath.EndsWith("ServiceCollectionExtensions.g.cs")).ToString();

        output.ShouldMatchApproved();
    }
}
