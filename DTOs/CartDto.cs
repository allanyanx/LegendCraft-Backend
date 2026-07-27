namespace LegendCraft_Backend.DTOs
{
    public class CartDto
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        public decimal TotalPrice { get; set; }
    }
}
