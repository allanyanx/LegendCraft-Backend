namespace LegendCraft_Backend.DTOs
{
    public class ArticleDetailResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public bool IsPrintOnDemand { get; set; }
        public int PrintTimeDays { get; set; }

        public List<string> Highlights { get; set; } = new();

        public Dictionary<string, string> Attributes { get; set; } = new();

        public List<ArticleImageResponseDto> Images { get; set; } = new();
    }
}
