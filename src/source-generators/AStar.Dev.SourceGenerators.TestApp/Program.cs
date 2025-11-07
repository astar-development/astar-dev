using AStar.Dev.SourceGenerators.TestApp;

Console.WriteLine(Hello.Message); // Stage 0: generated


var order = new Order { Id = OrderId.New(), Status = "Open", Quantity = 3 };
var dto = order.ToOrderDto();
Console.WriteLine($"{dto.Id} {dto.Status} {dto.Quantity}");
