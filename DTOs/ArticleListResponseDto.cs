namespace LegendCraft_Backend.DTOs
{
    public class ArticleListResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool IsPrintOnDemand { get; set; }
        public bool RequiresQuote { get; set; }
        public int PrintTimeDays { get; set; }
        public bool IsOnSale { get; set; }
        public decimal? DiscountPrice { get; set; }
        public decimal? DiscountPercentage { get; set; }
        
        public string MainImageUrl { get; set; } = string.Empty;
    }
}
