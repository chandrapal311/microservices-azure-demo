using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts;

namespace inventory_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {

        [HttpPost]
        public IActionResult Update([FromBody] OrderCreatedEvent order)
        {
            var correlationId = Request.Headers["x-correlation-id"];
            return Ok($"Inventory Updated OrderId:{order.OrderId} x-correlation-id:{correlationId}");
        }
    }
}
