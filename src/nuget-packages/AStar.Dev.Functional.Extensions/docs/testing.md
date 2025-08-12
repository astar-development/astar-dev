# 🧪 Testing with Results

This document demonstrates effective patterns for testing code that uses the `Result<TSuccess, TError>` type.

## 🎯 Testing Functions that Return Results

Testing functions that return `Result<TSuccess, TError>` is straightforward because all possible outcomes are explicitly modeled.

### 📚 Example: Testing a Book Service

```csharp
public class BookService
{
    private readonly IBookRepository _repository;
    
    public BookService(IBookRepository repository)
    {
        _repository = repository;
    }
    
    public Result<Book, string> FindBook(string isbn)
    {
        if (string.IsNullOrEmpty(isbn))
            return new Result<Book, string>.Error("ISBN cannot be empty");
            
        Book book = _repository.GetByIsbn(isbn);
        
        if (book == null)
            return new Result<Book, string>.Error($"No book found with ISBN: {isbn}");
            
        return new Result<Book, string>.Ok(book);
    }
}
```

### ✅ Testing Success Case

```csharp
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
    Assert.IsType<Result<Book, string>.Ok>(result);
    
    if (result is Result<Book, string>.Ok success)
    {
        Assert.Equal("Test Book", success.Value.Title);
    }
    else
    {
        Assert.Fail("Result should be Ok type");
    }
}
```

### ❌ Testing Error Cases

```csharp
[Fact]
public void FindBook_WithEmptyIsbn_ReturnsError()
{
    // Arrange
    var mockRepo = new Mock<IBookRepository>();
    var service = new BookService(mockRepo.Object);
    
    // Act
    var result = service.FindBook("");
    
    // Assert
    Assert.IsType<Result<Book, string>.Error>(result);
    
    if (result is Result<Book, string>.Error error)
    {
        Assert.Equal("ISBN cannot be empty", error.Reason);
    }
    else
    {
        Assert.Fail("Result should be Error type");
    }
}

[Fact]
public void FindBook_WithNonExistentIsbn_ReturnsError()
{
    // Arrange
    var mockRepo = new Mock<IBookRepository>();
    mockRepo.Setup(r => r.GetByIsbn("nonexistent")).Returns((Book)null);
    var service = new BookService(mockRepo.Object);
    
    // Act
    var result = service.FindBook("nonexistent");
    
    // Assert
    Assert.IsType<Result<Book, string>.Error>(result);
    
    if (result is Result<Book, string>.Error error)
    {
        Assert.Contains("No book found with ISBN", error.Reason);
    }
    else
    {
        Assert.Fail("Result should be Error type");
    }
}
```

## 🧪 Testing Async Methods

For testing asynchronous methods that return `Task<Result<TSuccess, TError>>`:

```csharp
public class OrderService
{
    private readonly IPaymentGateway _paymentGateway;
    
    public OrderService(IPaymentGateway paymentGateway)
    {
        _paymentGateway = paymentGateway;
    }
    
    public async Task<Result<PaymentConfirmation, OrderError>> ProcessPaymentAsync(
        Order order, PaymentDetails paymentDetails)
    {
        try
        {
            var response = await _paymentGateway.ProcessPaymentAsync(
                new PaymentRequest(order.TotalAmount, paymentDetails));
                
            if (response.Success)
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
}
```

### ✅ Testing Async Success Case

```csharp
[Fact]
public async Task ProcessPayment_WithValidDetails_ReturnsSuccess()
{
    // Arrange
    var order = new Order { TotalAmount = 100m };
    var paymentDetails = new PaymentDetails { CardNumber = "4111111111111111" };
    
    var mockGateway = new Mock<IPaymentGateway>();
    mockGateway.Setup(g => g.ProcessPaymentAsync(It.IsAny<PaymentRequest>()))
        .ReturnsAsync(new PaymentResponse 
        { 
            Success = true, 
            TransactionId = "tx123", 
            Amount = 100m 
        });
        
    var service = new OrderService(mockGateway.Object);
    
    // Act
    var result = await service.ProcessPaymentAsync(order, paymentDetails);
    
    // Assert
    Assert.IsType<Result<PaymentConfirmation, OrderError>.Ok>(result);
    
    if (result is Result<PaymentConfirmation, OrderError>.Ok success)
    {
        Assert.Equal("tx123", success.Value.TransactionId);
        Assert.Equal(100m, success.Value.Amount);
    }
}
```

### ❌ Testing Async Error Case

```csharp
[Fact]
public async Task ProcessPayment_WithDeclinedCard_ReturnsError()
{
    // Arrange
    var order = new Order { TotalAmount = 100m };
    var paymentDetails = new PaymentDetails { CardNumber = "4111111111111111" };
    
    var mockGateway = new Mock<IPaymentGateway>();
    mockGateway.Setup(g => g.ProcessPaymentAsync(It.IsAny<PaymentRequest>()))
        .ReturnsAsync(new PaymentResponse 
        { 
            Success = false, 
            Message = "Card declined" 
        });
        
    var service = new OrderService(mockGateway.Object);
    
    // Act
    var result = await service.ProcessPaymentAsync(order, paymentDetails);
    
    // Assert
    Assert.IsType<Result<PaymentConfirmation, OrderError>.Error>(result);
    
    if (result is Result<PaymentConfirmation, OrderError>.Error error)
    {
        Assert.Equal("PAYMENT_FAILED", error.Reason.Code);
        Assert.Equal("Card declined", error.Reason.Message);
    }
}

[Fact]
public async Task ProcessPayment_WithException_ReturnsError()
{
    // Arrange
    var order = new Order { TotalAmount = 100m };
    var paymentDetails = new PaymentDetails { CardNumber = "4111111111111111" };
    
    var mockGateway = new Mock<IPaymentGateway>();
    mockGateway.Setup(g => g.ProcessPaymentAsync(It.IsAny<PaymentRequest>()))
        .ThrowsAsync(new Exception("Connection error"));
        
    var service = new OrderService(mockGateway.Object);
    
    // Act
    var result = await service.ProcessPaymentAsync(order, paymentDetails);
    
    // Assert
    Assert.IsType<Result<PaymentConfirmation, OrderError>.Error>(result);
    
    if (result is Result<PaymentConfirmation, OrderError>.Error error)
    {
        Assert.Equal("PAYMENT_ERROR", error.Reason.Code);
        Assert.Equal("Connection error", error.Reason.Message);
    }
}
```

## 🔗 Testing Complex Chains

For testing complex chains of operations using `Bind` and `Map`:

```csharp
public class OrderProcessor
{
    private readonly IBookService _bookService;
    private readonly IInventoryService _inventoryService;
    private readonly IPaymentService _paymentService;
    
    public OrderProcessor(
        IBookService bookService,
        IInventoryService inventoryService,
        IPaymentService paymentService)
    {
        _bookService = bookService;
        _inventoryService = inventoryService;
        _paymentService = paymentService;
    }
    
    public Result<OrderConfirmation, string> ProcessOrder(string isbn, int quantity, PaymentDetails payment)
    {
        return _bookService.FindBook(isbn)
            .Bind(book => _inventoryService.CheckAvailability(book, quantity))
            .Bind(bookWithAvailability => _paymentService.ProcessPayment(
                bookWithAvailability.Book.Price * quantity, payment))
            .Map(paymentConfirmation => new OrderConfirmation
            {
                OrderId = Guid.NewGuid(),
                Book = isbn,
                Quantity = quantity,
                TotalAmount = paymentConfirmation.Amount,
                TransactionId = paymentConfirmation.TransactionId
            });
    }
}
```

### 🧪 Testing the Happy Path

```csharp
[Fact]
public void ProcessOrder_WhenAllStepsSucceed_ReturnsOrderConfirmation()
{
    // Arrange
    var book = new Book { Isbn = "123456789", Title = "Test Book", Price = 10m };
    var bookWithAvailability = new BookWithAvailability(book, new Availability { InStock = true, Quantity = 5 });
    var paymentConfirmation = new PaymentConfirmation("tx123", 20m);
    
    var mockBookService = new Mock<IBookService>();
    mockBookService
        .Setup(s => s.FindBook("123456789"))
        .Returns(new Result<Book, string>.Ok(book));
        
    var mockInventoryService = new Mock<IInventoryService>();
    mockInventoryService
        .Setup(s => s.CheckAvailability(book, 2))
        .Returns(new Result<BookWithAvailability, string>.Ok(bookWithAvailability));
        
    var mockPaymentService = new Mock<IPaymentService>();
    mockPaymentService
        .Setup(s => s.ProcessPayment(20m, It.IsAny<PaymentDetails>()))
        .Returns(new Result<PaymentConfirmation, string>.Ok(paymentConfirmation));
        
    var processor = new OrderProcessor(
        mockBookService.Object,
        mockInventoryService.Object,
        mockPaymentService.Object);
        
    // Act
    var result = processor.ProcessOrder("123456789", 2, new PaymentDetails());
    
    // Assert
    Assert.IsType<Result<OrderConfirmation, string>.Ok>(result);
    
    if (result is Result<OrderConfirmation, string>.Ok success)
    {
        Assert.Equal("123456789", success.Value.Book);
        Assert.Equal(2, success.Value.Quantity);
        Assert.Equal(20m, success.Value.TotalAmount);
        Assert.Equal("tx123", success.Value.TransactionId);
    }
}
```

### 🧪 Testing Failure Cases

```csharp
[Fact]
public void ProcessOrder_WhenBookNotFound_ReturnsError()
{
    // Arrange
    var mockBookService = new Mock<IBookService>();
    mockBookService
        .Setup(s => s.FindBook("nonexistent"))
        .Returns(new Result<Book, string>.Error("Book not found"));
        
    var mockInventoryService = new Mock<IInventoryService>();
    var mockPaymentService = new Mock<IPaymentService>();
        
    var processor = new OrderProcessor(
        mockBookService.Object,
        mockInventoryService.Object,
        mockPaymentService.Object);
        
    // Act
    var result = processor.ProcessOrder("nonexistent", 1, new PaymentDetails());
    
    // Assert
    Assert.IsType<Result<OrderConfirmation, string>.Error>(result);
    
    if (result is Result<OrderConfirmation, string>.Error error)
    {
        Assert.Equal("Book not found", error.Reason);
    }
}

[Fact]
public void ProcessOrder_WhenOutOfStock_ReturnsError()
{
    // Arrange
    var book = new Book { Isbn = "123456789", Title = "Test Book", Price = 10m };
    
    var mockBookService = new Mock<IBookService>();
    mockBookService
        .Setup(s => s.FindBook("123456789"))
        .Returns(new Result<Book, string>.Ok(book));
        
    var mockInventoryService = new Mock<IInventoryService>();
    mockInventoryService
        .Setup(s => s.CheckAvailability(book, 10))
        .Returns(new Result<BookWithAvailability, string>.Error("Insufficient stock"));
        
    var mockPaymentService = new Mock<IPaymentService>();
        
    var processor = new OrderProcessor(
        mockBookService.Object,
        mockInventoryService.Object,
        mockPaymentService.Object);
        
    // Act
    var result = processor.ProcessOrder("123456789", 10, new PaymentDetails());
    
    // Assert
    Assert.IsType<Result<OrderConfirmation, string>.Error>(result);
    
    if (result is Result<OrderConfirmation, string>.Error error)
    {
        Assert.Equal("Insufficient stock", error.Reason);
    }
}
```

## 🔬 Test Helpers for Results

You can create test helpers to make assertions more readable:

```csharp
public static class ResultAssertions
{
    public static void AssertOk<TSuccess, TError>(
        this Result<TSuccess, TError> result, 
        Action<TSuccess> successCheck = null)
    {
        if (result is Result<TSuccess, TError>.Ok ok)
        {
            successCheck?.Invoke(ok.Value);
        }
        else if (result is Result<TSuccess, TError>.Error err)
        {
            Assert.Fail($"Expected Ok result but got Error: {err.Reason}");
        }
        else
        {
            Assert.Fail("Result is neither Ok nor Error");
        }
    }
    
    public static void AssertError<TSuccess, TError>(
        this Result<TSuccess, TError> result, 
        Action<TError> errorCheck = null)
    {
        if (result is Result<TSuccess, TError>.Error err)
        {
            errorCheck?.Invoke(err.Reason);
        }
        else if (result is Result<TSuccess, TError>.Ok ok)
        {
            Assert.Fail($"Expected Error result but got Ok: {ok.Value}");
        }
        else
        {
            Assert.Fail("Result is neither Ok nor Error");
        }
    }
}
```

Usage:

```csharp
[Fact]
public void FindBook_WithValidIsbn_ReturnsBook()
{
    // Arrange
    var service = new BookService(mockRepo.Object);
    
    // Act
    var result = service.FindBook("123456789");
    
    // Assert
    result.AssertOk(book => 
    {
        Assert.Equal("123456789", book.Isbn);
        Assert.Equal("Test Book", book.Title);
    });
}

[Fact]
public void FindBook_WithEmptyIsbn_ReturnsError()
{
    // Arrange
    var service = new BookService(mockRepo.Object);
    
    // Act
    var result = service.FindBook("");
    
    // Assert
    result.AssertError(error => 
    {
        Assert.Equal("ISBN cannot be empty", error);
    });
}
```
