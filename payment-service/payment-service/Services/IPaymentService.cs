using Shared.Contracts;

namespace payment_service.Services
{
    public interface IPaymentService
    {

        Task<string> ProcessAsync(OrderCreatedEvent order);
        Task<string> RefundAsync(OrderCreatedEvent order);
    }
}
