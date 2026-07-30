namespace LegendCraft_Backend.DTOs
{
    public class ArticleListResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool IsPrintOnDemand { get; set; }
        public int PrintTimeDays { get; set; }
        
        public string MainImageUrl { get; set; } = string.Empty;
    }
}
