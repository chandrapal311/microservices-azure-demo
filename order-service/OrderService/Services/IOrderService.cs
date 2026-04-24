using OrderService.DTOs;
using OrderService.Enums;

namespace OrderService.Services
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto);
        List<OrderResponseDto> GetAll();

        Task<bool> UpdateStatus(int id, OrderStatus orderStatus);

    }
}
