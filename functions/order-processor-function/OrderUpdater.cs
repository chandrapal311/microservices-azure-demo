using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using order_processor_function.Services;
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
        private readonly AuthenticatedApiClient _apiClient;
        private readonly ILogger<OrderUpdater> _logger;
        private readonly ServiceBusSender _sender;

        public OrderUpdater(AuthenticatedApiClient apiClient, ILogger<OrderUpdater> logger, ServiceBusSender sender)
        {
            _apiClient = apiClient;
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

            var correlationId = message.ApplicationProperties.ContainsKey("CorrelationId")
    ? message.ApplicationProperties["CorrelationId"]?.ToString()
    : Guid.NewGuid().ToString();

            var body = message.Body.ToString();
            var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(body);

            try
            {
                _logger.LogInformation("OrderUpdater received {eventType} for Order {id}",
                    eventType, orderEvent?.OrderId);

                if (eventType == "PaymentFailed")
                {
                    await _apiClient.PutAsync(
                        $"https://localhost:7083/api/order/{orderEvent!.OrderId}/status?status=Failed",
                        correlationId!);
                }
                else if (eventType == "InventoryCompleted")
                {
                    await _apiClient.PutAsync(
                        $"https://localhost:7083/api/order/{orderEvent!.OrderId}/status?status=Completed",
                        correlationId!);
                }
                else if (eventType == "InventoryFailed")
                {
                    // Refund
                    var refundEvent = new ServiceBusMessage(body);
                    refundEvent.ApplicationProperties["eventType"] = "RefundRequested";

                    await _sender.SendMessageAsync(refundEvent);

                    await _apiClient.PutAsync(
                        $"https://localhost:7083/api/order/{orderEvent!.OrderId}/status?status=Refunded",
                        correlationId!);
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
