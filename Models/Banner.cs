namespace LegendCraft_Backend.Models
{
    public class Banner : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int DisplayOrder { get; set; } = 0;
    }
}
