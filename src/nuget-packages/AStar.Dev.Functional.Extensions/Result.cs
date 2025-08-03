using System;

namespace AStar.Dev.Functional.Extensions;

/// <summary>
///     Represents a discriminated union of success or failure.
/// </summary>
/// <typeparam name="TSuccess">The type of the success value.</typeparam>
/// <typeparam name="TError">The type of the error reason.</typeparam>
public abstract class Result<TSuccess, TError>
{
    private Result()
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="onSuccess"></param>
    /// <param name="onError"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public TResult Match<TResult>(
        Func<TSuccess, TResult> onSuccess,
        Func<TError, TResult>   onError) =>
        this switch
        {
            Ok ok     => onSuccess(ok.Value),
            Error err => onError(err.Reason),
            _         => throw new InvalidOperationException("Unrecognized result type")
        };

    /// <summary>
    ///     Represents a successful outcome.
    /// </summary>
    public sealed class Ok : Result<TSuccess, TError>
    {
        /// <summary>
        ///     Creates a successful result.
        /// </summary>
        /// <param name="value">The result value.</param>
        public Ok(TSuccess value) => Value = value;

        /// <summary>
        ///     The successful value.
        /// </summary>
        public TSuccess Value { get; }
    }

    /// <summary>
    ///     Represents an error outcome.
    /// </summary>
    public sealed class Error : Result<TSuccess, TError>
    {
        /// <summary>
        ///     Creates an error result.
        /// </summary>
        /// <param name="reason">The failure reason.</param>
        public Error(TError reason) => Reason = reason;

        /// <summary>
        ///     The error reason.
        /// </summary>
        public TError Reason { get; }
    }
}
