using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using payment_service.Services;
using Shared.Contracts;
using Shared.Contracts.Responses;

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
            return Ok(new ApiResponse<string>
            {
                Success = true,
                StatusCode = 200,
                Message = "Payment successful",
                Data = result
            });
        }

        [HttpPost("refund")]
        [Authorize(Roles = "Service")]
        public async Task<IActionResult> Refund([FromBody] OrderCreatedEvent order)
        {
            var result = await _service.RefundAsync(order);
            return Ok(new ApiResponse<string>
            {
                Success = true,
                StatusCode = 200,
                Message = "Payment successful",
                Data = result
            });
           
        }
    }
}
