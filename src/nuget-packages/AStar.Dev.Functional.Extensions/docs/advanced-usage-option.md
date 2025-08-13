## 🚀 Advanced Usage

### ⚡ Async Operations

#### 📚 Checking Book Availability Asynchronously

``` csharp
public async Task<Option<BookAvailability>> CheckBookAvailabilityAsync(Book book)
{
    try
    {
        // Simulating an async API call to check inventory
        var response = await _inventoryApiClient.GetAsync($"/api/books/{book.Isbn}/availability");
        
        if (response.IsSuccessStatusCode)
        {
            var availability = await response.Content.ReadFromJsonAsync<BookAvailability>();
            return Option.Some(availability);
        }
        
        return Option.None<BookAvailability>();
    }
    catch (Exception)
    {
        return Option.None<BookAvailability>();
    }
}
```

### 🔄 Combining Sync and Async Operations

``` csharp
public async Task<Option<BookReservation>> ReserveBookByIsbnAsync(string isbn, int quantity)
{
    // Step 1: Find book (sync operation)
    Option<Book> bookOption = FindBook(isbn);
    
    // Step 2: Chain with async operations
    return await bookOption
        .BindAsync(async book => await CheckBookAvailabilityAsync(book))
        .BindAsync(async availability => 
        {
            if (availability.Quantity < quantity)
                return Option.None<BookReservation>();
                
            return await _reservationService.CreateReservationAsync(
                availability.BookIsbn, quantity);
        })
        .TapAsync(async reservation => 
        {
            // Log success
            await _logger.LogInfoAsync($"Book {isbn} reserved: {reservation.ReservationId}");
        });
}
```

### 🧩 Filtering Options

``` csharp
public Option<Book> FindAvailableBookByGenre(string genre, int minimumCopies)
{
    return _bookRepository.GetBooksByGenre(genre)
        // Convert to options
        .Select(book => Option.Some(book))
        // Filter for books with enough copies
        .Choose(book => _inventoryService.GetStock(book.Isbn) >= minimumCopies)
        // Take the first book that matches
        .FirstOrDefault() ?? Option.None<Book>();
}
```

## 🧪 Testing with Options

Testing functions that return is straightforward because all possible outcomes are explicitly modeled. `Option<T>`

### 📚 Example: Testing a Book Service

``` csharp
public class BookService
{
    private readonly IBookRepository _repository;
    
    public BookService(IBookRepository repository)
    {
        _repository = repository;
    }
    
    public Option<Book> FindBook(string isbn)
    {
        if (string.IsNullOrEmpty(isbn))
            return Option.None<Book>();
            
        Book book = _repository.GetByIsbn(isbn);
        
        if (book == null)
            return Option.None<Book>();
            
        return Option.Some(book);
    }
}
```

### ✅ Testing Some Case

``` csharp
[Fact]
public void FindBook_WithValidIsbn_ReturnsBook()
{
    // Arrange
    var mockBook = new Book { Isbn = "123456789", Title = "Test Book" };
    var mockRepo = new Mock<IBookRepository>();
    mockRepo.Setup(r => r.GetByIsbn("123456789")).Returns(mockBook);
    var service = new BookService(mockRepo.Object);
    
    // Act
    var result = service.FindBook("123456789");
    
    // Assert
    Assert.IsType<Option<Book>.Some>(result);
    
    if (result is Option<Book>.Some success)
    {
        Assert.Equal("Test Book", success.Value.Title);
    }
    else
    {
        Assert.Fail("Result should be Some type");
    }
}
```

### ❌ Testing None Case

``` csharp
[Fact]
public void FindBook_WithEmptyIsbn_ReturnsNone()
{
    // Arrange
    var mockRepo = new Mock<IBookRepository>();
    var service = new BookService(mockRepo.Object);
    
    // Act
    var result = service.FindBook("");
    
    // Assert
    Assert.IsType<Option<Book>.None>(result);
}
```

## 📋 Method Reference

This section provides a comprehensive reference for all methods available in the type. `Option<T>`

### 🔍 Pattern Matching

#### Match

``` csharp
public TResult Match<TResult>(
    Func<T, TResult> onSome,
    Func<TResult> onNone)
```

Executes one of the provided functions based on whether the option contains a value or not.
**Example:**

``` csharp
string message = option.Match(
    book => $"Found: {book.Title}",
    () => "Book not found"
);
```

### 🔄 Transformations

#### Map

``` csharp
public Option<TResult> Map<TResult>(
    Func<T, TResult> map)
```

Transforms the value inside the option using the provided function, if present.
**Example:**

``` csharp
Option<string> titleOption = bookOption.Map(book => book.Title);
```

#### Bind

``` csharp
public Option<TResult> Bind<TResult>(
    Func<T, Option<TResult>> bind)
```

Chains the option to another option-producing function.
**Example:**

``` csharp
Option<Reservation> reservationOption = 
    bookOption.Bind(book => TryReserve(book));
```

### 📊 Side Effects

#### Tap

``` csharp
public Option<T> Tap(
    Action<T> action)
```

Performs a side effect on the value, if present, and returns the original option.
**Example:**

``` csharp
bookOption.Tap(book => Console.WriteLine($"Found book: {book.Title}"));
```

### 🧰 Utility Methods

#### TryGetValue

``` csharp
public bool TryGetValue(out T value)
```

Attempts to get the value from the option, returning a boolean indicating success.
**Example:**

``` csharp
if (bookOption.TryGetValue(out var book))
{
    Console.WriteLine($"Book found: {book.Title}");
}
```

#### ToOption

``` csharp
public static Option<T> ToOption<T>(this T value)
```

Converts a value to an option (null becomes None).
**Example:**

``` csharp
Option<Book> bookOption = book.ToOption();
```

#### ToOption with Predicate

``` csharp
public static Option<T> ToOption<T>(
    this T value,
    Func<T, bool> predicate)
```

Converts a value to an option based on a predicate.
**Example:**

``` csharp
Option<Book> inStockBook = book.ToOption(b => b.InStock);
```

#### IsSome / IsNone

``` csharp
public bool IsSome<T>(this Option<T> option)
public bool IsNone<T>(this Option<T> option)
```

Checks if the option is Some or None.
**Example:**

``` csharp
if (bookOption.IsSome())
{
    // Process the book
}
```

#### OrElse

``` csharp
public T OrElse<T>(this Option<T> option, T fallback)
```

Returns the value if present, otherwise returns the fallback.
**Example:**

``` csharp
Book book = bookOption.OrElse(new Book { Title = "Default Book" });
```

#### OrThrow

``` csharp
public T OrThrow<T>(this Option<T> option, Exception ex = null)
```

Returns the value if present, otherwise throws an exception.
**Example:**

``` csharp
Book book = bookOption.OrThrow(new BookNotFoundException(isbn));
```

#### Choose

``` csharp
public static IEnumerable<Option<T>> Choose<T>(
    this IEnumerable<T> source,
    Func<T, bool> predicate)
```

Filters a collection based on a predicate and returns Some options for matching items.
**Example:**

``` csharp
var evenNumbers = numbers.Choose(x => x % 2 == 0);
```

#### Choose with Transformation

``` csharp
public static IEnumerable<TResult> Choose<T, TResult>(
    this IEnumerable<T> source,
    Func<T, Option<TResult>> chooser)
```

Maps and filters a collection, keeping only items that produced Some results.
**Example:**

``` csharp
var availableBooks = books.Choose(book => 
    _inventoryService.GetStock(book.Isbn) > 0 
        ? Option.Some(book) 
        : Option.None<Book>());
```

## 📝 Error Handling Patterns

The type works well with the type for comprehensive error handling: `Option<T>``Result<TSuccess, TError>`

``` csharp
public Result<Book, string> FindBookWithError(string isbn)
{
    if (string.IsNullOrEmpty(isbn))
        return new Result<Book, string>.Error("ISBN cannot be empty");
    
    Option<Book> bookOption = _repository.FindBookByIsbn(isbn);
    
    return bookOption.ToResult(() => $"No book found with ISBN: {isbn}");
}
```

## 📚 Conclusion

The type provides a powerful way to handle optional values in C#, making your code more predictable, composable, and free from null reference exceptions. By using throughout your codebase, you can
eliminate an entire class of bugs while also making your code more expressive and easier to reason about. `Option<T>``Option<T>`
