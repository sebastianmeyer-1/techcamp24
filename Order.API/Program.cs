using Dapr.Client;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

var paymentClient = DaprClient.CreateInvokeHttpClient(appId: "payment-processor");
var daprClient = new DaprClientBuilder().Build();

var DAPR_STATE_STORE = "statestore";

app.MapPost("/order", async (Order order) =>
{
    Console.WriteLine("New Order Request received");
    
    await daprClient.SaveStateAsync(DAPR_STATE_STORE, order.orderId, "OrderRequestReceived");
    
    Console.WriteLine($"Current State of Order '{order.orderId}' is {await daprClient.GetStateAsync<string>(DAPR_STATE_STORE, order.orderId)}");

    // Simulate processing with active waiting ;-)
    // Only for DEMO. Only for DEMO. Only for DEMO.
    await Task.Delay(5000);

    var response = await paymentClient.PostAsJsonAsync("/payment", order);
    if (response is null || !response.IsSuccessStatusCode)
    {
        return Results.BadRequest("Error while processing your Order. Please try again.");
    }
    
    var cancellationToken = new CancellationToken();

    // Simulate processing with active waiting ;-)
    // Only for DEMO. Only for DEMO. Only for DEMO.
    await Task.Delay(10000);

    await daprClient.PublishEventAsync<Order>("orderqueue", "orders", order, cancellationToken);

    return Results.Ok(await response.Content.ReadFromJsonAsync<PaymentAccepted>());
});

app.Run();

internal record PaymentAccepted(string paymentId, string reference, string status);

internal record Order(string orderId, string description);

