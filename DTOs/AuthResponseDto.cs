namespace LegendCraft_Backend.DTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public DateTime Expiration { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
        public string RefreshToken { get; set; } = string.Empty;
    }
}
