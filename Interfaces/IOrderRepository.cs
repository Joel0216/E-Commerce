using Domain.Entities;
using Domain.Enums;

namespace Domain.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(int id);
        Task AddAsync(Order order);
        Task UpdateStatusAsync(Order order, OrderStatus newStatus);
        Task AddItemAsync(int orderId, OrderItem item);
        Task<bool> IsOrderShippedAsync(int orderId);

        // ✅ MÉTODO FALTANTE
        Task<IEnumerable<Order>> GetAllWithItemsAsync();
    }
}
