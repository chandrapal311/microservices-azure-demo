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
    public interface IInventoryApi
    {
        [Post("api/inventory")]
        Task<ApiResponse<string>> UpdateAsync(
            [Header("Authorization")] string authorization,
            [Header("x-correlation-id")] string correlationId,
            [Body] OrderCreatedEvent order);
    }
}
