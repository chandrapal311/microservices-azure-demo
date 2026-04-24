using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Shared.Contracts;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace order_processor_function;

public class Function1
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<Function1> _logger;

    public Function1(IHttpClientFactory factory, ILogger<Function1> logger)
    {
        _httpClient = factory.CreateClient("ResilientClient");
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

        var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(body);

        _logger.LogInformation("Order received: {id}, Amount: {amount}",
            orderEvent?.OrderId,
            orderEvent?.Amount);

        _logger.LogInformation("Processing Order {id}, CorrelationId: {cid}", orderEvent?.OrderId, orderEvent?.CorrelationId);

        var paymentContent = new StringContent(JsonSerializer.Serialize(orderEvent), Encoding.UTF8, "application/json");

        try
        {
            _httpClient.DefaultRequestHeaders.Add("x-correlation-id", orderEvent?.CorrelationId);
            // Call Payment Service
            var paymentResponse = await _httpClient.PostAsync("https://localhost:7076/api/payment", paymentContent);
            if (!paymentResponse.IsSuccessStatusCode)
            {
                throw new Exception("Payment failed");
            }
            else
            {
                await _httpClient.PutAsync($"https://localhost:7083/api/order/{orderEvent.OrderId}/status?status=PaymentCompleted", null);
            }

            _logger.LogInformation("Payment status: {status}", paymentResponse.StatusCode);

            //Call Inventory Service
            var inventoryContent = new StringContent(JsonSerializer.Serialize(orderEvent), Encoding.UTF8, "application/json");

            var inventoryResponse = await _httpClient.PostAsync("https://localhost:7177/api/inventory", inventoryContent);
            if (!inventoryResponse.IsSuccessStatusCode)
            {
                throw new Exception("Inventory failed");
            }
            else
            {
                await _httpClient.PutAsync($"https://localhost:7083/api/order/{orderEvent.OrderId}/status?status=Completed", null);
            }

            _logger.LogInformation("Inventory status: {status}", inventoryResponse.StatusCode);
            // Complete the message
            //await messageActions.CompleteMessageAsync(message);
            await messageActions.CompleteMessageAsync(message);
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "Error processing order");
            await _httpClient.PutAsync($"https://localhost:7083/api/order/{orderEvent.OrderId}/status?status=Failed",
    null);
            // Compensation: Refund payment
            await _httpClient.PostAsync("https://localhost:7076/api/payment/refund",paymentContent);

            await _httpClient.PutAsync($"https://localhost:7083/api/order/{orderEvent.OrderId}/status?status=Refunded", null);

            _logger.LogInformation("Payment refunded");

            // Retry later
            await messageActions.AbandonMessageAsync(message);
        }

    }
}
