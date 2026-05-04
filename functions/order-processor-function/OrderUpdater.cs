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
    public class OrderUpdater
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OrderUpdater> _logger;
        private readonly ServiceBusSender _sender;

        public OrderUpdater(IHttpClientFactory factory, ILogger<OrderUpdater> logger, ServiceBusSender sender)
        {
            _httpClient = factory.CreateClient("ResilientClient");
            _logger = logger;
            _sender = sender;
        }

        [Function("OrderUpdater")]
        public async Task Run(
            [ServiceBusTrigger("order-events", "order-sub", Connection = "ServiceBusConnection")]
        ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            string? eventType = null;

            if (message.ApplicationProperties.ContainsKey("eventType"))
            {
                eventType = message.ApplicationProperties["eventType"]?.ToString();
            }

            var body = message.Body.ToString();
            var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(body);

            try
            {
                _logger.LogInformation("OrderUpdater received {eventType} for Order {id}",
                    eventType, orderEvent?.OrderId);

                if (eventType == "PaymentFailed")
                {
                    await _httpClient.PutAsync(
                        $"https://localhost:7083/api/order/{orderEvent!.OrderId}/status?status=Failed",
                        null);
                }
                else if (eventType == "InventoryCompleted")
                {
                    await _httpClient.PutAsync(
                        $"https://localhost:7083/api/order/{orderEvent!.OrderId}/status?status=Completed",
                        null);
                }
                else if (eventType == "InventoryFailed")
                {
                    // Refund
                    var refundEvent = new ServiceBusMessage(body);
                    refundEvent.ApplicationProperties["eventType"] = "RefundRequested";

                    await _sender.SendMessageAsync(refundEvent);

                    await _httpClient.PutAsync(
                        $"https://localhost:7083/api/order/{orderEvent!.OrderId}/status?status=Refunded",
                        null);
                }

                await messageActions.CompleteMessageAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OrderUpdater failed");

                await messageActions.AbandonMessageAsync(message);
            }
        }
    }
}
