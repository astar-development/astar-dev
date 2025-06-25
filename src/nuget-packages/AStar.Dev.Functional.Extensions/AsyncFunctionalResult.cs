namespace AStar.Dev.Functional.Extensions;

/// <summary>
/// </summary>
public static class AsyncFunctionalResult
{
    /// <summary>
    /// </summary>
    /// <param name="result"></param>
    /// <param name="map"></param>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<T2, TError>> MapAsync<T1, T2, TError>(
        this Task<Result<T1, TError>> result,
        Func<T1, T2>                  map) =>
        (await result).Map(map);

    /// <summary>
    /// </summary>
    /// <param name="result"></param>
    /// <param name="mapAsync"></param>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<T2, TError>> MapAsync<T1, T2, TError>(
        this Result<T1, TError> result,
        Func<T1, Task<T2>>      mapAsync) =>
        result.IsSuccess
            ? Result<T2, TError>.Success(await mapAsync(result.Value))
            : Result<T2, TError>.Failure(result.Error);

    /// <summary>
    /// </summary>
    /// <param name="result"></param>
    /// <param name="mapAsync"></param>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<T2, TError>> MapAsync<T1, T2, TError>(
        this Task<Result<T1, TError>> result,
        Func<T1, Task<T2>>            mapAsync) =>
        await (await result).MapAsync(mapAsync);

    /// <summary>
    /// </summary>
    /// <param name="result"></param>
    /// <param name="bind"></param>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<T2, TError>> BindAsync<T1, T2, TError>(
        this Task<Result<T1, TError>> result,
        Func<T1, Result<T2, TError>>  bind) =>
        (await result).Bind(bind);

    /// <summary>
    /// </summary>
    /// <param name="result"></param>
    /// <param name="bindAsync"></param>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<T2, TError>> BindAsync<T1, T2, TError>(
        this Result<T1, TError>            result,
        Func<T1, Task<Result<T2, TError>>> bindAsync) =>
        result.IsSuccess
            ? await bindAsync(result.Value)
            : Result<T2, TError>.Failure(result.Error);

    /// <summary>
    /// </summary>
    /// <param name="result"></param>
    /// <param name="bindAsync"></param>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<T2, TError>> BindAsync<T1, T2, TError>(
        this Task<Result<T1, TError>>      result,
        Func<T1, Task<Result<T2, TError>>> bindAsync) =>
        await (await result).BindAsync(bindAsync);

    /// <summary>
    /// </summary>
    /// <param name="result"></param>
    /// <param name="map"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <typeparam name="TNewError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<T, TNewError>> MapErrorAsync<T, TError, TNewError>(
        this Task<Result<T, TError>> result,
        Func<TError, TNewError>      map) =>
        (await result).MapError(map);

    /// <summary>
    /// </summary>
    /// <param name="result"></param>
    /// <param name="mapAsync"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <typeparam name="TNewError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<T, TNewError>> MapErrorAsync<T, TError, TNewError>(
        this Result<T, TError>        result,
        Func<TError, Task<TNewError>> mapAsync) =>
        result.IsSuccess
            ? Result<T, TNewError>.Success(result.Value)
            : Result<T, TNewError>.Failure(await mapAsync(result.Error));

    /// <summary>
    /// </summary>
    /// <param name="result"></param>
    /// <param name="mapAsync"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <typeparam name="TNewError"></typeparam>
    /// <returns></returns>
    public static async Task<Result<T, TNewError>> MapErrorAsync<T, TError, TNewError>(
        this Task<Result<T, TError>>  result,
        Func<TError, Task<TNewError>> mapAsync) =>
        await (await result).MapErrorAsync(mapAsync);

    /// <summary>
    /// </summary>
    /// <param name="result"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onFailure"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static async Task<TResult> MatchAsync<T, TError, TResult>(
        this Task<Result<T, TError>> result,
        Func<T, TResult>             onSuccess,
        Func<TError, TResult>        onFailure) =>
        (await result).Match(onSuccess, onFailure);

    /// <summary>
    /// </summary>
    /// <param name="result"></param>
    /// <param name="onSuccessAsync"></param>
    /// <param name="onFailure"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static async Task<TResult> MatchAsync<T, TError, TResult>(
        this Result<T, TError> result,
        Func<T, Task<TResult>> onSuccessAsync,
        Func<TError, TResult>  onFailure) =>
        result.IsSuccess
            ? await onSuccessAsync(result.Value)
            : onFailure(result.Error);

    /// <summary>
    /// </summary>
    /// <param name="result"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onFailureAsync"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static async Task<TResult> MatchAsync<T, TError, TResult>(
        this Result<T, TError>      result,
        Func<T, TResult>            onSuccess,
        Func<TError, Task<TResult>> onFailureAsync) =>
        result.IsSuccess
            ? onSuccess(result.Value)
            : await onFailureAsync(result.Error);

    /// <summary>
    /// </summary>
    /// <param name="result"></param>
    /// <param name="onSuccessAsync"></param>
    /// <param name="onFailure"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static async Task<TResult> MatchAsync<T, TError, TResult>(
        this Task<Result<T, TError>> result,
        Func<T, Task<TResult>>       onSuccessAsync,
        Func<TError, TResult>        onFailure) =>
        await (await result).MatchAsync(onSuccessAsync, onFailure);

    /// <summary>
    /// </summary>
    /// <param name="result"></param>
    /// <param name="onSuccess"></param>
    /// <param name="onFailureAsync"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static async Task<TResult> MatchAsync<T, TError, TResult>(
        this Task<Result<T, TError>> result,
        Func<T, TResult>             onSuccess,
        Func<TError, Task<TResult>>  onFailureAsync) =>
        await (await result).MatchAsync(onSuccess, onFailureAsync);

    /// <summary>
    /// </summary>
    /// <param name="result"></param>
    /// <param name="onSuccessAsync"></param>
    /// <param name="onFailureAsync"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static async Task<TResult> MatchAsync<T, TError, TResult>(
        this Result<T, TError>      result,
        Func<T, Task<TResult>>      onSuccessAsync,
        Func<TError, Task<TResult>> onFailureAsync) =>
        result.IsSuccess
            ? await onSuccessAsync(result.Value)
            : await onFailureAsync(result.Error);

    /// <summary>
    /// </summary>
    /// <param name="result"></param>
    /// <param name="onSuccessAsync"></param>
    /// <param name="onFailureAsync"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TError"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public static async Task<TResult> MatchAsync<T, TError, TResult>(
        this Task<Result<T, TError>> result,
        Func<T, Task<TResult>>       onSuccessAsync,
        Func<TError, Task<TResult>>  onFailureAsync) =>
        await (await result).MatchAsync(onSuccessAsync, onFailureAsync);
}
