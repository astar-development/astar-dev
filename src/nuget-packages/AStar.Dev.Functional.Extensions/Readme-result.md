# 🧭 Result<T, TError> Cheat Sheet

## 🧱 Core Concept

Result<T, TError> encapsulates either:

✅ Ok(T) — a success value

❌ Error(TError) — an error reason

```csharp
Result<string, string> nameResult = new Result<string, string>.Ok("Jason");
Result<int, string> errorResult = new Result<int, string>.Error("Invalid input");
```

## 🏗 Construction Helpers

| Expression                     | Outcome                     |
|--------------------------------|-----------------------------|
| new Result<T, E>.Ok(value)     | Constructs a success result |
| new Result<T, E>.Error(reason) | Constructs an error result  |

## 🔧 Transformation

| Method      | Description                                      |
|-------------|--------------------------------------------------|
| Map(fn)     | Transforms the success value                     |
| Bind(fn)    | Chains to another result-returning function      |
| Tap(action) | Invokes side effect on success, returns original |

```csharp
result.Map(value => value.ToUpper());
result.Bind(value => Validate(value));
result.Tap(Console.WriteLine);
```

## 🧪 Pattern Matching

```csharp
result.Match(
onSuccess: value => $"✅ {value}",
onError: reason => $"❌ {reason}"
);
```

## 🧞 LINQ Composition

```csharp
var final =
from input in GetInput()
from valid in Validate(input)
select $"Welcome, {valid}";
```

## LINQ Methods

| Method               | Description                                 |
|----------------------|---------------------------------------------|
| Select(fn)           | Maps over success value                     |
| SelectMany(fn)       | Binds to next result                        |
| SelectMany(..., ...) | Binds and projects from intermediate result |

## ⚡ Async Support

```csharp
var asyncResult = await resultTask.MapAsync(val => val.Length);
var finalValue = await resultTask.MatchAsync(...);
```

## Async LINQ

```csharp
var result =
await GetAsync()
.SelectMany(asyncValue => ValidateAsync(asyncValue), (a, b) => $"{a}-{b}");
```

## 🧯 Error Handling

```csharp
if (result is Result<T, TError>.Error err)
Log(err.Reason);
```

Or selectively tap into errors:

```csharp
public static Result<T, TError> TapError<T, TError>(
this Result<T, TError> result,
Action<TError> handler)
{
if (result is Result<T, TError>.Error error)
handler(error.Reason);
return result;
}
```
