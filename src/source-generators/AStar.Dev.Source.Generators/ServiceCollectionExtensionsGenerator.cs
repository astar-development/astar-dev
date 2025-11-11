using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AStar.Dev.Source.Generators;

[Generator]
public sealed class ServiceCollectionExtensionsGenerator : IIncrementalGenerator
{
    private static readonly DiagnosticDescriptor InvalidAsDescriptor =
        new(
#pragma warning disable RS2008
            Constants.DiagnosticIds.AStarGen001,
#pragma warning restore RS2008
            "Invalid RegisterService As value",
            "RegisterService As argument for '{0}' is not a type and will be ignored",
            "RegisterServiceGenerator",
            DiagnosticSeverity.Warning,
            true);

    private static readonly SymbolDisplayFormat EmitFormat = SymbolDisplayFormat.FullyQualifiedFormat;

    public void Initialize(IncrementalGeneratorInitializationContext incrementalContext)
    {
        IncrementalValuesProvider<INamedTypeSymbol?> classSyntax = SelectClassesWithAttributes(incrementalContext);
        IncrementalValuesProvider<(INamedTypeSymbol namedTypeSymbol, AttributeData? attributeData)> services = FilterClassesToRegisterServiceAnnotated(classSyntax);
        IncrementalValuesProvider<ServiceModel> serviceModels = CreateServiceRegistrationsModel(services)
            .Where(s => !string.IsNullOrEmpty(s?.ImplementationType?.Name));

        incrementalContext.RegisterSourceOutput(serviceModels.Collect(), (spc, models) =>
        {
            var modelsList = models.ToList();

            if (modelsList.Count == 0)
            {
                // emit a ping file to indicate that the generator ran but no classes required generating.
                spc.AddSource("GeneratorPing.g.cs", "// Generator ran");
                return;
            }

            var generated = Generate(modelsList);
            spc.AddSource($"{modelsList.First().ImplementationType.Name}ServiceCollectionExtensions.g.cs", generated);
        });
    }

    private static IncrementalValuesProvider<ServiceModel> CreateServiceRegistrationsModel(
        IncrementalValuesProvider<(INamedTypeSymbol namedTypeSymbol, AttributeData? attributeData)> services) =>
        services.Select((tuple, _) =>
        {
            (INamedTypeSymbol? symbol, AttributeData? attribute) = tuple;

            return ConstructorTypesToIgnore(symbol) ? null! : CreateModelFromAttribute(symbol, attribute);
        });

    private static bool IsOpenGeneric(INamedTypeSymbol s) => s.Arity > 0 && s.TypeArguments.Any(ta => ta.TypeKind == TypeKind.TypeParameter);

    private static ServiceModel CreateModelFromAttribute(INamedTypeSymbol implSymbol, AttributeData? attribute)
    {
        if (attribute is null || implSymbol.IsAbstract || IsOpenGeneric(implSymbol))
            return new ServiceModel();

        Lifetime lifetime = RequestedLifetime(attribute);

        INamedTypeSymbol? explicitAs = CheckForNamedTypeSymbol(attribute);

        var asSelf = false;
        foreach (KeyValuePair<string, TypedConstant> nv in attribute.NamedArguments)
        {
            if (nv.Key == "AsSelf" && nv.Value.Value is bool b)
                asSelf = b;
        }

        INamedTypeSymbol? serviceType = explicitAs ?? implSymbol.Interfaces.FirstOrDefault();
        serviceType ??= implSymbol;

        return new ServiceModel(implSymbol, serviceType, lifetime, asSelf);
    }

    private static Lifetime RequestedLifetime(AttributeData? attribute)
    {
        if (attribute is null || attribute.ConstructorArguments.Length == 0) return Lifetime.Scoped;

        TypedConstant arg = attribute.ConstructorArguments[0];
        if (arg is not { Kind: TypedConstantKind.Enum, Type.Name: nameof(Lifetime) }) return Lifetime.Scoped;
        switch (arg.Value)
        {
            case int i:
                return (Lifetime)i;
            case Lifetime l:
                return l;
        }

        if (arg.Value?.ToString() is { } s && Enum.TryParse<Lifetime>(s, out Lifetime parsed)) return parsed;

        return Lifetime.Scoped;
    }

    private static bool ConstructorTypesToIgnore(INamedTypeSymbol impl) => impl!.IsAbstract || impl.Arity != 0 || impl.DeclaredAccessibility != Accessibility.Public;

    private static INamedTypeSymbol? CheckForNamedTypeSymbol(AttributeData? attributeData)
    {
        if (attributeData is null) return null;
        INamedTypeSymbol? asType = null;
        foreach (KeyValuePair<string, TypedConstant> namedArgument in attributeData.NamedArguments)
        {
            if (namedArgument is not { Key: "As", Value.Value: INamedTypeSymbol ts }) continue;
            asType = ts;
            break;
        }

        return asType;
    }

    private static IncrementalValuesProvider<(INamedTypeSymbol namedTypeSymbol, AttributeData? attributeData)>
        FilterClassesToRegisterServiceAnnotated(IncrementalValuesProvider<INamedTypeSymbol?> classSymbols) =>
        classSymbols.Select((symbol, context) =>
        {
            if (symbol is null)
                return default;

            ImmutableArray<AttributeData> attributes = symbol.GetAttributes();
            AttributeData? matched = attributes.FirstOrDefault(IsRegisterServiceAttribute);

            return (symbol, matched);
        });

    private static IncrementalValuesProvider<INamedTypeSymbol?> SelectClassesWithAttributes(
        IncrementalGeneratorInitializationContext ctx) =>
        ctx.SyntaxProvider.CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 },
                static (syntaxCtx, _) =>
                {
                    var classDecl = (ClassDeclarationSyntax)syntaxCtx.Node;
                    INamedTypeSymbol? symbol = syntaxCtx.SemanticModel.GetDeclaredSymbol(classDecl);

                    return symbol;
                })
            .Where(static s => s is not null)!;

    private static bool IsRegisterServiceAttribute(AttributeData a)
    {
        INamedTypeSymbol? cls = a.AttributeClass;
        if (cls is null) return false;

        var fq = cls.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)?.Trim();
        if (!string.IsNullOrEmpty(fq) && fq!.StartsWith("global::", StringComparison.Ordinal))
            fq = fq.Substring("global::".Length);

        return string.Equals(fq, "AStar.Dev.Annotations.RegisterServiceAttribute", StringComparison.Ordinal)
               || string.Equals(fq, "AStar.Dev.Annotations.RegisterService", StringComparison.Ordinal);
    }

    private static string Generate(IReadOnlyList<ServiceModel> serviceModels)
    {
        ServiceModel[] filteredServiceModels =
            serviceModels.Where(s => s.ImplementationType.Name != "RegisterServiceAttribute").ToArray();
        if (filteredServiceModels.Length == 0) return string.Empty;

        var seen = new HashSet<string>(StringComparer.Ordinal);
        var sb = new StringBuilder();
        sb.AppendLine(Constants.SourceGeneratorHeader);
        sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
        sb.AppendLine();
        sb.AppendLine(Constants.GeneratedSourceXmlClassHeader);
        sb.AppendLine("public static class ServiceCollectionExtensions");
        sb.AppendLine("{");
        sb.AppendLine($"    {Constants.GeneratedSourceXmlHeader}");
        sb.AppendLine("    public static IServiceCollection AddApplicationServices(this IServiceCollection services)");
        sb.AppendLine("    {");

        foreach (ServiceModel m in filteredServiceModels)
        {
            var implFqn = m.ImplementationType.ToDisplayString(EmitFormat);
            var serviceFqn = (m.ServiceType ?? m.ImplementationType).ToDisplayString(EmitFormat);

            var method = m.Lifetime switch
            {
                Lifetime.Singleton => "AddSingleton",
                Lifetime.Scoped => "AddScoped",
                Lifetime.Transient => "AddTransient",
                _ => "AddScoped"
            };

            // If AsSelf requested and ServiceType is different, emit both concrete and service->implementation registrations
            if (m is { AlsoAsSelf: true, ServiceType: not null } &&
                !SymbolEqualityComparer.Default.Equals(m.ServiceType, m.ImplementationType))
            {
                sb.AppendLine($"        services.{method}<{implFqn}>();");
                sb.AppendLine($"        services.{method}<{serviceFqn}, {implFqn}>();");
            }
            else if (m.ServiceType is null || SymbolEqualityComparer.Default.Equals(m.ServiceType, m.ImplementationType))
                sb.AppendLine($"        services.{method}<{implFqn}>();");
            else
                sb.AppendLine($"        services.{method}<{serviceFqn}, {implFqn}>();");
        }
        
        sb.AppendLine();
        sb.AppendLine("        return services;");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    private sealed class ServiceModel
    {
        // ReSharper disable once ConvertToPrimaryConstructor
        public ServiceModel(INamedTypeSymbol implementationType,
            INamedTypeSymbol? serviceType,
            Lifetime lifetime,
            bool asSelf)
        {
            Lifetime = lifetime;
            ImplementationType = implementationType;
            AlsoAsSelf = asSelf;
            ServiceType = serviceType;
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public ServiceModel()
        {
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        public Lifetime Lifetime { get; }
        public INamedTypeSymbol ImplementationType { get; }
        public INamedTypeSymbol? ServiceType { get; }
        public bool AlsoAsSelf { get; }
    }
}
