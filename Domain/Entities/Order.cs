using Domain.Enums;

namespace Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pendiente;

        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}
