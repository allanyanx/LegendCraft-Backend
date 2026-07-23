namespace LegendCraft_Backend.Models
{
    public class ArticleImage : BaseEntity
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMain { get; set; }

        public int ArticleId { get; set; }
        public Article Article { get; set; } = null!;
    }
}
