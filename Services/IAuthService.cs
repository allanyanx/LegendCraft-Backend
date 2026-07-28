using LegendCraft_Backend.DTOs;
using Microsoft.AspNetCore.Identity;

namespace LegendCraft_Backend.Services
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
        Task<bool> MakeAdminAsync(string email);
        Task<(IdentityResult Result, AuthResponseDto? NewToken)> UpdateProfileAsync(string userId, UpdateProfileDto dto);
        Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordDto dto);
        Task<UserProfileDto?> GetProfileAsync(string userId);
    }
}
