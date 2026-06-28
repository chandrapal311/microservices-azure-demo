using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using payment_service.Services;
using Shared.Contracts;

namespace payment_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {


        private readonly IPaymentService _service;

        public PaymentController(IPaymentService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Roles = "Service")]
        public async Task<IActionResult> Process([FromBody] OrderCreatedEvent order)
        {
            var result = await _service.ProcessAsync(order);
            return Ok(result);
        }

        [HttpPost("refund")]
        [Authorize(Roles = "Service")]
        public async Task<IActionResult> Refund([FromBody] OrderCreatedEvent order)
        {
            var result = await _service.RefundAsync(order);
            return Ok(result);
        }
    }
}
