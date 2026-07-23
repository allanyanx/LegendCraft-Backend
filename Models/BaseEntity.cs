namespace LegendCraft_Backend.Models
{
    // Entidad Base de auditoria y eliminación lógica
    public class BaseEntity
    {
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
