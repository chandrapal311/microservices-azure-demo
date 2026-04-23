using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace order_processor_function;

public class Function1
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<Function1> _logger;

    public Function1(IHttpClientFactory factory, ILogger<Function1> logger)
    {
        _httpClient = factory.CreateClient();
        _logger = logger;
    }

    [Function(nameof(Function1))]
    public async Task Run(
        [ServiceBusTrigger("order-queue", Connection = "ServiceBusConnection")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        _logger.LogInformation("Message ID: {id}", message.MessageId);
        _logger.LogInformation("Message Body: {body}", message.Body);
        _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);
        var body = message.Body.ToString();

        // Call Payment Service
        var paymentResponse = await _httpClient.PostAsync(
            "https://localhost:5001/api/payment", null);

        _logger.LogInformation("Payment status: {status}", paymentResponse.StatusCode);

        //Call Inventory Service
        var inventoryResponse = await _httpClient.PostAsync(
            "https://localhost:5002/api/inventory", null);

        _logger.LogInformation("Inventory status: {status}", inventoryResponse.StatusCode);
        // Complete the message
        //await messageActions.CompleteMessageAsync(message);
        await Task.CompletedTask;
    }
}