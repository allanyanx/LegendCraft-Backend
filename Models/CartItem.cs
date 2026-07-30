namespace LegendCraft_Backend.Models
{
    public class CartItem : BaseEntity
    {
        public int CartId { get; set; }
        public Cart Cart { get; set; } = null!;

        public int ArticleId { get; set; }
        public Article Article { get; set; } = null!;

        public int Quantity { get; set; }
    }
}
