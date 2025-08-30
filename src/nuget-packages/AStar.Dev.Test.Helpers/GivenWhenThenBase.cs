using Xunit.Abstractions;

namespace AStar.Dev.Test.Helpers;

/// <summary>
///     Abstract base class for implementing the Given-When-Then testing pattern.
/// </summary>
public abstract class GivenWhenThenBase(string scenarioDescription)
{
    /// <summary>
    ///     Gets or sets the test output helper used to write messages to the test output,
    ///     such as scenario descriptions, steps, and assertions, during the test execution.
    /// </summary>
    /// <remarks>
    ///     This property is set using the <see cref="WithTestOutputHelper" /> method to associate an <see cref="ITestOutputHelper" /> instance with the current test case.
    /// </remarks>
    protected ITestOutputHelper? TestOutputHelper { get; private set; }

    /// <summary>
    ///     Associates the current test scenario with an <see cref="ITestOutputHelper" />
    ///     to enable writing scenario descriptions and steps to test output.
    /// </summary>
    /// <param name="testOutputHelper">The test output helper to write scenario information to the test output.</param>
    /// <returns>The current instance of <see cref="GivenWhenThenBase" /> for method chaining.</returns>
    public GivenWhenThenBase WithTestOutputHelper(ITestOutputHelper testOutputHelper)
    {
        TestOutputHelper = testOutputHelper;
        TestOutputHelper?.WriteLine($"Scenario:{Environment.NewLine}\t{scenarioDescription}{Environment.NewLine}");

        return this;
    }

    /// <summary>
    ///     Specifies the initial condition or setup for the test scenario in the Given-When-Then testing pattern.
    /// </summary>
    /// <param name="setupDescription">A description of the setup or initial condition being established.</param>
    /// <param name="performTheGivenSteps">An action that performs the steps required to establish the given setup.</param>
    /// <returns>The current instance of <see cref="GivenWhenThenBase" /> for method chaining.</returns>
    public abstract GivenWhenThenBase Given(string setupDescription, Action performTheGivenSteps);

    /// <summary>
    ///     Executes the specified action and associates it with the "When" step of the testing scenario.
    /// </summary>
    /// <param name="actionDescription">A description of the action or operation being performed during the "When" step.</param>
    /// <param name="performTheWhenSteps">The action representing the steps to be executed during the "When" phase of the test.</param>
    /// <returns>The current instance of <see cref="GivenWhenThenBase" /> for method chaining.</returns>
    public abstract GivenWhenThenBase When(string actionDescription, Action performTheWhenSteps);

    /// <summary>
    ///     Specifies the outcome and confirms the expected result of the test scenario.
    /// </summary>
    /// <param name="outcomeDescription">A description of the expected outcome for the scenario.</param>
    /// <param name="confirmTheExpectedResult">The action that confirms the expected result of the scenario.</param>
    public abstract void Then(string outcomeDescription, Action confirmTheExpectedResult);
}
