using Application.DTOs;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Application.Exceptions;

namespace Application.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task AddOrderAsync(CreateOrderDto dto)
        {
            var order = new Order
            {
                Status = OrderStatus.Pendiente,
                OrderItems = new List<OrderItem>()
            };

            foreach (var item in dto.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId)
                    ?? throw new BusinessException($"Producto con ID {item.ProductId} no encontrado.");

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });
            }

            await _orderRepository.AddAsync(order);
        }

        public async Task AddItemAsync(int orderId, OrderItemDto dto)
        {
            var order = await _orderRepository.GetByIdAsync(orderId)
                ?? throw new BusinessException("Orden no encontrada.");

            if (order.Status == OrderStatus.Enviado)
                throw new BusinessException("No se puede modificar una orden ya enviada.");

            var item = new OrderItem
            {
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice
            };

            await _orderRepository.AddItemAsync(orderId, item);
        }

        public async Task UpdateStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _orderRepository.GetByIdAsync(orderId)
                ?? throw new BusinessException("Orden no encontrada.");

            await _orderRepository.UpdateStatusAsync(order, newStatus);
        }
    }
}
