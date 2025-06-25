namespace AStar.Dev.Functional.Extensions;

/// <summary>
/// </summary>
/// <typeparam name="T">The type of the parameter</typeparam>
public sealed class Some<T> : Option<T>
{
    /// <summary>
    /// </summary>
    /// <param name="content"></param>
    public Some(T content) =>
        Content = content;

    /// <summary>
    /// </summary>
    public T Content { get; }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public override string ToString() =>
        $"Some {Content?.ToString() ?? "<null>"}";
}
