namespace LegendCraft_Backend.DTOs
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public string ArticleName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal => Price * Quantity;
    }
}
