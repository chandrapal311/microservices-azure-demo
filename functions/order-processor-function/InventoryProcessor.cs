using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace order_processor_function
{
    public class InventoryProcessor
    {
        private readonly HttpClient _httpClient;
        private readonly ServiceBusSender _sender;
        private readonly ILogger<InventoryProcessor> _logger;

        public InventoryProcessor(IHttpClientFactory factory, ServiceBusClient client, ILogger<InventoryProcessor> logger)
        {
            _httpClient = factory.CreateClient("ResilientClient");
            _sender = client.CreateSender("order-events");
            _logger = logger;
        }

        [Function("InventoryProcessor")]
        public async Task Run(
            [ServiceBusTrigger("order-events", "inventory-sub", Connection = "ServiceBusConnection")]
        ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {

            var correlationId = message.ApplicationProperties.ContainsKey("CorrelationId")
    ? message.ApplicationProperties["CorrelationId"]?.ToString()
    : Guid.NewGuid().ToString();

            _logger.LogInformation("CID: {cid}", correlationId);
            string? eventType = null;

            if (message.ApplicationProperties.ContainsKey("eventType"))
            {
                eventType = message.ApplicationProperties["eventType"]?.ToString();
            }

            if (eventType != "PaymentCompleted")
            {
                _logger.LogInformation("Skipping event: {eventType}", eventType);
                await messageActions.CompleteMessageAsync(message);
                return;
            }

            var body = message.Body.ToString();
            var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(body);

            try
            {
                _logger.LogInformation("Inventory processing Order {id}", orderEvent?.OrderId);

                var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7177/api/inventory");
                request.Headers.Add("x-correlation-id", correlationId);
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Inventory failed");

                var nextEvent = new ServiceBusMessage(body);
                nextEvent.ApplicationProperties["eventType"] = "InventoryCompleted";

                await _sender.SendMessageAsync(nextEvent);

                await messageActions.CompleteMessageAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Inventory failed");

                var failEvent = new ServiceBusMessage(body);
                failEvent.ApplicationProperties["eventType"] = "InventoryFailed";

                await _sender.SendMessageAsync(failEvent);

                await messageActions.CompleteMessageAsync(message);
            }
        }

    }
}
