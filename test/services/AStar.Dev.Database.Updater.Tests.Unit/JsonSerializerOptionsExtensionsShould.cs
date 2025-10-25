// using System.Text.Encodings.Web;
// using System.Text.Json;
// using System.Text.Json.Serialization;
//
// namespace AStar.Dev.Database.Updater.Tests.Unit;
//
// [TestSubject(typeof(JsonSerializerOptionsExtensions))]
// public class JsonSerializerOptionsExtensionsTest
// {
//     [Fact]
//     public void CreateJsonConfigureOptions_ShouldSetPropertyNamingPolicyToCamelCase()
//     {
//         var options = new JsonSerializerOptions();
//
//         JsonSerializerOptionsExtensions.CreateJsonConfigureOptions(options);
//
//         options.PropertyNamingPolicy.ShouldBe(JsonNamingPolicy.CamelCase);
//     }
//
//     [Fact]
//     public void CreateJsonConfigureOptions_ShouldSetDefaultIgnoreConditionToWhenWritingNull()
//     {
//         var options = new JsonSerializerOptions();
//
//         JsonSerializerOptionsExtensions.CreateJsonConfigureOptions(options);
//
//         options.DefaultIgnoreCondition.ShouldBe(JsonIgnoreCondition.WhenWritingNull);
//     }
//
//     [Fact]
//     public void CreateJsonConfigureOptions_ShouldEnableIndentedWriting()
//     {
//         var options = new JsonSerializerOptions();
//
//         JsonSerializerOptionsExtensions.CreateJsonConfigureOptions(options);
//
//         options.WriteIndented.ShouldBeTrue();
//     }
//
//     [Fact]
//     public void CreateJsonConfigureOptions_ShouldSetEncoderToDefault()
//     {
//         var options = new JsonSerializerOptions();
//
//         JsonSerializerOptionsExtensions.CreateJsonConfigureOptions(options);
//
//         options.Encoder.ShouldBe(JavaScriptEncoder.Default);
//     }
//
//     [Fact]
//     public void CreateJsonConfigureOptions_ShouldAllowTrailingCommas()
//     {
//         var options = new JsonSerializerOptions();
//
//         JsonSerializerOptionsExtensions.CreateJsonConfigureOptions(options);
//
//         options.AllowTrailingCommas.ShouldBeTrue();
//     }
//
//     [Fact]
//     public void CreateJsonConfigureOptions_ShouldSetNumberHandlingToAllowReadingFromString()
//     {
//         var options = new JsonSerializerOptions();
//
//         JsonSerializerOptionsExtensions.CreateJsonConfigureOptions(options);
//
//         options.NumberHandling.HasFlag(JsonNumberHandling.AllowReadingFromString).ShouldBeTrue();
//     }
//
//     [Fact]
//     public void CreateJsonConfigureOptions_ShouldSetPropertyNameCaseInsensitiveToTrue()
//     {
//         var options = new JsonSerializerOptions();
//
//         JsonSerializerOptionsExtensions.CreateJsonConfigureOptions(options);
//
//         options.PropertyNameCaseInsensitive.ShouldBeTrue();
//     }
//
//     [Fact]
//     public void CreateJsonConfigureOptions_ShouldSetReferenceHandlerToIgnoreCycles()
//     {
//         var options = new JsonSerializerOptions();
//
//         JsonSerializerOptionsExtensions.CreateJsonConfigureOptions(options);
//
//         options.ReferenceHandler.ShouldBe(ReferenceHandler.IgnoreCycles);
//     }
//
//     [Fact]
//     public void CreateJsonConfigureOptions_ShouldNotBeNullOrAlterUnrelatedProperties()
//     {
//         var options = new JsonSerializerOptions();
//
//         JsonSerializerOptionsExtensions.CreateJsonConfigureOptions(options);
//
//         options.ShouldNotBeNull();
//         options.IgnoreReadOnlyProperties.ShouldBeFalse(); // Default behavior remains unchanged
//     }
// }

