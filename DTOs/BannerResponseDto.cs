namespace LegendCraft_Backend.DTOs
{
    public class BannerResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}
