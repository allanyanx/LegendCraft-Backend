namespace LegendCraft_Backend.Models
{
    public class Article : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }

        // Relaciones
        public ICollection<ArticleImage> Images { get; set; } = new List<ArticleImage>();
        public ICollection<ArticleAttributeValue> ArticleAttributes { get; set; } = new List<ArticleAttributeValue>();
        public ICollection<ArticleHighlight> Highlights { get; set; } = new List<ArticleHighlight>();
    }
}
