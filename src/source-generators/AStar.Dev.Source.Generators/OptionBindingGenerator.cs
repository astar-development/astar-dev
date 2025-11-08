using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AStar.Dev.Source.Generators;

[Generator]
public sealed class OptionsBindingGenerator : IIncrementalGenerator
{
    private const string AttrFqn = "AStar.Dev.Annotations.ConfigSectionAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext ctx)
    {
        // A) Discover [ConfigSection] classes (syntax-only gate → semantic match)
        IncrementalValuesProvider<OptionsModel?> optionsTypes = ctx.SyntaxProvider.ForAttributeWithMetadataName(
                AttrFqn,
                static (node, _) =>
                    node is ClassDeclarationSyntax c &&
                    c.Modifiers.Any(m => m.Text == "partial") &&
                    c.AttributeLists.Count > 0,
                static (syntaxCtx, _) =>
                {
                    var type = (INamedTypeSymbol)syntaxCtx.TargetSymbol;

                    // Skip non-public/abstract/generic types
                    if (type.DeclaredAccessibility != Accessibility.Public ||
                        type.IsAbstract || type.Arity != 0)
                        return null;

                    AttributeData attr = syntaxCtx.Attributes[0];

                    // [ConfigSection("Payments")]
                    var section = "Options";
                    if (attr.ConstructorArguments.Length == 1 &&
                        attr.ConstructorArguments[0].Value is string s && !string.IsNullOrWhiteSpace(s))
                        section = s;

                    // Collect simple property info so we can emit defaults
                    PropModel[] props = type.GetMembers()
                        .OfType<IPropertySymbol>()
                        .Where(p => p.DeclaredAccessibility == Accessibility.Public && !p.IsStatic)
                        .Select(p =>
                        {
                            SimpleKind sk = GetSimpleKind(p.Type);
                            return new PropModel(p.Name, sk);
                        })
                        .ToArray();

                    var ns = type.ContainingNamespace.IsGlobalNamespace
                        ? null
                        : type.ContainingNamespace.ToDisplayString();

                    return new OptionsModel(
                        ns,
                        type.Name,
                        section,
                        props
                    );
                })
            .Where(static m => m is not null);

        // B) Read AdditionalFiles (all *.options.schema) → collect all entries
        IncrementalValueProvider<ImmutableArray<SchemaFile>> schemaFiles = ctx.AdditionalTextsProvider
            .Where(static f => f.Path.EndsWith("options.schema", StringComparison.OrdinalIgnoreCase))
            .Select(static (text, ct) => ParseSchema(text))
            .Collect();

        // C) Pair each options type with the current schema set
        IncrementalValuesProvider<(OptionsModel? Left, ImmutableArray<SchemaFile> Right)> paired = optionsTypes.Combine(schemaFiles);

        // D) Emit one file per options type (incremental per-type)
        ctx.RegisterSourceOutput(paired, static (spc, pair) =>
        {
            (OptionsModel? model, ImmutableArray<SchemaFile> allSchemas) = pair;
            Dictionary<string, Dictionary<string, SchemaEntry>> dict = BuildSchemaDictionary(allSchemas); // Section -> (PropName -> Entry)
            var code = Generate(model!, dict);
            spc.AddSource($"{model!.TypeName}.OptionsBinding.g.cs", code);
        });
    }

    // ---- helpers -------------------------------------------------------------

    private static string Generate(OptionsModel m, Dictionary<string, Dictionary<string, SchemaEntry>> schema)
    {
        var ns = m.Namespace is null ? null : $"namespace {m.Namespace};";
        var sb = new StringBuilder();
        if (ns is not null)
            sb.AppendLine(Constants.SourceGeneratorHeader).AppendLine(ns).AppendLine();
        else sb.AppendLine(Constants.SourceGeneratorHeader);

        // Look up defaults/required for this section
        schema.TryGetValue(m.SectionName, out Dictionary<string, SchemaEntry>? sectionMap);
        sectionMap ??= new Dictionary<string, SchemaEntry>(StringComparer.Ordinal);

        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using System.ComponentModel.DataAnnotations;");
        sb.AppendLine("using Microsoft.Extensions.Configuration;");
        sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
        sb.AppendLine("using Microsoft.Extensions.Options;");
        sb.AppendLine();
        sb.AppendLine($"public static class {m.TypeName}OptionsRegistration");
        sb.AppendLine("{");
        sb.AppendLine(
            $"    public static IServiceCollection Add{m.TypeName}(this IServiceCollection s, IConfiguration cfg)");
        sb.AppendLine("    {");
        sb.AppendLine($"        var section = cfg.GetSection(\"{Escape(m.SectionName)}\");");
        sb.AppendLine($"        var opts = section.Get<{m.TypeName}>() ?? new {m.TypeName}();");

        // Apply defaults from schema (only simple types)
        foreach (PropModel p in m.Properties)
        {
            if (sectionMap.TryGetValue(p.Name, out SchemaEntry? entry) && entry.DefaultValue is string dv)
            {
                var assign = DefaultAssignment(p, dv);
                if (assign is not null) sb.AppendLine(assign);
            }
        }

        // Validate with DataAnnotations
        sb.AppendLine("        var results = new List<ValidationResult>();");
        sb.AppendLine(
            "        var ok = Validator.TryValidateObject(opts, new ValidationContext(opts), results, validateAllProperties: true);");

// Add extra 'required' checks from schema (lightweight)
        foreach (PropModel p in m.Properties)
        {
            if (sectionMap.TryGetValue(p.Name, out SchemaEntry? entry) && entry.IsRequired)
            {
                switch (p.Kind)
                {
                    case SimpleKind.String:
                        sb.AppendLine(
                            "        if (string.IsNullOrWhiteSpace(opts." + p.Name +
                            ")) { ok = false; results.Add(new ValidationResult(\"" +
                            p.Name + " is required\", new[]{\"" + p.Name + "\"})); }");
                        break;

                    case SimpleKind.Int32:
                        sb.AppendLine(
                            "        if (opts." + p.Name +
                            " == default(int)) { ok = false; results.Add(new ValidationResult(\"" +
                            p.Name + " must be non-zero\", new[]{\"" + p.Name + "\"})); }");
                        break;

                    case SimpleKind.Boolean:
                        sb.AppendLine(
                            "        /* schema-required boolean: ensure it's explicitly set if that matters to you */");
                        break;
                }
            }
        }

        sb.AppendLine("        if (!ok)");
        sb.AppendLine(
            $"            throw new OptionsValidationException(\"{m.TypeName}\", typeof({m.TypeName}), results.ConvertAll(r => r.ErrorMessage ?? \"Invalid\"));");

        // Register binding with DI so IOptions<T> works elsewhere
        sb.AppendLine($"        s.Configure<{m.TypeName}>(section);");
        sb.AppendLine("        return s;");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    private static string? DefaultAssignment(PropModel p, string defaultLiteral)
    {
        // Our schema format uses simple literals: strings, ints, bools
        switch (p.Kind)
        {
            case SimpleKind.String:
                return
                    $"        if (string.IsNullOrWhiteSpace(opts.{p.Name})) opts.{p.Name} = {ToCSharpString(defaultLiteral)};";
            case SimpleKind.Int32:
                if (int.TryParse(defaultLiteral, out _))
                    return $"        if (opts.{p.Name} == default(int)) opts.{p.Name} = {defaultLiteral};";
                return null;
            case SimpleKind.Boolean:
                if (bool.TryParse(defaultLiteral, out _))
                {
                    return
                        $"        /* boolean default from schema */ if (opts.{p.Name} == default(bool)) opts.{p.Name} = {defaultLiteral.ToLowerInvariant()};";
                }

                return null;
            default:
                return null;
        }
    }

    private static string ToCSharpString(string s)
    {
        return "\"" + s.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
    }

    private static string Escape(string s)
    {
        return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }

    private static Dictionary<string, Dictionary<string, SchemaEntry>> BuildSchemaDictionary(
        ImmutableArray<SchemaFile> files)
    {
        var map = new Dictionary<string, Dictionary<string, SchemaEntry>>(StringComparer.Ordinal);
        foreach (SchemaFile? f in files)
        foreach (SchemaEntry? e in f.Entries)
        {
            if (!map.TryGetValue(e.Section, out Dictionary<string, SchemaEntry>? props))
            {
                props = new Dictionary<string, SchemaEntry>(StringComparer.Ordinal);
                map[e.Section] = props;
            }

            props[e.Property] = e; // last write wins
        }

        return map;
    }

    private static SchemaFile ParseSchema(AdditionalText text)
    {
        // Very simple "ini-like" schema format per line:
        // Section:Property=default:VALUE
        // Section:Property=required
        // e.g. Payments:ApiKey=required
        //      Payments:TimeoutSeconds=default:30
        var entries = new List<SchemaEntry>();
        var content = text.GetText()?.ToString() ?? "";
        var lines = content.Split(["\r\n", "\n"], StringSplitOptions.None);
        foreach (var raw in lines)
        {
            var line = raw.Trim();
            if (line.Length == 0 || line.StartsWith("#", StringComparison.Ordinal)) continue;
            var parts = line.Split(['='], 2);
            if (parts.Length != 2) continue;

            var left = parts[0].Trim(); // Section:Property
            var right = parts[1].Trim(); // required | default:VALUE

            var sp = left.Split([':'], 2);
            if (sp.Length != 2) continue;

            var section = sp[0].Trim();
            var prop = sp[1].Trim();
            var isReq = false;
            string? def = null;

            if (right.Equals("required", StringComparison.OrdinalIgnoreCase))
                isReq = true;
            else if (right.StartsWith("default:", StringComparison.OrdinalIgnoreCase))
                def = right.Substring("default:".Length).Trim();

            if (!string.IsNullOrEmpty(section) && !string.IsNullOrEmpty(prop))
                entries.Add(new SchemaEntry(section, prop, isReq, def));
        }

        return new SchemaFile(text.Path, entries);
    }

    // ---- small models -------------------------------------------------------

    private static SimpleKind GetSimpleKind(ITypeSymbol t)
    {
        switch (t)
        {
            case { SpecialType: SpecialType.System_String }: return SimpleKind.String;
            case { SpecialType: SpecialType.System_Int32 }: return SimpleKind.Int32;
            case { SpecialType: SpecialType.System_Boolean }: return SimpleKind.Boolean;
            default: return SimpleKind.Other;
        }
    }

    private enum SimpleKind
    {
        String,
        Int32,
        Boolean,
        Other
    }

    private sealed class PropModel(string name, SimpleKind kind)
    {
        public string Name { get; } = name;
        public SimpleKind Kind { get; } = kind;
    }

    private sealed class OptionsModel(string? @namespace, string typeName, string sectionName, PropModel[] properties)
    {
        public string? Namespace { get; } = @namespace;
        public string TypeName { get; } = typeName;
        public string SectionName { get; } = sectionName;
        public PropModel[] Properties { get; } = properties;
    }

    private sealed class SchemaFile(string path, List<SchemaEntry> entries)
    {
        public string Path { get; } = path;
        public List<SchemaEntry> Entries { get; } = entries;
    }

    private sealed class SchemaEntry(string section, string property, bool isRequired, string? defaultValue)
    {
        public string Section { get; } = section;
        public string Property { get; } = property;
        public bool IsRequired { get; } = isRequired;
        public string? DefaultValue { get; } = defaultValue;
    }
}
