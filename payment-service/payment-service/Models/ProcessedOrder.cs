namespace payment_service.Models
{
    public class ProcessedOrder
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }
}
