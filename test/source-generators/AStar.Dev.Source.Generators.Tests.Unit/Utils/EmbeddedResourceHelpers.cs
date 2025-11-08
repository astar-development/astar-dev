using AStar.Dev.Source.Generators.Test.Unit.ServiceCollectionExtensionsGenerator.ScopedLifetime;

namespace AStar.Dev.Source.Generators.Test.Unit.Utils;

public static class EmbeddedResourceHelpers
{
    internal static string GetResourceAsString(string resourceName)
    {
        var assembly = typeof(ServiceCollectionExtensionsTests).Assembly;
        var manifestResourceNames = assembly.GetManifestResourceNames();
        resourceName = manifestResourceNames.Single(x => x.Equals($"AStar.Dev.Source.Generators.Test.Unit.ExampleFiles.{resourceName}", StringComparison.OrdinalIgnoreCase));

        using var stream = assembly.GetManifestResourceStream(resourceName) ?? throw new InvalidOperationException($"Resource '{resourceName}' not found.");
        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }
}
