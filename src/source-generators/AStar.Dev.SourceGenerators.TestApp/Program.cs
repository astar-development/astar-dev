using AStar.Dev.SourceGenerators.TestApp;

var order = new Order { Id = OrderId.New(), Status = "Open", Quantity = 3 };
Console.WriteLine(order);
