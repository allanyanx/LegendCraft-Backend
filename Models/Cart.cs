namespace LegendCraft_Backend.Models
{
    public class Cart : BaseEntity
    {
        public string? UserId { get; set; } // Nullable, por si quieres soportar carritos anónimos
        public ApplicationUser? User { get; set; }
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
