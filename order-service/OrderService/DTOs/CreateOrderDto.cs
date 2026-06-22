namespace OrderService.DTOs
{
    public class CreateOrderDto
    {
        public int ProductId { get; set; }
        public decimal Amount { get; set; }
    }
}
