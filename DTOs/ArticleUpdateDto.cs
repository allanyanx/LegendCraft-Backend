namespace LegendCraft_Backend.DTOs
{
    public class ArticleUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool IsPrintOnDemand { get; set; }
        public bool RequiresQuote { get; set; }
        public int PrintTimeDays { get; set; }

        public List<string> Highlights { get; set; } = new();
        public List<int> AttributeValueIds { get; set; } = new();
    }
}
