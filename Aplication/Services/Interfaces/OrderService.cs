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
            try
            {
                // Validar que no haya productos duplicados
                var productIds = dto.Items.Select(i => i.ProductId).ToList();
                var duplicateIds = productIds.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
                
                if (duplicateIds.Any())
                    throw new BusinessException($"Productos duplicados en la orden: {string.Join(", ", duplicateIds)}");

                var order = new Order
                {
                    Status = OrderStatus.Pending,
                    OrderItems = new List<OrderItem>()
                };

                foreach (var item in dto.Items)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId)
                        ?? throw new BusinessException($"Producto con ID {item.ProductId} no encontrado.");

                    // Validar stock disponible
                    if (product.Stock < item.Quantity)
                        throw new BusinessException($"Stock insuficiente para el producto '{product.Name}'. Disponible: {product.Stock}, Solicitado: {item.Quantity}");

                    // Validar que la cantidad sea mayor a 0
                    if (item.Quantity <= 0)
                        throw new BusinessException($"La cantidad del producto '{product.Name}' debe ser mayor a 0");

                    order.OrderItems.Add(new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price
                    });

                    // Actualizar stock del producto
                    product.Stock -= item.Quantity;
                    await _productRepository.UpdateAsync(product);
                }

                await _orderRepository.AddAsync(order);
            }
            catch (Exception ex)
            {
                // Log del error para debugging
                Console.WriteLine($"❌ Error al crear orden: {ex.Message}");
                Console.WriteLine($"📍 Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task AddItemAsync(int orderId, OrderItemDto dto)
        {
            var order = await _orderRepository.GetByIdAsync(orderId)
                ?? throw new BusinessException("Orden no encontrada.");

            if (order.Status == OrderStatus.Shipped)
                throw new BusinessException("No se puede modificar una orden ya enviada.");

            var product = await _productRepository.GetByIdAsync(dto.ProductId)
                ?? throw new BusinessException("Producto no encontrado.");

            // Validar stock disponible
            if (product.Stock < dto.Quantity)
                throw new BusinessException($"Stock insuficiente para el producto '{product.Name}'. Disponible: {product.Stock}, Solicitado: {dto.Quantity}");

            // Validar que la cantidad sea mayor a 0
            if (dto.Quantity <= 0)
                throw new BusinessException($"La cantidad del producto '{product.Name}' debe ser mayor a 0");

            var item = new OrderItem
            {
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                UnitPrice = product.Price
            };

            // Actualizar stock del producto
            product.Stock -= dto.Quantity;
            await _productRepository.UpdateAsync(product);

            await _orderRepository.AddItemAsync(orderId, item);
        }

        public async Task UpdateStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _orderRepository.GetByIdAsync(orderId)
                ?? throw new BusinessException("Orden no encontrada.");

            await _orderRepository.UpdateStatusAsync(order, newStatus);
        }

        public async Task<IEnumerable<OrderResponseDto>> GetAllAsync()
        {
            var orders = await _orderRepository.GetAllWithItemsAsync();
            return orders.Select(order => new OrderResponseDto
            {
                Id = order.Id,
                Status = order.Status.ToString(),
                Items = order.OrderItems.Select(item => new OrderItemResponseDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.Product?.Name ?? string.Empty,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity
                }).ToList()
            });
        }
    }
}
