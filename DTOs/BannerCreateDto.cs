using System.ComponentModel.DataAnnotations;

namespace LegendCraft_Backend.DTOs
{
    public class BannerCreateDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public int DisplayOrder { get; set; } = 0;
    }
}
