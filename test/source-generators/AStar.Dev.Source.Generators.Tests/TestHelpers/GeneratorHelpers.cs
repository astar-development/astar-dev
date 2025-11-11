using System.Reflection;
using System.Runtime;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyInjection;

namespace AStar.Dev.Source.Generators.Tests.TestHelpers;

public static class GeneratorHelpers
{
    private static readonly CSharpParseOptions ParseOptions = new(LanguageVersion.Latest);
    private static readonly IIncrementalGenerator Generator = new ServiceCollectionExtensionsGenerator();

    public static string RunGeneratorAndGetGeneratedForType(string typeFullName, string attributeSource,
        string serviceSource)
    {
        var syntaxTrees = new[]
        {
            CSharpSyntaxTree.ParseText(attributeSource, ParseOptions),
            CSharpSyntaxTree.ParseText(serviceSource, ParseOptions)
        };

        var references = new List<MetadataReference>
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(GCSettings).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(IServiceCollection).Assembly.Location),
            MetadataReference.CreateFromFile(Assembly.GetExecutingAssembly().Location)
        };

        var compilation = CSharpCompilation.Create("TestAssembly",
            syntaxTrees,
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var driver = CSharpGeneratorDriver.Create(Generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out var diagnostics);

        // Grab all generated trees and merge them for easier assertions
        var generatedText = string.Join(Environment.NewLine,
            updatedCompilation.SyntaxTrees
                .Where(t => t.FilePath.EndsWith(".g.cs") || t.FilePath == "")
                .Select(t => t.ToString()));

        // If the generator emits per-type files, try to find the one for the type
        if (string.IsNullOrEmpty(typeFullName))
        {
            return generatedText;
        }

        {
            var marker = typeFullName.Split('.').Last(); // DemoService
            var byName = updatedCompilation.SyntaxTrees
                .Select(t => new { Tree = t, Text = t.ToString(), Path = t.FilePath })
                .FirstOrDefault(x => x.Path.EndsWith($"{marker}ServiceCollectionExtensions.g.cs"));
            if (byName != null)
            {
                return byName.Text;
            }
        }

        return generatedText;
    }
}