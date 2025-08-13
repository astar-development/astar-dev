# 🧱 Core Concepts

## 🧭 What is Result<TSuccess, TError>?

is a discriminated union type that represents either successful completion with a value or failure with an error. It is a powerful alternative to exception-based error handling and enables functional
programming patterns in C#. `Result<TSuccess, TError>`

## 🏗️ Structure

encapsulates either: `Result<TSuccess, TError>`

- ✅ `Ok(TSuccess)` — a success value
- ❌ `Error(TError)` — an error reason

``` csharp
// Create a success result
Result<string, string> successResult = new Result<string, string>.Ok("Harry Potter");

// Create an error result
Result<decimal, string> errorResult = new Result<decimal, string>.Error("Book out of stock");
```

## 🧰 Construction

| Expression                                   | Description                 |
|----------------------------------------------|-----------------------------|
| `new Result<TSuccess, TError>.Ok(value)`     | Constructs a success result |
| `new Result<TSuccess, TError>.Error(reason)` | Constructs an error result  |

## 🔍 Checking Result Type

You can use pattern matching to check the type of a result:

``` csharp
if (result is Result<Book, string>.Ok ok)
{
    // Handle success case
    Book book = ok.Value;
}
else if (result is Result<Book, string>.Error err)
{
    // Handle error case
    string reason = err.Reason;
}
```

## 🔄 Pattern Matching

The most common way to handle both success and error cases is with the method: `Match`

``` csharp
string message = result.Match(
    onSuccess: book => $"Found: {book.Title} by {book.Author}",
    onFailure: error => $"Error: {error}"
);
```

## 🌟 Benefits Over Exceptions

1. **Explicit Error Types**: The error type is part of the method signature
2. **No Unexpected Exceptions**: All possible failure modes are explicitly modeled
3. **Composition**: Results can be easily composed and chained
4. **Performance**: No stack unwinding cost of exceptions
5. **Clarity**: Makes error handling paths explicit and visible

## 🤔 When to Use Result

Use when: `Result<TSuccess, TError>`

- A function can fail in an expected way
- You want to make failure cases explicit in your API
- You need to return detailed error information
- You want to compose operations that might fail
- You want to avoid try/catch blocks

## 📝 Common Error Types

While you can use any type for , these are common patterns: `TError`

- **Simple string**: For basic error messages
- **Enum**: For categorized error types
- **Custom error class**: For detailed error information
- **Union type**: For multiple error categories

Example of a custom error class:

``` csharp
public class OrderError
{
    public string Code { get; }
    public string Message { get; }
    public object Details { get; }

    public OrderError(string code, string message, object details = null)
    {
        Code = code;
        Message = message;
        Details = details;
    }
}
```
