using Application.DTOs;
using Domain.Enums;

namespace Application.Services.Interfaces
{
    public interface IOrderService
    {
        Task AddOrderAsync(CreateOrderDto dto);
        Task AddItemAsync(int orderId, OrderItemDto dto);
        Task UpdateStatusAsync(int orderId, OrderStatus newStatus);
    }
}
