using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts;
using Shared.Contracts.Responses;

namespace inventory_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {

        [HttpPost]
        [Authorize(Roles = "Service")]
        public IActionResult Update([FromBody] OrderCreatedEvent order)
        {
            var correlationId = Request.Headers["x-correlation-id"];
            
            return Ok(new ApiResponse<string>
            {
                Success = true,
                StatusCode = 200,
                Message = $"Inventory Updated OrderId:{order.OrderId} x-correlation-id:{correlationId}",
                Data = "Inventory Updated"
            });
        }
    }
}
