namespace LegendCraft_Backend.Models
{
    public class AttributeType : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Ej: "Franquicia"

        public ICollection<AttributeValue> Values { get; set; } = new List<AttributeValue>();
    }
}
