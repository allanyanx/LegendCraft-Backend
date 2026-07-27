namespace LegendCraft_Backend.DTOs
{
    public class OrderItemResponseDto
    {
        public int ArticleId { get; set; }
        public string ArticleName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
    }
}
