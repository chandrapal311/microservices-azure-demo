using RestEase;
using Shared.Contracts.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace order_processor_function.Clients
{
    public interface IOrderApi
    {
        [Put("api/order/{orderId}/status")]
        Task<ApiResponse<string>> UpdateStatusAsync(
            [Path] int orderId,
            [Query] string status,
            [Header("Authorization")] string authorization,
            [Header("x-correlation-id")] string correlationId);
    }
}
