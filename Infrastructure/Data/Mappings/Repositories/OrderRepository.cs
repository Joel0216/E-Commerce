using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task AddAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(Order order, OrderStatus newStatus)
        {
            order.Status = newStatus;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task AddItemAsync(int orderId, OrderItem item)
        {
            item.OrderId = orderId;
            _context.OrderItems.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsOrderShippedAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            return order != null && order.Status == OrderStatus.Enviado;
        }
    }
}
