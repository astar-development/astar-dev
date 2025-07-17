using System;

namespace AStar.Dev.Functional.Extensions;

/// <summary>
///     Represents a computation that may succeed or throw an exception.
/// </summary>
public abstract class Try<T>
{
    private Try()
    {
    }

    /// <summary>
    ///     Runs a function and returns a Try containing success or captured exception.
    /// </summary>
    /// <param name="func">The computation to execute.</param>
    /// <returns>A <see cref="Try{T}" /> result.</returns>
    public static Try<T> Run(Func<T> func)
    {
        try
        {
            return new Success(func());
        }
        catch (Exception ex)
        {
            return new Failure(ex);
        }
    }

    /// <summary>
    ///     Pattern matches on success or failure.
    /// </summary>
    /// <typeparam name="TResult">Result type.</typeparam>
    /// <param name="onSuccess">Called when successful.</param>
    /// <param name="onFailure">Called when an exception occurred.</param>
    public TResult Match<TResult>(
        Func<T, TResult>         onSuccess,
        Func<Exception, TResult> onFailure) =>
        this switch
        {
            Success success => onSuccess(success.Value),
            Failure failure => onFailure(failure.Exception),
            _               => throw new InvalidOperationException()
        };

    /// <summary>
    ///     A successful result.
    /// </summary>
    public sealed class Success : Try<T>
    {
        /// <summary>
        ///     Creates a success.
        /// </summary>
        /// <param name="value">The result.</param>
        public Success(T value) => Value = value;

        /// <summary>
        ///     The successful value.
        /// </summary>
        public T Value { get; }
    }

    /// <summary>
    ///     A failed result due to exception.
    /// </summary>
    public sealed class Failure : Try<T>
    {
        /// <summary>
        ///     Creates a failure.
        /// </summary>
        /// <param name="ex">The exception.</param>
        public Failure(Exception ex) => Exception = ex;

        /// <summary>
        ///     The exception thrown.
        /// </summary>
        public Exception Exception { get; }
    }
}
