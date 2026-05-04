using Azure.Messaging.ServiceBus;

namespace OrderService.Messaging
{
    public class ServiceBusPublisher : IMessagePublisher
    {
        private readonly IConfiguration _config;
        private readonly ServiceBusClient _client;

        public ServiceBusPublisher(IConfiguration config, ServiceBusClient client)
        {
            _client = client;
            _config = config;
        }

        //public async Task PublishAsync(string message)
        //{
        //    var connectionString = _config["ServiceBus:ConnectionString"];
        //    var queueName = _config["ServiceBus:QueueName"];

        //    await using var client = new ServiceBusClient(connectionString);
        //    var sender = client.CreateSender(queueName);

        //    var serviceBusMessage = new ServiceBusMessage(message);

        //    await sender.SendMessageAsync(serviceBusMessage);
        //}

        public async Task PublishAsync(string topicName, ServiceBusMessage message)
        {
            
           
            try
            {
                await using var sender = _client.CreateSender(topicName);

                await sender.SendMessageAsync(message);
                Console.WriteLine("Message SENT successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                throw;
            }
        }
    }
}
