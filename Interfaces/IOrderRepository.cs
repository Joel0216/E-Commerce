using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int id);
        Task AddAsync(Order order);
        Task UpdateStatusAsync(Order order, Enums.OrderStatus newStatus);
        Task AddItemAsync(int orderId, OrderItem item);
        Task<bool> IsOrderShippedAsync(int orderId);
    }
}
