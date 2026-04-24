using Microsoft.EntityFrameworkCore;
using payment_service.Data;
using payment_service.Models;
using Shared.Contracts;

namespace payment_service.Services
{
    public class PaymentService: IPaymentService
    {
       

        private readonly PaymentDbContext _context;

        public PaymentService(PaymentDbContext context)
        {
            _context = context;
        }

        public async Task<string> ProcessAsync(OrderCreatedEvent order)
        {
            var exists = await _context.ProcessedOrders
                .AnyAsync(x => x.OrderId == order.OrderId);

            if (exists)
                return $"Already processed Order {order.OrderId}";

            _context.ProcessedOrders.Add(new ProcessedOrder
            {
                OrderId = order.OrderId
            });

            await _context.SaveChangesAsync();

            return $"Payment done for Order {order.OrderId}";
        }

        public async Task<string> RefundAsync(OrderCreatedEvent order)
        {
            return $"Refund processed for Order {order.OrderId}";
        }
    }
}
