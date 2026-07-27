using LegendCraft_Backend.DTOs;
using Microsoft.AspNetCore.Identity;

namespace LegendCraft_Backend.Services
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    }
}
