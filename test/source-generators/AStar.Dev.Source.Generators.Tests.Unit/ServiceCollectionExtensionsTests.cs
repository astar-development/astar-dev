using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AStar.Dev.Source.Generators.Test.Unit;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void CreateTheServiceCollectionExtensions()
    {
        var generator = new ServiceCollectionExtensionsGenerator();
        var syntaxTree = CSharpSyntaxTree.ParseText(GetResourceAsString("DemoService.cs"));
        var compilation = CSharpCompilation.Create(
            nameof(CreateTheServiceCollectionExtensions),
            [syntaxTree],
            [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)]);
        var driver = CSharpGeneratorDriver.Create(generator)
            .RunGenerators(compilation);
        var result = driver.GetRunResult();
        var output = result.GeneratedTrees.Single(t =>
            t.FilePath.EndsWith("ServiceCollectionExtensions.g.cs")).ToString();

        output.ShouldMatchApproved();
    }

    private string GetResourceAsString(string resourceName)
    {
        var assembly = typeof(ServiceCollectionExtensionsTests).Assembly;
        var manifestResourceNames = assembly.GetManifestResourceNames();
        resourceName = manifestResourceNames.Single(x => x.Equals($"AStar.Dev.Source.Generators.Test.Unit.ExampleFiles.{resourceName}", StringComparison.OrdinalIgnoreCase));

        using var stream = assembly.GetManifestResourceStream(resourceName) ?? throw new InvalidOperationException($"Resource '{resourceName}' not found.");
        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }
}
