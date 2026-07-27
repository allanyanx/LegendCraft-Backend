namespace LegendCraft_Backend.Models
{
    public class Order : BaseEntity
    {
        // Si el usuario está registrado, guardamos su ID.
        // Si es invitado, esto será nulo.
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        // Datos del invitado (pueden venir nulos si el usuario está registrado y usamos su info)
        public string? GuestEmail { get; set; }
        public string? GuestFirstName { get; set; }
        public string? GuestLastName { get; set; }

        // Detalles de la orden
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string ShippingAddress { get; set; } = string.Empty;

        // Rastreo para usuarios invitados
        public Guid TrackingNumber { get; set; } = Guid.NewGuid();

        // Relaciones
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
