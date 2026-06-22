using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace order_processor_function
{
    public class RefundProcessor
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RefundProcessor> _logger;

        public RefundProcessor(IHttpClientFactory factory, ILogger<RefundProcessor> logger)
        {
            _httpClient = factory.CreateClient("ResilientClient");
            _logger = logger;
        }

        [Function("RefundProcessor")]
        public async Task Run(
            [ServiceBusTrigger("order-events", "refund-sub", Connection = "ServiceBusConnection")]
        ServiceBusReceivedMessage message,
            ServiceBusMessageActions actions)
        {
            var body = message.Body.ToString();

            await _httpClient.PostAsync(
                "https://localhost:7076/api/payment/refund",
                new StringContent(body, Encoding.UTF8, "application/json"));

            await actions.CompleteMessageAsync(message);
        }
    }
}
