using Microsoft.EntityFrameworkCore;
using payment_service.Models;

namespace payment_service.Data
{
    public class PaymentDbContext: DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options)
        : base(options) { }

        public DbSet<ProcessedOrder> ProcessedOrders { get; set; }
    }
}
