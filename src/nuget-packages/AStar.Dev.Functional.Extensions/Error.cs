namespace AStar.Dev.Functional.Extensions;

/// <summary>
/// </summary>
/// <param name="Message"></param>
public record Error(string Message)
{
    /// <summary>
    ///     Do we need this?
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    public static implicit operator Error(string error) =>
        new (error);

    /// <summary>
    ///     Do we need this?
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    public static implicit operator string(Error error) =>
        error.Message;
}
