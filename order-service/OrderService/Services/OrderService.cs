using OrderService.DTOs;
using OrderService.Models;

namespace OrderService.Services
{
    public class OrderService : IOrderService
    {
        private readonly OrderDbContext _context;

        public OrderService(OrderDbContext context)
        {
            _context = context;
        }

        public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto)
        {
            var order = new Order
            {
                ProductId = dto.ProductId,
                Amount = dto.Amount
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return new OrderResponseDto
            {
                Id = order.Id,
                ProductId = order.ProductId,
                Amount = order.Amount,
                Status = order.Status
            };
        }

        public List<OrderResponseDto> GetAll()
        {
            return _context.Orders.Select(o => new OrderResponseDto
            {
                Id = o.Id,
                ProductId = o.ProductId,
                Amount = o.Amount,
                Status = o.Status
            }).ToList();
        }
    }
}
