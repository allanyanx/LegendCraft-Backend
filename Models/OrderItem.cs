namespace LegendCraft_Backend.Models
{
    public class OrderItem : BaseEntity
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int ArticleId { get; set; }
        public Article Article { get; set; } = null!;

        public int Quantity { get; set; }
        
        // Congelamos el precio unitario al momento de la compra
        public decimal UnitPrice { get; set; }
    }
}
