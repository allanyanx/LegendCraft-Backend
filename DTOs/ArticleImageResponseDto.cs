namespace LegendCraft_Backend.DTOs
{
    public class ArticleImageResponseDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMain { get; set; }
    }
}
