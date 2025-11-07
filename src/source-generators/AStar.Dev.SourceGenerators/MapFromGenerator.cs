using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AStar.Dev.SourceGenerators;

[Generator]
public sealed class MapFromGenerator : IIncrementalGenerator
{
    private const string AttrFqn = "AStar.Dev.Annotations.MapFromAttribute";

#pragma warning disable RS2008
    private static readonly DiagnosticDescriptor MissingPropertyDiag = new(
        "MAP001",
        "Destination property has no matching source",
        "Property '{0}' on destination type '{1}' has no matching readable property on source type '{2}'",
        "Mapping",
        DiagnosticSeverity.Error,
        true);

    private static readonly DiagnosticDescriptor IncompatibleTypeDiag = new(
        "MAP002",
        "Property types are incompatible",
        "Cannot assign source '{0}.{1}' (type '{2}') to destination '{3}.{4}' (type '{5}')",
        "Mapping",
        DiagnosticSeverity.Error,
        true);
#pragma warning restore RS2008

    public void Initialize(IncrementalGeneratorInitializationContext ctx)
    {
        // Find [MapFrom(typeof(...))] on classes/structs (syntax gate is cheap)
        IncrementalValuesProvider<MapModel?> stream = ctx.SyntaxProvider.ForAttributeWithMetadataName(
                AttrFqn,
                static (node, _) =>
                    node is TypeDeclarationSyntax { AttributeLists.Count: > 0 } and (ClassDeclarationSyntax or StructDeclarationSyntax),
                static (attrCtx, _) =>
                {
                    var dest = (INamedTypeSymbol)attrCtx.TargetSymbol;
                    if (dest.DeclaredAccessibility != Accessibility.Public || dest.Arity != 0)
                        return null;

                    AttributeData attr = attrCtx.Attributes[0]; // semantic match: our attribute
                    if (attr.ConstructorArguments.Length != 1) return null;
                    var srcType = attr.ConstructorArguments[0].Value as INamedTypeSymbol;
                    if (srcType is null) return null;

                    // Collect public, settable destination properties
                    IPropertySymbol[] destProps = dest.GetMembers().OfType<IPropertySymbol>()
                        .Where(p => p.DeclaredAccessibility == Accessibility.Public && p is { IsStatic: false, SetMethod: not null })
                        .ToArray();

                    // Collect public, gettable source properties
                    IPropertySymbol[] srcProps = srcType.GetMembers().OfType<IPropertySymbol>()
                        .Where(p => p.DeclaredAccessibility == Accessibility.Public && p is { IsStatic: false, GetMethod: not null })
                        .ToArray();

                    var ns = dest.ContainingNamespace.IsGlobalNamespace
                        ? null
                        : dest.ContainingNamespace.ToDisplayString();
                    SyntaxNode? attrSyntax = attr.ApplicationSyntaxReference?.GetSyntax();
                    Location attrLocation = attrSyntax?.GetLocation() ?? Location.None;

                    return new MapModel(ns, dest, srcType, destProps, srcProps, attrLocation);
                })
            .Where(static m => m is not null)!;

        // Emit per destination type; also report diagnostics
        ctx.RegisterSourceOutput(stream, static (spc, model) =>
        {
            // Diagnostics: missing or incompatible properties
            (List<Diagnostic> missing, List<Diagnostic> incompatible) = Analyze(model!);

            foreach (Diagnostic? d in missing) spc.ReportDiagnostic(d);
            foreach (Diagnostic? d in incompatible) spc.ReportDiagnostic(d);

            // If there are errors, skip codegen to avoid cascading failures
            if (missing.Count != 0 || incompatible.Count != 0) return;

            spc.AddSource($"{model!.Dest.Name}.Mapping.g.cs", Emit(model));
        });
    }

    private static (List<Diagnostic> missing, List<Diagnostic> incompatible) Analyze(MapModel m)
    {
        var missing = new List<Diagnostic>();
        var incompatible = new List<Diagnostic>();

        foreach (IPropertySymbol dp in m.DestProps)
        {
            IPropertySymbol? srcProp = m.SrcProps.FirstOrDefault(p => p.Name == dp.Name);

            // Prefer the destination property's source location; fall back to the [MapFrom] attribute
            Location loc = dp.Locations.FirstOrDefault(l => l.IsInSource)
                           ?? m.AttrLocation
                           ?? Location.None;

            if (srcProp is null)
            {
                missing.Add(Diagnostic.Create(
                    MissingPropertyDiag, loc,
                    dp.Name, m.Dest.Name, m.Src.Name));
                continue;
            }

            // exact type match → OK
            if (SymbolEqualityComparer.Default.Equals(dp.Type, srcProp.Type)) continue;

            // allow mapping to string via ToString()
            var destIsString = dp.Type.SpecialType == SpecialType.System_String;
            if (destIsString) continue;

            incompatible.Add(Diagnostic.Create(
                IncompatibleTypeDiag, loc,
                m.Src.Name, srcProp.Name, srcProp.Type.ToDisplayString(),
                m.Dest.Name, dp.Name, dp.Type.ToDisplayString()));
        }

        return (missing, incompatible);
    }

    private static string Emit(MapModel m)
    {
        var ns = m.Namespace is null ? null : $"namespace {m.Namespace};";
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated/>");
        if (ns is not null) sb.AppendLine(ns).AppendLine();

        var destName = m.Dest.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        var srcName = m.Src.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

        sb.AppendLine("using System;");
        sb.AppendLine();
        sb.AppendLine($"public static partial class {m.Dest.Name}Mapper");
        sb.AppendLine("{");
        sb.AppendLine($"    public static {destName} To{m.Dest.Name}(this {srcName} src)");
        sb.AppendLine("    {");
        sb.AppendLine($"        var dest = new {destName}();");

        foreach (IPropertySymbol dp in m.DestProps)
        {
            IPropertySymbol? sp = m.SrcProps.FirstOrDefault(p => p.Name == dp.Name);
            if (sp is null) continue; // unreachable if diagnostics prevented emit

            var assignExpr =
                dp.Type.SpecialType == SpecialType.System_String &&
                dp.Type.SpecialType != sp.Type.SpecialType
                    ? $"src.{sp.Name}?.ToString()"
                    : $"src.{sp.Name}";

            sb.AppendLine($"        dest.{dp.Name} = {assignExpr};");
        }

        // partial hook for customization
        sb.AppendLine("        OnAfterMap(src, ref dest);");
        sb.AppendLine("        return dest;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine($"    static partial void OnAfterMap(in {srcName} src, ref {destName} dest);");
        sb.AppendLine("}");
        return sb.ToString();
    }

    private sealed class MapModel(
        string? ns,
        INamedTypeSymbol dest,
        INamedTypeSymbol src,
        IPropertySymbol[] destProps,
        IPropertySymbol[] srcProps,
        Location attrLocation)
    {
        public string? Namespace { get; } = ns;
        public INamedTypeSymbol Dest { get; } = dest;
        public INamedTypeSymbol Src { get; } = src;
        public IPropertySymbol[] DestProps { get; } = destProps;
        public IPropertySymbol[] SrcProps { get; } = srcProps;
        public Location AttrLocation { get; } = attrLocation;
    }
}
