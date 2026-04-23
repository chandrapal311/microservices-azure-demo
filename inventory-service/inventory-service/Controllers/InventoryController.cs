using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace inventory_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {

        [HttpPost]
        public IActionResult Update()
        {
            return Ok("Inventory updated");
        }
    }
}
