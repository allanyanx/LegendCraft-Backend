namespace LegendCraft_Backend.Models
{
    public class ArticleAttributeValue
    {
        public int ArticleId { get; set; }
        public Article Article { get; set; } = null!;

        public int AttributeValueId { get; set; }
        public AttributeValue AttributeValue { get; set; } = null!;
    }
}
