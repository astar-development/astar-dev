using Xunit;

namespace AStar.Dev.Test.Helpers;

/// <summary>
///     Indicates that a method is used for making assertions in a test scenario.
/// </summary>
/// <remarks>
///     This attribute can be applied to methods to mark them as assertion methods, which are typically used
///     to verify the expected outcomes in unit tests. Methods marked with this attribute are used to confirm
///     the results of the test operations.
/// </remarks>
/// <seealso cref="FactAttribute" />
[AttributeUsage(AttributeTargets.Method)]
public class FactAssertionAttribute : FactAttribute
{
}
