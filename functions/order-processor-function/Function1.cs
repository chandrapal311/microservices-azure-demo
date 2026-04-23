using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace order_processor_function;

public class Function1
{
    private readonly ILogger<Function1> _logger;

    public Function1(ILogger<Function1> logger)
    {
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

        _logger.LogInformation("Message Body: {body}", body);
        Console.WriteLine($"Received: {body}");
        // Complete the message
        //await messageActions.CompleteMessageAsync(message);
        await Task.CompletedTask;
    }
}