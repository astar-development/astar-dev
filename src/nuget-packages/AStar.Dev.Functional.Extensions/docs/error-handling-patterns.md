# 📝 Error Handling Patterns

This document outlines common patterns for error handling with the type in a book store application context. `Result<TSuccess, TError>`

## 🚦 Early Validation Return

Validate inputs early and return errors immediately:

``` csharp
public Result<Order, string> CreateOrder(Customer customer, List<BookItem> items)
{
    // Early validation
    if (customer == null)
        return new Result<Order, string>.Error("Customer cannot be null");
        
    if (items == null || items.Count == 0)
        return new Result<Order, string>.Error("Order must contain at least one item");
        
    foreach (var item in items)
    {
        if (item.Quantity <= 0)
            return new Result<Order, string>.Error($"Quantity for book '{item.Book.Title}' must be greater than zero");
    }
    
    // Proceed with order creation
    var order = new Order
    {
        OrderId = Guid.NewGuid(),
        Customer = customer,
        Items = items,
        OrderDate = DateTime.UtcNow,
        Status = "PENDING"
    };
    
    return new Result<Order, string>.Ok(order);
}
```

## 📊 Error Categorization

Use custom error types to categorize errors:

``` csharp
public enum OrderErrorType
{
    ValidationError,
    InventoryError,
    PaymentError,
    SystemError
}

public class OrderError
{
    public OrderErrorType Type { get; }
    public string Message { get; }
    
    public OrderError(OrderErrorType type, string message)
    {
        Type = type;
        Message = message;
    }
}
```

Using the error type:

``` csharp
public Result<Order, OrderError> PlaceOrder(ShoppingCart cart, PaymentDetails payment)
{
    // Validate cart
    if (cart == null || cart.Items.Count == 0)
        return new Result<Order, OrderError>.Error(
            new OrderError(OrderErrorType.ValidationError, "Shopping cart is empty"));
    
    // Check inventory
    foreach (var item in cart.Items)
    {
        var stock = _inventoryService.GetStockLevel(item.Book.Isbn);
        if (stock < item.Quantity)
            return new Result<Order, OrderError>.Error(
                new OrderError(OrderErrorType.InventoryError, 
                    $"Insufficient stock for '{item.Book.Title}'. Available: {stock}"));
    }
    
    // Process payment
    try
    {
        var paymentResult = _paymentService.ProcessPayment(payment, cart.TotalAmount);
        if (!paymentResult.Success)
            return new Result<Order, OrderError>.Error(
                new OrderError(OrderErrorType.PaymentError, 
                    $"Payment failed: {paymentResult.Message}"));
    }
    catch (Exception ex)
    {
        return new Result<Order, OrderError>.Error(
            new OrderError(OrderErrorType.SystemError, 
                $"System error during payment processing: {ex.Message}"));
    }
    
    // Create order
    var order = new Order
    {
        OrderId = Guid.NewGuid(),
        Items = cart.Items,
        TotalAmount = cart.TotalAmount,
        OrderDate = DateTime.UtcNow,
        Status = "CONFIRMED"
    };
    
    return new Result<Order, OrderError>.Ok(order);
}
```

## 🔄 Error Transformation

Transform technical errors to user-friendly messages:

``` csharp
public Result<Order, string> CreateUserFriendlyResult(Result<Order, OrderError> technicalResult)
{
    return technicalResult.MapFailure(error => 
    {
        switch (error.Type)
        {
            case OrderErrorType.ValidationError:
                return error.Message; // Already user-friendly
                
            case OrderErrorType.InventoryError:
                return "One or more items in your cart are not available in the requested quantity.";
                
            case OrderErrorType.PaymentError:
                return "We couldn't process your payment. Please check your payment details and try again.";
                
            case OrderErrorType.SystemError:
                // Log the detailed technical error
                _logger.Error($"System error: {error.Message}");
                return "We're experiencing technical difficulties. Please try again later.";
                
            default:
                return "An unexpected error occurred. Please try again.";
        }
    });
}
```

## 🎯 Handling Different Error Types

Handle different error types specifically:

``` csharp
orderResult.Match(
    order => DisplayOrderConfirmation(order),
    error => 
    {
        switch (error.Type)
        {
            case OrderErrorType.ValidationError:
                DisplayValidationError(error.Message);
                break;
                
            case OrderErrorType.InventoryError:
                DisplayInventoryError(error.Message);
                SuggestAlternativeBooks();
                break;
                
            case OrderErrorType.PaymentError:
                DisplayPaymentError(error.Message);
                ShowAlternativePaymentOptions();
                break;
                
            case OrderErrorType.SystemError:
                DisplayGenericError("System error occurred");
                _logger.Error($"System error: {error.Message}");
                break;
        }
    }
);
```

## 🔎 Detailed Logging with TapError

Log detailed error information without affecting the result flow:

``` csharp
public Result<OrderConfirmation, OrderError> PlaceOrderWithLogging(
    ShoppingCart cart, PaymentDetails payment)
{
    return ValidateCart(cart)
        .TapError(error => _logger.Warn($"Cart validation failed: {error.Message}"))
        .Bind(validCart => CheckInventory(validCart))
        .TapError(error => _logger.Warn($"Inventory check failed: {error.Message}"))
        .Bind(inventory => ProcessPayment(cart.TotalAmount, payment))
        .TapError(error => 
        {
            if (error.Type == OrderErrorType.PaymentError)
                _logger.Warn($"Payment failed: {error.Message}");
            else
                _logger.Error($"Critical error during payment: {error.Message}");
        })
        .Map(paymentConfirmation => CreateOrderConfirmation(cart, paymentConfirmation));
}
```

## 📊 Collecting Multiple Errors

Sometimes you want to collect multiple errors rather than stopping at the first one:

``` csharp
public class ValidationResult<T>
{
    public T Value { get; }
    public List<string> Errors { get; }
    public bool IsValid => Errors.Count == 0;
    
    private ValidationResult(T value, List<string> errors)
    {
        Value = value;
        Errors = errors;
    }
    
    public static ValidationResult<T> Valid(T value) => 
        new ValidationResult<T>(value, new List<string>());
        
    public static ValidationResult<T> Invalid(List<string> errors) => 
        new ValidationResult<T>(default, errors);
}

public ValidationResult<Order> ValidateOrder(Order order)
{
    var errors = new List<string>();
    
    if (order.Customer == null)
        errors.Add("Customer information is required");
        
    if (order.Items == null || order.Items.Count == 0)
        errors.Add("Order must contain at least one item");
    else
    {
        foreach (var item in order.Items)
        {
            if (item.Quantity <= 0)
                errors.Add($"Quantity for '{item.Book.Title}' must be greater than zero");
                
            if (item.Book == null)
                errors.Add("Book information is missing for an item");
        }
    }
    
    if (order.ShippingAddress == null)
        errors.Add("Shipping address is required");
        
    return errors.Count == 0 
        ? ValidationResult<Order>.Valid(order) 
        : ValidationResult<Order>.Invalid(errors);
}

public Result<Order, List<string>> CreateValidatedOrder(Order order)
{
    var validationResult = ValidateOrder(order);
    
    if (validationResult.IsValid)
        return new Result<Order, List<string>>.Ok(validationResult.Value);
    else
        return new Result<Order, List<string>>.Error(validationResult.Errors);
}
```

## 🧪 Converting Try/Catch to Results

Wrap operations that might throw exceptions in a Result:

``` csharp
public Result<Book, string> FindBookSafely(string isbn)
{
    try
    {
        if (string.IsNullOrEmpty(isbn))
            return new Result<Book, string>.Error("ISBN cannot be empty");
            
        Book book = _bookRepository.GetByIsbn(isbn); // Might throw
        
        if (book == null)
            return new Result<Book, string>.Error($"No book found with ISBN: {isbn}");
            
        return new Result<Book, string>.Ok(book);
    }
    catch (DatabaseException ex)
    {
        _logger.Error("Database error while finding book", ex);
        return new Result<Book, string>.Error("Database error occurred. Please try again later.");
    }
    catch (Exception ex)
    {
        _logger.Error("Unexpected error while finding book", ex);
        return new Result<Book, string>.Error("An unexpected error occurred. Please try again.");
    }
}
```

## 🔄 Async Error Handling

Handling errors in asynchronous code:

``` csharp
public async Task<Result<BookAvailability, string>> CheckAvailabilityAsync(string isbn)
{
    try
    {
        var response = await _inventoryApiClient.GetAsync($"/api/books/{isbn}/availability");
        
        if (response.IsSuccessStatusCode)
        {
            var availability = await response.Content.ReadFromJsonAsync<BookAvailability>();
            return new Result<BookAvailability, string>.Ok(availability);
        }
        
        if (response.StatusCode == HttpStatusCode.NotFound)
            return new Result<BookAvailability, string>.Error($"Book with ISBN {isbn} not found");
            
        return new Result<BookAvailability, string>.Error(
            $"Error checking availability: {response.StatusCode}");
    }
    catch (HttpRequestException ex)
    {
        _logger.Error("API error", ex);
        return new Result<BookAvailability, string>.Error(
            "Cannot connect to inventory service. Please try again later.");
    }
    catch (Exception ex)
    {
        _logger.Error("Unexpected error", ex);
        return new Result<BookAvailability, string>.Error(
            "An unexpected error occurred. Please try again.");
    }
}
```
