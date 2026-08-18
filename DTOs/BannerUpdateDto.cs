using System.ComponentModel.DataAnnotations;

namespace LegendCraft_Backend.DTOs
{
    public class BannerUpdateDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}
