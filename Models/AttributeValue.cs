namespace LegendCraft_Backend.Models
{
    public class AttributeValue : BaseEntity
    {
        public int Id { get; set; }
        public string Value { get; set; } = string.Empty; 

        public int AttributeTypeId { get; set; }
        public AttributeType AttributeType { get; set; } = null!;

        public ICollection<ArticleAttributeValue> ArticleAttributes { get; set; } = new List<ArticleAttributeValue>();
    }
}
