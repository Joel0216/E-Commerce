namespace Domain.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int Quantity { get; set; }

        // ✅ Esta propiedad es obligatoria para evitar el error
        public decimal UnitPrice { get; set; }
    }
}
