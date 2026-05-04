using Azure.Messaging.ServiceBus;

namespace OrderService.Messaging
{
    //public interface IMessagePublisher
    //{
    //    Task PublishAsync(string message);
    //}

    public interface IMessagePublisher
    {
        Task PublishAsync(string topicName, ServiceBusMessage message);
    }
}
