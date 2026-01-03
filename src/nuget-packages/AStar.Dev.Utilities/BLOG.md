### AStar.Dev.Utilities — The junior dev’s field guide to handy .NET helpers (with lots of examples) 🧰✨

Welcome! This post is your practical, friendly tour of `AStar.Dev.Utilities`, a small set of extension methods designed to make everyday .NET tasks faster, clearer, and safer. If you’ve ever thought
“there must be a simpler way to do this,” you’re in the right place.

We’ll cover everything the package offers, with copy‑pasteable snippets and realistic scenarios. You’ll learn not only how to call each method, but also when to use it, when not to, and what pitfalls
to avoid. We’ll also include performance notes, testing tips, and a few jokes so your linter doesn’t think you’re a robot. 😄

This guide is Medium‑friendly and web‑friendly Markdown. C# examples target .NET 8/9/10 idioms. Time to sharpen your tools. ☕

— Updated on 2025‑12‑02 20:03 local time

---

## Table of contents

- Getting started and namespaces
- JSON helpers
    - `Constants.WebDeserialisationSettings`
    - `ObjectExtensions.ToJson`
    - `StringExtensions.FromJson<T>` overloads
- String helpers
    - `IsNull`, `IsNotNull`, `IsNullOrWhiteSpace`, `IsNotNullOrWhiteSpace`
    - `IsImage`
    - `IsNumberOnly`
    - `TruncateIfRequired`
    - `RemoveTrailing`
    - `SanitizeFilePath`
- Regex helpers
    - `ContainsAtLeastOneLowercaseLetter`
    - `ContainsAtLeastOneUppercaseLetter`
    - `ContainsAtLeastOneDigit`
    - `ContainsAtLeastOneSpecialCharacter`
- LINQ helper: `ForEach`
- Enums: `ParseEnum<T>`
- Encryption: `Encrypt`, `Decrypt` (AES)
- Recipes: combine helpers to solve real problems
- Testing tips and pitfalls
- FAQ and checklists

---

## Getting started

Add the namespace once:

```csharp
using AStar.Dev.Utilities;
```

All members here are extension methods or simple constants, so you can call them directly on your existing objects. No setup or DI required.

---

## JSON helpers — be consistent and readable 📦➡️📜

### `Constants.WebDeserialisationSettings`

This property returns a `JsonSerializerOptions` configured with `JsonSerializerDefaults.Web`. That means camelCase property names and sensible web‑style defaults.

When to use:

- You want a shared, consistent set of JSON options across your app.
- You need to deserialize external JSON that follows common web conventions.

Example:

```csharp
var options = Constants.WebDeserialisationSettings; // camelCase, web defaults
var person = JsonSerializer.Deserialize<Person>(
    "{\"firstName\":\"Ada\",\"lastName\":\"Lovelace\"}",
    options
);
```

Tip: these options pair nicely with the `FromJson<T>` extension below for a one‑liner.

### `ObjectExtensions.ToJson<T>(this T obj)`

Serialize using `JsonSerializerDefaults.Web` and `WriteIndented = true` so your output is human‑friendly (great for logs and debugging).

```csharp
var model = new { FirstName = "Ada", LastName = "Lovelace" };
string json = model.ToJson();
// {
//   "firstName": "Ada",
//   "lastName": "Lovelace"
// }
```

When to use:

- Logging DTOs during development.
- Generating readable samples for documentation or diagnostics.

Performance note: Pretty printing (`WriteIndented`) is slower and larger than compact JSON. For high‑volume telemetry, use your own `JsonSerializerOptions` with `WriteIndented = false`.

### `StringExtensions.FromJson<T>(this string json)`

Deserialize a JSON string to `T` using `Constants.WebDeserialisationSettings`.

```csharp
var user = "{\"firstName\":\"Ada\"}".FromJson<User>();
```

### `StringExtensions.FromJson<T>(this string json, JsonSerializerOptions options)`

Same as above, but you can pass custom options (e.g., case‑insensitive matching, number handling, converters, etc.).

```csharp
var opts = new JsonSerializerOptions(JsonSerializerDefaults.Web)
{
    PropertyNameCaseInsensitive = true
};
var user2 = "{\"FirstName\":\"ADA\"}".FromJson<User>(opts);
```

Pitfall: If JSON is missing required fields, you may get default values or exceptions depending on `T` and your options. Unit test both happy and sad paths.

---

## String helpers — the everyday workhorses 🧵💪

Namespace: `AStar.Dev.Utilities`

### `IsNull` and `IsNotNull`

```csharp
string? maybeNull = null;
bool isNull = maybeNull.IsNull();       // true
bool isNotNull = maybeNull.IsNotNull(); // false
```

Why they exist: expressive, chainable checks that read like English.

### `IsNullOrWhiteSpace` and `IsNotNullOrWhiteSpace`

```csharp
var name = " ";
bool blank    = name.IsNullOrWhiteSpace();       // true
bool notBlank = name.IsNotNullOrWhiteSpace();    // false
```

Use these to guard inputs and show helpful validation messages.

### `IsImage()`

Returns true if a string ends with a common image extension: `.jpg`, `.jpeg`, `.png`, `.bmp`, `.gif` (case‑insensitive).

```csharp
"avatar.JPG".IsImage();  // true
"report.pdf".IsImage();  // false
```

When to use:

- Quick checks when routing files to different processors.
  When not to use:
- When you need MIME detection. Extensions can be faked; use content sniffing for security‑sensitive scenarios.

### `IsNumberOnly()`

Returns true if the string contains only digits, underscore `_`, or dot `.`. This is intentionally permissive to allow version‑like tokens (`1.2_3`).

```csharp
"123".IsNumberOnly();    // true
"1.2_3".IsNumberOnly();  // true
"12a".IsNumberOnly();    // false
```

Use cases:

- Lightweight validation of numeric-ish identifiers.
- Filtering tokens before heavier parsing.

### `TruncateIfRequired(int truncateLength)`

If the string is longer than `truncateLength`, return the prefix; otherwise return the original string unchanged.

```csharp
"Hello, World".TruncateIfRequired(5); // "Hello"
"Hi".TruncateIfRequired(5);           // "Hi"
```

Pattern: log‑safe truncation. Use this to keep logs readable without losing the beginning of values (IDs, paths). If the tail matters, pair with an ellipsis in your UI:
`text.TruncateIfRequired(20) + (text.Length > 20 ? "…" : "")`.

### `RemoveTrailing(string token)`

If the string ends with `token` (case‑insensitive), removes exactly one occurrence. If not, returns the string unchanged.

```csharp
"hello/".RemoveTrailing("/");      // "hello"
"hello//".RemoveTrailing("/");     // "hello/" (removes one)
"file.TXT".RemoveTrailing(".txt"); // "file"
"name".RemoveTrailing("");         // "name" (no‑op)
```

This is especially handy when normalizing paths or URLs before concatenation.

### `SanitizeFilePath()`

Replaces path separators, hyphens, and underscores with spaces to produce a human‑readable label.

```csharp
"folder/sub-folder/file_name.txt".SanitizeFilePath();
// "folder sub folder file name.txt"
```

When to use:

- Generating user‑facing labels from file paths.
  When not to use:
- Persisting paths! This method is for display only; do not feed the sanitized value back into file APIs.

---

## Regex helpers — simple quality signals 🔎

These helpers are implemented with source‑generated regular expressions (fast and AOT‑friendly). Each method checks whether the input contains at least one character from a category.

- `ContainsAtLeastOneLowercaseLetter`
- `ContainsAtLeastOneUppercaseLetter`
- `ContainsAtLeastOneDigit`
- `ContainsAtLeastOneSpecialCharacter`

Example:

```csharp
var pwd = "Secr3t!";
bool good = pwd.ContainsAtLeastOneLowercaseLetter()
         && pwd.ContainsAtLeastOneUppercaseLetter()
         && pwd.ContainsAtLeastOneDigit()
         && pwd.ContainsAtLeastOneSpecialCharacter();
```

Use cases:

- Password strength hints.
- Lightweight validation messages (“add a number”, “add a special character”).

Note: These are not comprehensive password policies. For enterprise rules, layer additional checks (length, banned lists, entropy estimates).

---

## LINQ helper — `ForEach` 🔁

`ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)` applies an action to every element.

```csharp
new[] { "Ada", "Alan", "Grace" }.ForEach(Console.WriteLine);
```

When to use:

- When performing side‑effects (logging, writing to console, building a StringBuilder) on each item.

When not to use:

- Inside fluent transformation chains where pure LINQ (`Select`, `Where`) is clearer. Keep side‑effects explicit.

---

## Enum parser — `ParseEnum<T>` 🧭

Case‑insensitive parsing that throws a helpful exception on invalid values.

```csharp
public enum Flavor { Vanilla, Chocolate, Strawberry }

var f1 = "vanilla".ParseEnum<Flavor>();   // Flavor.Vanilla
var f2 = "CHOCOLATE".ParseEnum<Flavor>(); // Flavor.Chocolate
// "pineapple".ParseEnum<Flavor>();       // throws ArgumentException 🍍🙅
```

Tip: Use `Enum.TryParse` if you prefer non‑throwing behavior. Choose based on the calling context.

---

## Encryption — AES made approachable 🔐

Two helpers simplify symmetric encryption using AES:

- `Encrypt(this string plainText, string? key = null, string? iv = null)`
- `Decrypt(this string cipherText, string? key = null, string? iv = null)`

Basics:

- AES requires a key length of 16, 24, or 32 bytes; IV (initialization vector) must be 16 bytes.
- The helpers accept optional key/IV. If omitted, built‑in defaults are used (good for samples and tests, not for production).

Example:

```csharp
const string key = "1234567890ABCDEF1234567890ABCDEF"; // 32 bytes
const string iv  = "ABCDEF1234567890";                  // 16 bytes

var secret = "Launch at dawn";
string cipher    = secret.Encrypt(key, iv);
string roundTrip = cipher.Decrypt(key, iv);

Console.WriteLine(roundTrip); // "Launch at dawn"
```

Security notes (important):

- Never commit real keys or IVs to source control. Use secure stores (Azure Key Vault, AWS KMS, 1Password Secrets Automation, etc.).
- A fixed IV reduces security for repeated messages. Prefer generating a random IV per message and storing/transmitting it alongside the ciphertext (e.g., prefix the IV to the payload).
- These helpers are a convenience layer. For serious cryptography, consult your security team and threat model.

---

## Recipes — combine helpers to solve real problems 🍳

### 1) Create a friendly document label from a path

```csharp
string path  = "/reports/2025-12/HELLO_WORLD.txt";
string label = path.SanitizeFilePath().TruncateIfRequired(32);
// => "  reports  2025 12 HELLO WORLD.txt" (leading spaces depend on your separators)
```

Improve: Trim spaces and normalize casing for a neat UI label.

```csharp
label = string.Join(' ', label.Split(' ', StringSplitOptions.RemoveEmptyEntries));
label = label[..1].ToUpperInvariant() + label[1..];
```

### 2) Guard user input

```csharp
string input = console.ReadLine() ?? string.Empty;
if (input.IsNullOrWhiteSpace())
{
    Console.WriteLine("Please enter a value.");
}
else if (!input.IsNumberOnly())
{
    Console.WriteLine("Use digits, '.' or '_' only.");
}
else
{
    Console.WriteLine($"Thanks! You typed: {input}");
}
```

### 3) Normalize URLs before concatenation

```csharp
string baseUrl = "https://api.example.com/";
string route   = "/v1/items";

string normalized = baseUrl.RemoveTrailing("/") + "/" + route.RemoveTrailing("");
// => "https://api.example.com/v1/items"
```

### 4) Password hint banner

```csharp
string pwd = GetPasswordFromUser();
var hints = new List<string>();
if (!pwd.ContainsAtLeastOneLowercaseLetter()) hints.Add("lowercase");
if (!pwd.ContainsAtLeastOneUppercaseLetter()) hints.Add("uppercase");
if (!pwd.ContainsAtLeastOneDigit())           hints.Add("digit");
if (!pwd.ContainsAtLeastOneSpecialCharacter()) hints.Add("special character");

Console.WriteLine(hints.Count == 0
    ? "Looks good!"
    : $"Try adding: {string.Join(", ", hints)}");
```

### 5) Securely store a token (demo‑level)

```csharp
string token = "abc-123-xyz";
string cipher = token.Encrypt(key: Environment.GetEnvironmentVariable("APP_AES_KEY"),
                              iv:  Environment.GetEnvironmentVariable("APP_AES_IV"));

// Later
string plain = cipher.Decrypt(key: Environment.GetEnvironmentVariable("APP_AES_KEY"),
                              iv:  Environment.GetEnvironmentVariable("APP_AES_IV"));
```

Reminder: Rotate keys, protect env vars, and consider per‑message IVs.

---

## Testing tips 🧪

These helpers are all deterministic and easy to test.

- `ToJson` and `FromJson` round‑trip: serialize a small object, deserialize, compare.
- `IsImage` and `IsNumberOnly`: use theory tests with various inputs.
- `TruncateIfRequired`: test both branches (short string vs long string).
- `RemoveTrailing`: verify it removes only one trailing token.
- `SanitizeFilePath`: assert expected spaces for different separators.
- Regex helpers: quick samples for each category.
- `ParseEnum<T>`: test valid/invalid values.
- Encryption: encrypt/decrypt round‑trip with a stable key/IV.

Example (xUnit + Shouldly):

```csharp
[Theory]
[InlineData("hello/", "/", "hello")]
[InlineData("hello//", "/", "hello/")]
[InlineData("file.TXT", ".txt", "file")]
public void RemoveTrailing_Works(string input, string token, string expected)
    => input.RemoveTrailing(token).ShouldBe(expected);
```

---

## Common pitfalls and how to avoid them ⚠️

- Using `SanitizeFilePath` for persistence. It’s for display only; you’ll lose path fidelity.
- Treating `IsImage` as a security gate. Check MIME types for uploads instead.
- Logging huge strings without truncation. Use `TruncateIfRequired` for sanity.
- Storing encryption keys in code. Use secure stores/env vars; rotate keys.
- Relying on `ParseEnum<T>` for user input without try/catch. Consider `Enum.TryParse` when appropriate.

Code review checklist:

- [ ] Are JSON options consistent (web defaults where appropriate)?
- [ ] Are string guards (`IsNullOrWhiteSpace`) in place for user input?
- [ ] Are labels generated with `SanitizeFilePath` only for display, not storage?
- [ ] Are long values truncated before logging?
- [ ] Is encryption used with externalized key/IV?

---

## FAQ 🙋

Q: Is this a framework?  
A: Nope — just pragmatic, well‑named helpers.

Q: Safe for tests?  
A: Yes! Everything is deterministic. We ship tests proving behavior.

Q: Can I use just one or two?  
A: Absolutely. Import the namespace and call what you like.

Q: Any performance costs?  
A: Negligible for typical app code. Regex helpers are source‑generated; JSON pretty‑printing is the main extra cost (use compact JSON when needed).

---

### Final thoughts

Small, sharp tools beat giant toolboxes when you’re under deadline. Use these helpers to keep code readable and intentions obvious. Your code reviewers will thank you. Your future self will, too. 🙌
