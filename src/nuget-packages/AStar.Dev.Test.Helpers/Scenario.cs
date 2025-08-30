namespace AStar.Dev.Test.Helpers;

/// <summary>
///     Represents a specific implementation of the Given-When-Then testing pattern.
///     This class extends the functionality of <see cref="GivenWhenThenBase" />
///     and provides a concrete mechanism for outputting scenario information
///     while executing the sequence of test steps: Given, When, and Then.
/// </summary>
/// <remarks>
///     The <see cref="Scenario" /> class allows for better structuring and understanding of
///     test cases by adhering to the Given-When-Then testing methodology. Each method outputs
///     descriptive messages for each step, includes the scenario description, and executes the provided delegate actions.
/// </remarks>
public class Scenario(string scenarioDescription) : GivenWhenThenBase(scenarioDescription)
{
    /// <inheritdoc />
    public override GivenWhenThenBase Given(string setupDescription, Action performTheGivenSteps)
    {
        TestOutputHelper?.WriteLine($"\tGiven {setupDescription.ToLowerInvariant()}");
        performTheGivenSteps();

        return this;
    }

    /// <inheritdoc />
    public override GivenWhenThenBase When(string actionDescription, Action performTheWhenSteps)
    {
        TestOutputHelper?.WriteLine($"\tWhen {actionDescription.ToLowerInvariant()}");
        performTheWhenSteps();

        return this;
    }

    /// <inheritdoc />
    public override void Then(string outcomeDescription, Action confirmTheExpectedResult)
    {
        TestOutputHelper?.WriteLine($"\tThen {outcomeDescription.ToLowerInvariant()}");
        confirmTheExpectedResult();
        TestOutputHelper?.WriteLine("\tThe assertion passed (YAY!!!!)");
    }
}
