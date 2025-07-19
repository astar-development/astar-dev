namespace AStar.Dev.Functional.Extensions;

/// <summary>
///     Provides pattern-style checks across Option, Result, and Try.
/// </summary>
public static class Pattern
{
    /// <summary>
    /// </summary>
    /// <param name="option"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool IsSome<T>(Option<T> option) => option is Option<T>.Some;

    /// <summary>
    /// </summary>
    /// <param name="option"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool IsNone<T>(Option<T> option) => option is Option<T>.None;

    /// <summary>
    /// </summary>
    /// <param name="result"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TE"></typeparam>
    /// <returns></returns>
    public static bool IsOk<T, TE>(Result<T, TE>    result) => result is Result<T, TE>.Ok;

    /// <summary>
    /// </summary>
    /// <param name="result"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TE"></typeparam>
    /// <returns></returns>
    public static bool IsError<T, TE>(Result<T, TE> result) => result is Result<T, TE>.Error;

    /// <summary>
    /// </summary>
    /// <param name="result"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool IsSuccess<T>(Try<T> result) => result is Try<T>.Success;

    /// <summary>
    /// </summary>
    /// <param name="result"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool IsFailure<T>(Try<T> result) => result is Try<T>.Failure;
}
