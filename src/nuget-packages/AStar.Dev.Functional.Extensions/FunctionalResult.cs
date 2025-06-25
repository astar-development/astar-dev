namespace AStar.Dev.Functional.Extensions;

/// <summary>
/// </summary>
public static class FunctionalResult
{
    /// <summary>
    /// </summary>
    /// <param name="result"></param>
    /// <param name="map"></param>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static Result<T2, TError> Map<T1, T2, TError>(this Result<T1, TError> result, Func<T1, T2> map) =>
        result.IsSuccess
            ? Result<T2, TError>.Success(map(result.Value))
            : Result<T2, TError>.Failure(result.Error);

    /// <summary>
    /// </summary>
    /// <param name="result"></param>
    /// <param name="bind"></param>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static Result<T2, TError> Bind<T1, T2, TError>(this Result<T1, TError> result, Func<T1, Result<T2, TError>> bind) =>
        result.IsSuccess
            ? bind(result.Value)
            : Result<T2, TError>.Failure(result.Error);

    /// <summary>
    /// </summary>
    /// <param name="result"></param>
    /// <param name="map"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <typeparam name="TNewError"></typeparam>
    /// <returns></returns>
    public static Result<T, TNewError> MapError<T, TError, TNewError>(this Result<T, TError> result, Func<TError, TNewError> map) =>
        result.IsSuccess
            ? Result<T, TNewError>.Success(result.Value)
            : Result<T, TNewError>.Failure(map(result.Error));

    /// <summary>
    /// </summary>
    /// <param name="result"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onFailure"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static TResult Match<T, TError, TResult>(
        this Result<T, TError> result,
        Func<T, TResult>       onSuccess,
        Func<TError, TResult>  onFailure) =>
        result.IsSuccess
            ? onSuccess(result.Value)
            : onFailure(result.Error);
}
