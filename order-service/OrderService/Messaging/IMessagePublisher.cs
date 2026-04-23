namespace OrderService.Messaging
{
    public interface IMessagePublisher
    {
        Task PublishAsync(string message);
    }
}
