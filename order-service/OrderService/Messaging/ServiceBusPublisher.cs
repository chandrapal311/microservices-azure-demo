using Azure.Messaging.ServiceBus;

namespace OrderService.Messaging
{
    public class ServiceBusPublisher:IMessagePublisher
    {
        private readonly IConfiguration _config;

        public ServiceBusPublisher(IConfiguration config)
        {
            _config = config;
        }

        public async Task PublishAsync(string message)
        {
            var connectionString = _config["ServiceBus:ConnectionString"];
            var queueName = _config["ServiceBus:QueueName"];

            await using var client = new ServiceBusClient(connectionString);
            var sender = client.CreateSender(queueName);

            var serviceBusMessage = new ServiceBusMessage(message);

            await sender.SendMessageAsync(serviceBusMessage);
        }
    }
}
