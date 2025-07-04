using Domain.Entities;
using Domain.Enums;
using Application.DTOs;

namespace Application.Services.Interfaces
{
    public interface IOrderService
    {
        Task AddOrderAsync(CreateOrderDto dto);
        Task AddItemAsync(int orderId, OrderItemDto dto);
        Task UpdateStatusAsync(int orderId, OrderStatus newStatus);
        Task<IEnumerable<OrderResponseDto>> GetAllAsync(); // <-- optimizado para DTO plano
    }
}