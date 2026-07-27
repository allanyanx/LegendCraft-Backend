namespace LegendCraft_Backend.DTOs
{
    public class UserProfileDto
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        
        // Si más adelante se agrega dirección de facturación, número de teléfono, etc., irán aquí.
    }
}
