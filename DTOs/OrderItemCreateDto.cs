using System.ComponentModel.DataAnnotations;

namespace LegendCraft_Backend.DTOs
{
    public class OrderItemCreateDto
    {
        [Required]
        public int ArticleId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public int Quantity { get; set; }
    }
}
