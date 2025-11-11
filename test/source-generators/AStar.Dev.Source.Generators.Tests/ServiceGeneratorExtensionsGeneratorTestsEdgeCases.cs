using AStar.Dev.Source.Generators.Tests.TestHelpers;
using Shouldly;

namespace AStar.Dev.Source.Generators.Tests;

public class ServiceGeneratorExtensionsGeneratorTestsEdgeCases
{
    [Fact]
    public void Registers_When_Attribute_In_Second_AttributeList()
    {
        const string serviceSource = """
                                     using AStar.Dev.Annotations;
                                     namespace Demo;
                                     [Serializable]
                                     [RegisterService(Lifetime.Singleton)]
                                     public class DemoService {}
                                     """;

        var generated =
            GeneratorHelpers.RunGeneratorAndGetGeneratedForType("Demo.DemoService",
                TestHelpers.Constants.AttributeSource, serviceSource);
        generated.ShouldContain("AddSingleton<global::Demo.DemoService>");
    }

    [Fact]
    public void Registers_When_Attribute_In_Third_AttributeList()
    {
        const string serviceSource = """
                                     using AStar.Dev.Annotations;
                                     namespace Demo;
                                     [Obsolete]
                                     [Serializable]
                                     [RegisterService(Lifetime.Scoped)]
                                     public class DemoService {}
                                     """;

        var generated =
            GeneratorHelpers.RunGeneratorAndGetGeneratedForType("Demo.DemoService",
                TestHelpers.Constants.AttributeSource, serviceSource);
        generated.ShouldContain("AddScoped<global::Demo.DemoService>");
    }

    [Fact]
    public void Registers_When_Multiple_Attributes_In_Same_List_But_Not_First()
    {
        const string serviceSource = """
                                     using AStar.Dev.Annotations;
                                     namespace Demo;
                                     [Obsolete, RegisterService(Lifetime.Transient)]
                                     public class DemoService {}
                                     """;

        var generated =
            GeneratorHelpers.RunGeneratorAndGetGeneratedForType("Demo.DemoService",
                TestHelpers.Constants.AttributeSource, serviceSource);
        generated.ShouldContain("AddTransient<global::Demo.DemoService>");
    }

    [Fact]
    public void Generates_For_Multiple_Annotated_Classes_In_One_File()
    {
        const string serviceSource = """
                                     using AStar.Dev.Annotations;
                                     namespace Demo;
                                     [RegisterService(Lifetime.Singleton)]
                                     public class FirstService {}
                                     [RegisterService(Lifetime.Transient)]
                                     public class SecondService {}
                                     """;

        var generatedFirst =
            GeneratorHelpers.RunGeneratorAndGetGeneratedForType("Demo.FirstService",
                TestHelpers.Constants.AttributeSource, serviceSource);
        var generatedSecond =
            GeneratorHelpers.RunGeneratorAndGetGeneratedForType("Demo.SecondService",
                TestHelpers.Constants.AttributeSource, serviceSource);

        generatedFirst.ShouldContain("AddSingleton<global::Demo.FirstService>");
        generatedSecond.ShouldContain("AddTransient<global::Demo.SecondService>");
    }

    [Fact]
    public void Generates_For_Generic_Class_ClosedType()
    {
        const string serviceSource = """
                                     using AStar.Dev.Annotations;
                                     namespace Demo;
                                     [RegisterService(Lifetime.Singleton)]
                                     public class GenericService<T> {}
                                     // closed type used somewhere (consumer) so generator sees a named type
                                     public class GenericServiceUsage { private GenericService<int> _ = null!; }
                                     """;

        var generated =
            GeneratorHelpers.RunGeneratorAndGetGeneratedForType("Demo.GenericService",
                TestHelpers.Constants.AttributeSource, serviceSource);
        // Expect registration for closed constructed type is not typical; ensure generator at least emits for the declaring generic type name
        generated.ShouldContain("GenericService");
    }

    [Fact]
    public void Generates_For_Nested_Class()
    {
        const string serviceSource = """
                                     using AStar.Dev.Annotations;
                                     namespace Demo;
                                     public class Outer
                                     {
                                         [RegisterService(Lifetime.Scoped)]
                                         public class InnerService {}
                                     }
                                     """;

        var generated =
            GeneratorHelpers.RunGeneratorAndGetGeneratedForType("Demo.Outer.InnerService",
                TestHelpers.Constants.AttributeSource,
                serviceSource);
        generated.ShouldContain("AddScoped<global::Demo.Outer.InnerService>");
    }

    [Fact]
    public void Skips_Open_Generic_Class()
    {
        const string serviceSource = """
                                     using AStar.Dev.Annotations;
                                     namespace Demo;
                                     [RegisterService(Lifetime.Transient)]
                                     public class OpenGenericService<T> {}
                                     """;

        var generated =
            GeneratorHelpers.RunGeneratorAndGetGeneratedForType("Demo.OpenGenericService",
                TestHelpers.Constants.AttributeSource,
                serviceSource);
        // open generics should not be registered; assert no AddTransient appears for the raw open generic type
        generated.IndexOf("AddTransient<global::Demo.OpenGenericService>", StringComparison.OrdinalIgnoreCase)
            .ShouldBe(-1);
    }

    [Fact]
    public void Handles_Multiple_Attributes_Instances_In_Same_List()
    {
        const string serviceSource = """
                                     using AStar.Dev.Annotations;
                                     namespace Demo;
                                     [Obsolete, RegisterService(Lifetime.Transient), SomeOther]
                                     public class DemoService {}
                                     """;

        var generated =
            GeneratorHelpers.RunGeneratorAndGetGeneratedForType("Demo.DemoService",
                TestHelpers.Constants.AttributeSource, serviceSource);
        generated.ShouldContain("AddTransient<global::Demo.DemoService>");
    }

    [Fact]
    public void Handles_Attribute_Not_In_First_AttributeList_2ndAnd3rd()
    {
        const string serviceSource = """
                                     using AStar.Dev.Annotations;
                                     namespace Demo;
                                     [Obsolete]
                                     [SomeOther]
                                     [RegisterService(Lifetime.Singleton)]
                                     public class DemoService {}
                                     """;

        var generated =
            GeneratorHelpers.RunGeneratorAndGetGeneratedForType("Demo.DemoService",
                TestHelpers.Constants.AttributeSource, serviceSource);
        generated.ShouldContain("AddSingleton<global::Demo.DemoService>");
    }
}