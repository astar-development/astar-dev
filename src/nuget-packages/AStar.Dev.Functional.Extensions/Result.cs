namespace AStar.Dev.Functional.Extensions;
//
// /// <summary>
// ///     The <see cref="Result{TSuccess,TFailure}" /> class
// /// </summary>
// public class Result<TSuccess, TFailure>
// {
//     /// <summary>
//     ///     The failure object when the result is a failure
//     /// </summary>
//     public readonly  TFailure    Error;
//
//     /// <summary>
//     ///     The actual object when the result is a success
//     /// </summary>
//     public readonly  TSuccess    Value;
//
//     private Result(TSuccess v, TFailure e, bool isSuccess)
//     {
//         Value     = v;
//         Error     = e;
//         IsSuccess = isSuccess;
//     }
//
//     /// <summary>
//     ///     A flag denoting whether the Result is a success or not
//     /// </summary>
//     public bool IsSuccess { get ; }
//
//     /// <summary>
//     ///     A flag denoting whether the Result is a failure or not
//     /// </summary>
//     public bool IsFailure => !IsSuccess;
//
//     /// <summary>
//     ///     An extension method to create a Success object
//     /// </summary>
//     /// <param name="success">The actual success object</param>
//     /// <returns>An instance of <see cref="Result{TSuccess,TFailure}" /> configured as a success</returns>
//     public static Result<TSuccess, TFailure> Success(TSuccess success) =>
//         new(success, default!, true);
//
//     /// <summary>
//     ///     An extension method to create a Failure object
//     /// </summary>
//     /// <returns>An instance of <see cref="Result{TSuccess,TFailure}" /> configured as a failure</returns>
//     /// <returns></returns>
//     public static Result<TSuccess, TFailure> Fail(TFailure e) =>
//         new(default!, e, false);
//
//     /// <summary>
//     ///     The <see cref="Match{TResult}" /> method that will perform either the success or failure function depending on the current state
//     /// </summary>
//     /// <param name="success">The function to run when the result is a success</param>
//     /// <param name="failure">The function to run when the result is a failure</param>
//     /// <typeparam name="TResult">The type of the return object</typeparam>
//     /// <returns>The result of running either the success or failure function</returns>
//     public TResult Match<TResult>(Func<TSuccess, TResult> success, Func<TFailure, TResult> failure) =>
//         IsSuccess ? success(Value) : failure(Error);
//
//     /// <summary>
//     ///     The implicit operator to create a new success Result
//     /// </summary>
//     /// <param name="success">The success object to set as the success value</param>
//     /// <returns>An instance of <see cref="Result{TSuccess,TFailure}" /> configured as a success</returns>
//     public static implicit operator Result<TSuccess, TFailure>(TSuccess success) =>
//         new(success, default!, true);
//
//     /// <summary>
//     ///     The implicit operator to create a new failure Result
//     /// </summary>
//     /// <param name="error">The error object to set as the error value</param>
//     /// <returns>An instance of <see cref="Result{TSuccess,TFailure}" /> configured as a failure</returns>
//     public static implicit operator Result<TSuccess, TFailure>(TFailure error) =>
//         new(default!, error, false);
//
// }

/// <summary>
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="TError"></typeparam>
public class Result<T, TError>
{
    private TError? error;
    private T?      value;

    private Result(bool isSuccess, T? value, TError? error) =>
        (IsSuccess, this.value, this.error) = (isSuccess, value, error);

    /// <summary>
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public T Value
    {
        get => IsSuccess ? value! : throw new InvalidOperationException("Result is not successful");
        private set => this.value = value;
    }

    /// <summary>
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public TError Error
    {
        get => !IsSuccess ? error! : throw new InvalidOperationException("Result is successful");
        private set => error = value;
    }

    /// <summary>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Result<T, TError> Success(T value) =>
        new  (true, value, default);

    /// <summary>
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    public static Result<T, TError> Failure(TError error) =>
        new (false, default, error);

    /// <summary>
    ///     The implicit operator to create a new success Result
    /// </summary>
    /// <param name="success">The success object to set as the success value</param>
    /// <returns>An instance of <see cref="Result{TSuccess,TFailure}" /> configured as a success</returns>
    public static implicit operator Result<T, TError>(T success) =>
        new(false, success, default!);

    /// <summary>
    ///     The implicit operator to create a new failure Result
    /// </summary>
    /// <param name="error">The error object to set as the error value</param>
    /// <returns>An instance of <see cref="Result{TSuccess,TFailure}" /> configured as a failure</returns>
    public static implicit operator Result<T, TError>(TError error) =>
        new(false, default!, error);
}
