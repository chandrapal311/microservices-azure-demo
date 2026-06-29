using RestEase;
using Shared.Contracts;
using Shared.Contracts.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace order_processor_function.Clients
{
    public interface IPaymentApi
    {
        [Post("api/payment")]
        Task<ApiResponse<string>> ProcessAsync(
            [Header("Authorization")] string authorization,
            [Header("x-correlation-id")] string correlationId,
            [Body] OrderCreatedEvent order);

        [Post("api/payment/refund")]
        Task<ApiResponse<string>> RefundAsync(
            [Header("Authorization")] string authorization,
            [Header("x-correlation-id")] string correlationId,
            [Body] OrderCreatedEvent order);
    }
}
