namespace LegendCraft_Backend.DTOs
{
    public class ArticleCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool IsPrintOnDemand { get; set; } = true;
        public bool RequiresQuote { get; set; } = false;
        public int PrintTimeDays { get; set; } = 3;
        
        public bool IsOnSale { get; set; }
        public decimal? DiscountPercentage { get; set; }

        public List<string> Highlights { get; set; } = new();

        public List<int> AttributeValueIds { get; set; } = new();
    }
}
