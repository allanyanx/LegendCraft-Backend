using System.ComponentModel.DataAnnotations;

namespace LegendCraft_Backend.DTOs
{
    public class FaqCreateDto
    {
        [Required]
        public string Question { get; set; } = string.Empty;
        
        [Required]
        public string Answer { get; set; } = string.Empty;
        
        public int DisplayOrder { get; set; } = 0;
    }
}
