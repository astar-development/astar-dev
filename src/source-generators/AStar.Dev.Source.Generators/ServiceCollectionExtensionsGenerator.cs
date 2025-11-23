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
    private const string AttributeFqn = $"RegisterService";
    private const string AttributeFqnSecondaryNaming = $"RegisterServiceAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext incrementalContext)
    {
        IncrementalValuesProvider<INamedTypeSymbol?> classSyntax = SelectClassesWithAttributes(incrementalContext);

        IncrementalValuesProvider<(INamedTypeSymbol namedTypeSymbol, AttributeData? attributeData)> services = FilterClassesToRegisterServiceAnnotated(classSyntax);

        IncrementalValuesProvider<ServiceModel?> serviceModels = CreateServiceRegistrationsModel(services);

        // 4) Combine with Compilation and collect to one batch
        IncrementalValueProvider<(Compilation Left, ImmutableArray<ServiceModel?> Right)> combined = incrementalContext.CompilationProvider.Combine(serviceModels.Collect());

        incrementalContext.RegisterSourceOutput(combined, static (spc, pair) =>
        {
            (_, ImmutableArray<ServiceModel?> batch) = pair;
            var code = Generate(batch);
            spc.AddSource("ServiceCollectionExtensions.g.cs", code);
        });
    }

    private static IncrementalValuesProvider<ServiceModel?> CreateServiceRegistrationsModel(IncrementalValuesProvider<(INamedTypeSymbol namedTypeSymbol, AttributeData? attributeData)> services)
        => services.Select(static (tuple, _) =>
        {
            (INamedTypeSymbol? implementation, AttributeData? attribute) = tuple;

            Lifetime lifetime = Lifetime.Scoped;
            if (attribute!.ConstructorArguments.Length == 1 && attribute.ConstructorArguments[0].Value is int requestedLifetime) lifetime = (Lifetime)requestedLifetime;

            INamedTypeSymbol? asType = CheckForNamedTypeSymbol(attribute);

            var asSelf = CheckWhetherToRegisterAsSelf(attribute);

            if (ConstructorTypesToIgnore(implementation))
                return null;

            INamedTypeSymbol? inferred = CheckWhetherToRegisterAs(asType, implementation);

            INamedTypeSymbol? service = asType ?? inferred;

            return new ServiceModel(
                lifetime,
                implementation.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                service?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                asSelf
            );
        }).Where(static serviceModel => serviceModel is not null)!;

    private static INamedTypeSymbol? CheckWhetherToRegisterAs(INamedTypeSymbol? asType, INamedTypeSymbol implementation)
    {
        INamedTypeSymbol? inferred = null;
        if(asType is not null) return inferred;
        
        INamedTypeSymbol[] candidates = implementation.AllInterfaces
            .Where(i => i.DeclaredAccessibility == Accessibility.Public
                        && i is { TypeKind: TypeKind.Interface, Arity: 0 }
                        && i.ToDisplayString() != "System.IDisposable")
            .ToArray();
        if (candidates.Length == 1) inferred = candidates[0];

        return inferred;
    }

    private static bool ConstructorTypesToIgnore(INamedTypeSymbol impl) => impl!.IsAbstract || impl.Arity != 0 || impl.DeclaredAccessibility != Accessibility.Public;

    private static bool CheckWhetherToRegisterAsSelf(AttributeData attr)
    {
        var asSelf = false;
        foreach (KeyValuePair<string, TypedConstant> na in attr.NamedArguments)
        {
            if(na is not { Key: "AsSelf", Value.Value: bool b }) continue;
            asSelf = b;
            break;
        }

        return asSelf;
    }

    private static INamedTypeSymbol? CheckForNamedTypeSymbol(AttributeData attributeData)
    {
        INamedTypeSymbol? asType = null;
        foreach (KeyValuePair<string, TypedConstant> namedArgument in attributeData.NamedArguments)
        {
            if(namedArgument is not { Key: "As", Value.Value: INamedTypeSymbol ts }) continue;
            asType = ts;
            break;
        }

        return asType;
    }

    private static IncrementalValuesProvider<(INamedTypeSymbol namedTypeSymbol, AttributeData? attributeData)> FilterClassesToRegisterServiceAnnotated(IncrementalValuesProvider<INamedTypeSymbol?> classSyntax)
    => classSyntax
            .Select(static (sym, _) => ValueTuple(sym))
            .Where(static t => t.attr is not null);

    private static (INamedTypeSymbol sym, AttributeData? attr) ValueTuple(INamedTypeSymbol? sym)
    {
        ImmutableArray<AttributeData> attributeDatas = sym!.GetAttributes();
        AttributeData? attr = attributeDatas
            .FirstOrDefault(a =>
                a.AttributeClass?.ToDisplayString() == AttributeFqn ||
                a.AttributeClass?.ToDisplayString() == AttributeFqnSecondaryNaming);
        return (sym, attr);
    }

    private static IncrementalValuesProvider<INamedTypeSymbol?> SelectClassesWithAttributes(IncrementalGeneratorInitializationContext ctx)
        => ctx.SyntaxProvider.CreateSyntaxProvider(
                static (node, _) =>
                    node is ClassDeclarationSyntax { AttributeLists.Count: > 0, TypeParameterList: null },
                static (syntaxCtx, _) =>
                {
                    var classDecl = (ClassDeclarationSyntax)syntaxCtx.Node;
                    INamedTypeSymbol? symbol = syntaxCtx.SemanticModel.GetDeclaredSymbol(classDecl);
                    return symbol;
                })
            .Where(static s => s is not null)!;

    private static string Generate(IReadOnlyList<ServiceModel> serviceModels)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var sb = new StringBuilder();
        _ = sb.AppendLine(Constants.SourceGeneratorHeader);
        _ = sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
        _ = sb.AppendLine();
        _ = sb.AppendLine(Constants.GeneratedSourceXmlClassHeader);
        _ = sb.AppendLine("public static class ServiceCollectionExtensions");
        _ = sb.AppendLine("{");
        _ = sb.AppendLine($"    {Constants.GeneratedSourceXmlHeader}");
        _ = sb.AppendLine("    public static IServiceCollection AddGeneratedServices(this IServiceCollection services)");
        _ = sb.AppendLine("    {");

        foreach (ServiceModel? serviceModel in serviceModels)
        {
            var method = serviceModel.Lifetime switch
            {
                Lifetime.Singleton => "AddSingleton",
                Lifetime.Scoped => "AddScoped",
                Lifetime.Transient => "AddTransient",
                _ => "AddScoped"
            };

            // Register against interface if present; else self
            if (!string.IsNullOrEmpty(serviceModel.ServiceFqn))
            {
                var line = $"        services.{method}<{serviceModel.ServiceFqn}, {serviceModel.ImplFqn}>();";
                if (seen.Add(line))
                    _ = sb.AppendLine(line);
                if(!serviceModel.AlsoAsSelf) continue;
                
                var self = $"        services.{method}<{serviceModel.ImplFqn}>();";
                if (seen.Add(self))
                    _ = sb.AppendLine(self);
            }
            else
            {
                var self = $"        services.{method}<{serviceModel.ImplFqn}>();";
                if (seen.Add(self))
                    _ = sb.AppendLine(self);
            }
        }

        _ = sb.AppendLine("        return services;");
        _ = sb.AppendLine("    }");
        _ = sb.AppendLine("}");

        return sb.ToString();
    }

    private sealed class ServiceModel
    {
        public ServiceModel(Lifetime lifetime, string implFqn, string? serviceFqn, bool alsoAsSelf)
        {
            Lifetime = lifetime;
            ImplFqn = implFqn;
            ServiceFqn = serviceFqn;
            AlsoAsSelf = alsoAsSelf;
        }

        public Lifetime Lifetime { get; }
        public string ImplFqn { get; }
        public string? ServiceFqn { get; }
        public bool AlsoAsSelf { get; }
    }

    // Mirror the annotations enum here so we can parse constructor args without referencing the annotations assembly at runtime.
    private enum Lifetime
    {
        Singleton = 0,
        Scoped = 1,
        Transient = 2
    }
}
