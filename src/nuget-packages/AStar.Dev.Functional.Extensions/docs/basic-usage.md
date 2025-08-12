# 📖 Basic Usage Guide

This guide demonstrates the fundamental patterns for using the type in a book store application context. `Result<TSuccess, TError>`

## 📚 Finding a Book

Let's start with a simple function to find a book by ISBN:

``` csharp
public Result<Book, string> FindBook(string isbn)
{
    if (string.IsNullOrEmpty(isbn))
        return new Result<Book, string>.Error("ISBN cannot be empty");
        
    Book book = _bookRepository.GetByIsbn(isbn);
    
    if (book == null)
        return new Result<Book, string>.Error($"No book found with ISBN: {isbn}");
        
    return new Result<Book, string>.Ok(book);
}
```

### 🔍 Using the Result

``` csharp
Result<Book, string> result = FindBook("978-0439708180");

// Match on result to handle both cases
string message = result.Match(
    book => $"Found: {book.Title} by {book.Author}",
    error => $"Error: {error}"
);

// Display the message
Console.WriteLine(message);
```

## 🛒 Adding a Book to Cart

``` csharp
public Result<ShoppingCart, string> AddToCart(ShoppingCart cart, string isbn, int quantity)
{
    // Validate inputs
    if (cart == null)
        return new Result<ShoppingCart, string>.Error("Shopping cart cannot be null");
        
    if (quantity <= 0)
        return new Result<ShoppingCart, string>.Error("Quantity must be greater than zero");
    
    // Find the book
    Result<Book, string> bookResult = FindBook(isbn);
    
    // Match on the book result
    return bookResult.Match(
        book => 
        {
            // Add the book to cart
            cart.AddItem(new CartItem(book, quantity));
            return new Result<ShoppingCart, string>.Ok(cart);
        },
        error => new Result<ShoppingCart, string>.Error(error)
    );
}
```

### 🔄 Using Map Instead of Match

We can simplify the above example using : `Map`

``` csharp
public Result<ShoppingCart, string> AddToCart(ShoppingCart cart, string isbn, int quantity)
{
    // Validate inputs
    if (cart == null)
        return new Result<ShoppingCart, string>.Error("Shopping cart cannot be null");
        
    if (quantity <= 0)
        return new Result<ShoppingCart, string>.Error("Quantity must be greater than zero");
    
    // Find the book and map to cart
    return FindBook(isbn).Map(book => 
    {
        cart.AddItem(new CartItem(book, quantity));
        return cart;
    });
}
```

## ⛓️ Chaining Operations

Let's implement a simple checkout process:

``` csharp
// Step 1: Validate cart
public Result<ShoppingCart, string> ValidateCart(ShoppingCart cart)
{
    if (cart == null || cart.Items.Count == 0)
        return new Result<ShoppingCart, string>.Error("Cart is empty");
        
    return new Result<ShoppingCart, string>.Ok(cart);
}

// Step 2: Calculate total
public Result<decimal, string> CalculateTotal(ShoppingCart cart)
{
    decimal total = cart.Items.Sum(item => item.Book.Price * item.Quantity);
    
    if (total <= 0)
        return new Result<decimal, string>.Error("Total must be greater than zero");
        
    return new Result<decimal, string>.Ok(total);
}

// Step 3: Process payment
public Result<PaymentConfirmation, string> ProcessPayment(decimal amount, PaymentDetails payment)
{
    // Simulate payment processing
    if (payment.CardNumber.EndsWith("0000"))
        return new Result<PaymentConfirmation, string>.Error("Payment declined");
        
    var confirmation = new PaymentConfirmation
    {
        TransactionId = Guid.NewGuid().ToString(),
        Amount = amount,
        Date = DateTime.Now
    };
    
    return new Result<PaymentConfirmation, string>.Ok(confirmation);
}
```

### ⚙️ Using Bind to Chain Operations

Now we can chain these operations using : `Bind`

``` csharp
public Result<OrderConfirmation, string> Checkout(ShoppingCart cart, PaymentDetails payment)
{
    return ValidateCart(cart)
        .Bind(validCart => CalculateTotal(validCart))
        .Bind(total => ProcessPayment(total, payment))
        .Map(paymentConfirmation => new OrderConfirmation
        {
            OrderId = Guid.NewGuid(),
            PaymentId = paymentConfirmation.TransactionId,
            TotalAmount = paymentConfirmation.Amount,
            OrderDate = DateTime.Now
        });
}
```

## 📝 Handling Side Effects with Tap

``` csharp
public Result<OrderConfirmation, string> PlaceOrder(ShoppingCart cart, PaymentDetails payment)
{
    return Checkout(cart, payment)
        .Tap(confirmation => 
        {
            // Log successful order
            _logger.Info($"Order {confirmation.OrderId} placed successfully");
            
            // Send confirmation email
            _emailService.SendOrderConfirmation(confirmation);
        })
        .TapError(error => 
        {
            // Log error
            _logger.Error($"Order failed: {error}");
        });
}
```

## 🔀 Transforming Errors

``` csharp
public Result<OrderConfirmation, UserFriendlyError> PlaceOrderWithFriendlyErrors(
    ShoppingCart cart, PaymentDetails payment)
{
    return PlaceOrder(cart, payment)
        .MapFailure(errorMessage => 
        {
            if (errorMessage.Contains("Payment declined"))
                return new UserFriendlyError("Your payment was declined. Please try a different payment method.");
                
            if (errorMessage.Contains("out of stock"))
                return new UserFriendlyError("One or more items in your cart are no longer available.");
                
            return new UserFriendlyError("We couldn't process your order. Please try again later.");
        });
}
```

## 🎯 Using the Result in UI

``` csharp
// In a controller or UI handler
Result<OrderConfirmation, string> orderResult = bookStore.PlaceOrder(cart, paymentDetails);

orderResult.Match(
    confirmation => 
    {
        // Show success view
        ShowOrderConfirmation(confirmation);
    },
    error => 
    {
        // Show error view
        ShowErrorMessage(error);
    }
);
```
