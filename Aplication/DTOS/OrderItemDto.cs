using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class OrderItemDto
    {
        [Required(ErrorMessage = "El ID del producto es obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del producto debe ser mayor a 0")]
        public int ProductId { get; set; }
        
        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public int Quantity { get; set; }
    }
}
