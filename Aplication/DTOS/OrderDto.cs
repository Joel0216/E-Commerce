using Domain.Enums;

namespace Application.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
