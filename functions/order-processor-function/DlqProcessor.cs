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
    public class DlqProcessor
    {
        private readonly ILogger<DlqProcessor> _logger;

        public DlqProcessor(ILogger<DlqProcessor> logger)
        {
            _logger = logger;
        }

        [Function("DlqProcessor")]
        public async Task Run(
            [ServiceBusTrigger("order-events", "payment-sub/$DeadLetterQueue", Connection = "ServiceBusConnection")]
        ServiceBusReceivedMessage message,
            ServiceBusMessageActions actions)
        {
            var correlationId = message.ApplicationProperties.ContainsKey("CorrelationId")
    ? message.ApplicationProperties["CorrelationId"]?.ToString()
    : Guid.NewGuid().ToString();

            _logger.LogInformation("CID: {cid}", correlationId);
            var body = message.Body.ToString();

            _logger.LogError("DLQ Message: {body}", body);

            _logger.LogError("DeadLetter Reason: {reason}", message.DeadLetterReason);
            _logger.LogError("DeadLetter Error: {error}", message.DeadLetterErrorDescription);

            // Option 1: just log
            await actions.CompleteMessageAsync(message);

            // Option 2 (later): resend / alert / store in DB
        }

//        inventory-sub/$DeadLetterQueue
//order-sub/$DeadLetterQueue
    }
}
