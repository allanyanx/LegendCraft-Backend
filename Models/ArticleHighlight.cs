namespace LegendCraft_Backend.Models
{
    public class ArticleHighlight : BaseEntity
    {
        public string Text { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }

        public int ArticleId { get; set; }
        public Article Article { get; set; } = null!;
    }
}
