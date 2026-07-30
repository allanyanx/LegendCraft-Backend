using System.ComponentModel.DataAnnotations;

namespace LegendCraft_Backend.DTOs
{
    public class OrderCreateDto
    {
        public string? GuestEmail { get; set; }
        public string? GuestFirstName { get; set; }
        public string? GuestLastName { get; set; }

        [Required(ErrorMessage = "La dirección de envío es obligatoria.")]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono de contacto es obligatorio.")]
        public string ContactPhone { get; set; } = string.Empty;

        [Required(ErrorMessage = "El método de pago es obligatorio.")]
        public string PaymentMethod { get; set; } = string.Empty;

        [Required]
        [MinLength(1, ErrorMessage = "La orden debe tener al menos un artículo.")]
        public List<OrderItemCreateDto> Items { get; set; } = new List<OrderItemCreateDto>();
    }
}
