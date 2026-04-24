namespace Shared.Contracts
{
    public class OrderCreatedEvent
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string ProductName { get; set; }
    }
}
