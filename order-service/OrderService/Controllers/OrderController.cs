using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using OrderService.DTOs;
using OrderService.Enums;
using OrderService.Models;
using OrderService.Services;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : Controller
    {

        private readonly IOrderService _service;
        public OrderController(IOrderService service)
        {
            _service = service;
        }

        [HttpPost]
        [EnableRateLimiting("default")]
        public async Task<IActionResult> Create(CreateOrderDto dto)
        {
            var result = await _service.CreateOrderAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        [EnableRateLimiting("default")]
        public IActionResult GetAll()
        {
            return Ok(_service.GetAll());
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Service")]
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] OrderStatus status)
        {            
            return Ok(await _service.UpdateStatus(id, status));
        }
    }
}
