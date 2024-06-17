using Dapr;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapPost("/brew", (CloudEvent<Order> order) =>
{
    var orderData = order.Data;
    Console.WriteLine($"Barrista legt los mit der Bestellung {orderData.orderId} und bereitet {orderData.description} zu.");
    return Results.Ok();
});

app.Run();

public record Order(string orderId, string description);