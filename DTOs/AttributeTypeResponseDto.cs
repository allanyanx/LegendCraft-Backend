namespace LegendCraft_Backend.DTOs
{
    public class AttributeTypeResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<AttributeValueResponseDto> Values { get; set; } = new();
    }
}
