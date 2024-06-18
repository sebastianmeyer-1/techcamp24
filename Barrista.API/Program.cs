using Dapr;
using Dapr.Client;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

var daprClient = new DaprClientBuilder().Build();
var DAPR_STATE_STORE = "statestore";

app.MapPost("/brew", async (CloudEvent<Order> order) =>
{
    var orderData = order.Data;  
    
    await daprClient.SaveStateAsync(DAPR_STATE_STORE, orderData.orderId, "BarristaIsPreparing");
    Console.WriteLine($"Barrista legt los mit der Bestellung {orderData.orderId} und bereitet {orderData.description} zu.");
    
    await Task.Delay(30000);

    await daprClient.SaveStateAsync(DAPR_STATE_STORE, orderData.orderId, "BarristaCompletedYourOrder");
    Console.WriteLine($"Enjoy your {orderData.description}");
    
    return Results.Ok();
});

app.Run();

public record Order(string orderId, string description);