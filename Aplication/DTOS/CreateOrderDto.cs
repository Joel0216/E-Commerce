using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "La lista de productos es obligatoria")]
        [MinLength(1, ErrorMessage = "Debe incluir al menos un producto")]
        public List<OrderItemDto> Items { get; set; } = new();
    }
}