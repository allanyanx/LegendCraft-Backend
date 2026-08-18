namespace LegendCraft_Backend.Models
{
    public class Faq : BaseEntity
    {
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public int DisplayOrder { get; set; } = 0;
    }
}
