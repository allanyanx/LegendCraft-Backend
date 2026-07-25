namespace LegendCraft_Backend.DTOs
{
    public class ArticleListResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string MainImageUrl { get; set; } = string.Empty;
    }
}
