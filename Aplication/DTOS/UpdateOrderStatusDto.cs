using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class UpdateOrderStatusDto
    {
        [Required(ErrorMessage = "El status es obligatorio")]
        public string Status { get; set; } = string.Empty;
    }
} 