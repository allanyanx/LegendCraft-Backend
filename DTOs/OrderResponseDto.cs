using LegendCraft_Backend.Models;

namespace LegendCraft_Backend.DTOs
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? GuestEmail { get; set; }
        public string? GuestFirstName { get; set; }
        public string? GuestLastName { get; set; }

        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public Guid TrackingNumber { get; set; }

        public List<OrderItemResponseDto> Items { get; set; } = new List<OrderItemResponseDto>();
    }
}
