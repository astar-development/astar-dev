# 🚀 Advanced Usage

This guide demonstrates more advanced patterns for using the type, including combining synchronous and asynchronous operations in a book store application. `Result<TSuccess, TError>`

## ⚡ Async Operations

### 📚 Checking Book Availability Asynchronously

``` csharp
public async Task<Result<BookAvailability, OrderError>> CheckBookAvailabilityAsync(Book book)
{
    try
    {
        // Simulating an async API call to check inventory
        var response = await _inventoryApiClient.GetAsync($"/api/books/{book.Isbn}/availability");
        
        if (response.IsSuccessStatusCode)
        {
            var availability = await response.Content.ReadFromJsonAsync<BookAvailability>();
            return new Result<BookAvailability, OrderError>.Ok(availability);
        }
        
        return new Result<BookAvailability, OrderError>.Error(
            new OrderError("AVAILABILITY_CHECK_FAILED", "Could not verify book availability"));
    }
    catch (Exception ex)
    {
        return new Result<BookAvailability, OrderError>.Error(
            new OrderError("API_ERROR", ex.Message));
    }
}
```

### 💳 Processing Payment Asynchronously

``` csharp
public async Task<Result<PaymentConfirmation, OrderError>> ProcessPaymentAsync(
    Order order, PaymentDetails paymentDetails)
{
    try
    {
        var paymentRequest = new PaymentRequest
        {
            Amount = order.TotalAmount,
            CreditCard = paymentDetails.CreditCardNumber,
            ExpiryDate = paymentDetails.ExpiryDate,
            Cvv = paymentDetails.Cvv
        };
        
        var response = await _paymentGateway.ProcessPaymentAsync(paymentRequest);
        
        if (response.Status == "SUCCESS")
            return new Result<PaymentConfirmation, OrderError>.Ok(
                new PaymentConfirmation(response.TransactionId, response.Amount));
                
        return new Result<PaymentConfirmation, OrderError>.Error(
            new OrderError("PAYMENT_FAILED", response.Message));
    }
    catch (Exception ex)
    {
        return new Result<PaymentConfirmation, OrderError>.Error(
            new OrderError("PAYMENT_ERROR", ex.Message));
    }
}
```

## 🔄 Combining Sync and Async Operations

### 🛒 Complete Order Processing Pipeline

This example shows how to combine synchronous and asynchronous operations in a complete order processing pipeline:

``` csharp
public async Task<Result<OrderConfirmation, OrderError>> PlaceOrderAsync(
    ShoppingCart cart, PaymentDetails paymentDetails)
{
    // Step 1: Validate cart (sync operation)
    Result<ShoppingCart, OrderError> validationResult = ValidateCart(cart);
    
    // Step 2: Chaining async operations with appropriate async methods
    return await validationResult
        .BindAsync(async validCart => 
        {
            // Step 3: Check availability for all books in parallel
            var availabilityTasks = validCart.Items
                .Select(item => CheckBookAvailabilityAsync(item.Book)
                    .BindAsync(async availability => 
                    {
                        if (!availability.InStock || availability.Quantity < item.Quantity)
                            return new Result<BookWithAvailability, OrderError>.Error(
                                new OrderError("INSUFFICIENT_STOCK", 
                                    $"Insufficient stock for '{item.Book.Title}'"));
                                    
                        return new Result<BookWithAvailability, OrderError>.Ok(
                            new BookWithAvailability(item.Book, availability));
                    }))
                .ToList();
                
            var availabilityResults = await Task.WhenAll(availabilityTasks);
            
            // Step 4: If any book is unavailable, return the first error
            var firstError = availabilityResults
                .FirstOrDefault(r => r is Result<BookWithAvailability, OrderError>.Error);
                
            if (firstError != null && firstError is Result<BookWithAvailability, OrderError>.Error err)
                return new Result<ShoppingCart, OrderError>.Error(err.Reason);
                
            // Step 5: Create order from valid cart
            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                Items = validCart.Items,
                TotalAmount = validCart.Items.Sum(i => i.Book.Price * i.Quantity),
                OrderDate = DateTime.UtcNow,
                Status = "PENDING"
            };
                
            return new Result<Order, OrderError>.Ok(order);
        })
        .BindAsync(order => ProcessPaymentAsync(order, paymentDetails))
        .MapAsync(async payment => 
        {
            // Step 6: Create order in database
            var order = await _orderRepository.CreateAsync(
                new Order
                {
                    OrderId = Guid.NewGuid(),
                    Items = cart.Items,
                    PaymentId = payment.TransactionId,
                    TotalAmount = payment.Amount,
                    OrderDate = DateTime.UtcNow,
                    Status = "CONFIRMED"
                });
                
            // Step 7: Send confirmation email
            await _emailService.SendOrderConfirmationAsync(order);
            
            return new OrderConfirmation
            {
                OrderId = order.OrderId,
                TotalAmount = order.TotalAmount,
                EstimatedDelivery = DateTime.UtcNow.AddDays(3),
                TrackingNumber = Guid.NewGuid().ToString("N")
            };
        })
        .TapAsync(async confirmation => 
        {
            // Step 8: Log order success
            await _logger.LogInfoAsync($"Order {confirmation.OrderId} placed successfully");
        })
        .TapErrorAsync(async error => 
        {
            // Step 9: Log error
            await _logger.LogErrorAsync($"Order failed: {error.Code} - {error.Message}");
        });
}
```

## 🧩 Parallel Processing with Results

This example shows how to process multiple results in parallel and combine them:

``` csharp
public async Task<Result<RecommendationResult, RecommendationError>> GetPersonalizedRecommendationsAsync(
    string userId, List<string> recentlyViewedBooks)
{
    try
    {
        // Step 1: Execute multiple async operations in parallel
        Task<Result<UserPreferences, RecommendationError>> userPrefsTask = 
            GetUserPreferencesAsync(userId);
            
        Task<Result<List<BookCategory>, RecommendationError>> topCategoriesTask = 
            GetUserTopCategoriesAsync(userId);
            
        Task<Result<List<Book>, RecommendationError>> recentTrendsTask = 
            GetRecentTrendingBooksAsync();
            
        // Step 2: Wait for all tasks to complete
        await Task.WhenAll(userPrefsTask, topCategoriesTask, recentTrendsTask);
        
        // Step 3: Extract results
        var userPrefsResult = await userPrefsTask;
        var topCategoriesResult = await topCategoriesTask;
        var recentTrendsResult = await recentTrendsTask;
        
        // Step 4: Handle errors
        if (userPrefsResult is Result<UserPreferences, RecommendationError>.Error userPrefsError)
            return new Result<RecommendationResult, RecommendationError>.Error(userPrefsError.Reason);
            
        if (topCategoriesResult is Result<List<BookCategory>, RecommendationError>.Error categoriesError)
            return new Result<RecommendationResult, RecommendationError>.Error(categoriesError.Reason);
            
        if (recentTrendsResult is Result<List<Book>, RecommendationError>.Error trendsError)
            return new Result<RecommendationResult, RecommendationError>.Error(trendsError.Reason);
            
        // Step 5: Extract success values
        var userPrefs = ((Result<UserPreferences, RecommendationError>.Ok)userPrefsResult).Value;
        var topCategories = ((Result<List<BookCategory>, RecommendationError>.Ok)topCategoriesResult).Value;
        var recentTrends = ((Result<List<Book>, RecommendationError>.Ok)recentTrendsResult).Value;
        
        // Step 6: Generate recommendations based on all the data
        var recommendations = _recommendationEngine.GenerateRecommendations(
            userPrefs, topCategories, recentTrends, recentlyViewedBooks);
            
        return new Result<RecommendationResult, RecommendationError>.Ok(
            new RecommendationResult
            {
                Recommendations = recommendations,
                GeneratedAt = DateTime.UtcNow,
                UserId = userId
            });
    }
    catch (Exception ex)
    {
        return new Result<RecommendationResult, RecommendationError>.Error(
            new RecommendationError("RECOMMENDATION_FAILED", ex.Message));
    }
}
```

## 🏭 Creating Factory Methods

For improved readability, you can create factory methods:

``` csharp
public static class Results
{
    // Success factory methods
    public static Result<TSuccess, TError> Success<TSuccess, TError>(TSuccess value)
        => new Result<TSuccess, TError>.Ok(value);
        
    public static Result<TSuccess, string> Success<TSuccess>(TSuccess value)
        => new Result<TSuccess, string>.Ok(value);
        
    // Error factory methods
    public static Result<TSuccess, TError> Failure<TSuccess, TError>(TError error)
        => new Result<TSuccess, TError>.Error(error);
        
    public static Result<TSuccess, string> Failure<TSuccess>(string errorMessage)
        => new Result<TSuccess, string>.Error(errorMessage);
        
    // Try method to convert exceptions to Results
    public static Result<TSuccess, Exception> Try<TSuccess>(Func<TSuccess> func)
    {
        try
        {
            return Success<TSuccess, Exception>(func());
        }
        catch (Exception ex)
        {
            return Failure<TSuccess, Exception>(ex);
        }
    }
    
    // Async Try method
    public static async Task<Result<TSuccess, Exception>> TryAsync<TSuccess>(
        Func<Task<TSuccess>> func)
    {
        try
        {
            return Success<TSuccess, Exception>(await func());
        }
        catch (Exception ex)
        {
            return Failure<TSuccess, Exception>(ex);
        }
    }
}
```

Usage:

``` csharp
// Using factory methods
var successResult = Results.Success<Order, OrderError>(order);
var errorResult = Results.Failure<Order, OrderError>(new OrderError("INVALID_ORDER", "Order is invalid"));

// Using Try to convert exceptions to Results
Result<int, Exception> divisionResult = Results.Try(() => 10 / userInput);

divisionResult.Match(
    result => Console.WriteLine($"Result: {result}"),
    ex => Console.WriteLine($"Error: {ex.Message}")
);
```

## 🔄 Converting Between Different Error Types

``` csharp
public Result<Order, UserFriendlyError> ConvertToUserFriendlyError(
    Result<Order, TechnicalError> technicalResult)
{
    return technicalResult.MapFailure(technicalError => 
    {
        switch (technicalError.Code)
        {
            case "DB_CONNECTION":
                return new UserFriendlyError(
                    "We're experiencing technical difficulties. Please try again later.");
                    
            case "INVENTORY_API":
                return new UserFriendlyError(
                    "Cannot verify product availability right now. Please try again.");
                    
            default:
                _logger.Error($"Unhandled technical error: {technicalError.Code}", technicalError.Exception);
                return new UserFriendlyError(
                    "An unexpected error occurred. Our team has been notified.");
        }
    });
}
```
