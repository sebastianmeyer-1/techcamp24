using Dapr.Client;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

var daprClient = new DaprClientBuilder().Build();
var DAPR_STATE_STORE = "statestore";
var SUCCESS_STATE = "PaymentAccepted";

app.MapPost("/payment", async (Order order) =>
{
    Console.WriteLine($"Payment Request received for Order: '{order.orderId}'");

    await daprClient.SaveStateAsync(DAPR_STATE_STORE, order.orderId, SUCCESS_STATE);

    Console.WriteLine($"Current State of Order '{order.orderId}' is {await daprClient.GetStateAsync<string>(DAPR_STATE_STORE, order.orderId)}");

    return Results.Ok(new PaymentAccepted(Guid.NewGuid().ToString(), order.orderId, SUCCESS_STATE));
});

app.Run();

public record PaymentAccepted(string paymentId, string reference, string status);

public record Order(string orderId, string description);
