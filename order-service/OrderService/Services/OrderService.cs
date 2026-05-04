using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using OrderService.Controllers;
using OrderService.DTOs;
using OrderService.Enums;
using OrderService.Messaging;
using OrderService.Models;
using Shared.Contracts;
using System.Text.Json;

namespace OrderService.Services
{
    public class OrderService : IOrderService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly OrderDbContext _context;
        private readonly IMessagePublisher _publisher;
        private readonly IConfiguration _config;

        public OrderService(OrderDbContext context, IMessagePublisher publisher, ILogger<OrderService> logger, IConfiguration config)
        {
            _context = context;
            _publisher = publisher;
            _logger = logger;
            _config = config;
        }

        public async Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto)
        {
            _logger.LogInformation("Creating new order");
            var order = new Order
            {
                ProductId = dto.ProductId,
                Amount = dto.Amount
            };

            _context.Orders.Add(order);
            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Order saved to DB: {OrderId}", order.Id);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception("Order was modified by another process");
            }


            _logger.LogInformation("Sending message to Service Bus Topic");
            var orderEvent = new OrderCreatedEvent
            {
                OrderId = order.Id,
                Amount = order.Amount,
                ProductName = dto.ProductId.ToString(),
                CorrelationId = Guid.NewGuid().ToString()
            };

            //await _publisher.PublishAsync(JsonSerializer.Serialize(orderEvent));

            var topicName = _config["ServiceBus:TopicName"]??string.Empty; 

            _logger.LogInformation("Using Service Bus: {Conn}",
    _config["ServiceBus:ConnectionString"]);
            var message = new ServiceBusMessage(JsonSerializer.Serialize(orderEvent));
            //message.ApplicationProperties["CorrelationId"] = orderEvent.CorrelationId;
            message.ApplicationProperties["eventType"] = "OrderCreated";
            message.ApplicationProperties["CorrelationId"] = orderEvent.CorrelationId;
            try
            {
                await _publisher.PublishAsync(topicName, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message");
                throw;
            }
           
            _logger.LogInformation("Message sent to Service Bus Topic");
            return new OrderResponseDto
            {
                Id = order.Id,
                ProductId = order.ProductId,
                Amount = order.Amount,
                Status = order.Status
            };
        }

        public List<OrderResponseDto> GetAll()
        {
            return _context.Orders.Select(o => new OrderResponseDto
            {
                Id = o.Id,
                ProductId = o.ProductId,
                Amount = o.Amount,
                Status = o.Status
            }).ToList();
        }

        public async Task<bool> UpdateStatus(int id, OrderStatus orderStatus)
        {
            int rowsAffected = await _context.Orders
        .Where(o => o.Id == id)
        .ExecuteUpdateAsync(setters => setters
            .SetProperty(o => o.Status, orderStatus.ToString()));

            return rowsAffected > 0;
        }
    }
}
