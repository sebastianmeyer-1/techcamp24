using Dapr.Client;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

var daprClient = new DaprClientBuilder().Build();
var DAPR_STATE_STORE = "statestore";

app.UseHttpsRedirection();

app.MapGet("/status/{orderId}", async (string orderId) =>
{
    Console.WriteLine($"Status requested for {orderId}");
    string status = await daprClient.GetStateAsync<string>(DAPR_STATE_STORE, orderId);
    Console.WriteLine($"Status for Order {orderId} is {status}");

    return Results.Ok(new StatusMessage(orderId, status));
});

app.Run();

internal record StatusMessage(string orderId, string status);