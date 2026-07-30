using LegendCraft_Backend.DTOs;
using LegendCraft_Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LegendCraft_Backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterDto dto)
        {
            var user = new ApplicationUser 
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName, 
                LastName = dto.LastName    
            };

            return await _userManager.CreateAsync(user, dto.Password);
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            // Verificamos que el usuario exista
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                return null; 
            }

            return await GenerateTokenAsync(user);
        }

        private async Task<AuthResponseDto> GenerateTokenAsync(ApplicationUser user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.GivenName, user.FirstName), 
                new Claim(ClaimTypes.Surname, user.LastName),    
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            int expireHours = Convert.ToInt32(_configuration["Jwt:ExpireHours"] ?? "2");

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.UtcNow.AddHours(expireHours),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Email = user.Email!,
                FirstName = user.FirstName, 
                LastName = user.LastName,
                Expiration = token.ValidTo,
                Roles = userRoles,
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
            };
        }

        public async Task<bool> MakeAdminAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return false;

            // Verificamos si el rol 'Admin' existe en la BD
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Asignamos el rol al usuario
            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }

            return true;
        }

        public async Task<(IdentityResult Result, AuthResponseDto? NewToken)> UpdateProfileAsync(string userId, UpdateProfileDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return (IdentityResult.Failed(new IdentityError { Description = "Usuario no encontrado" }), null);

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return (result, null);

            var newToken = await GenerateTokenAsync(user);
            return (result, newToken);
        }

        public async Task<UserProfileDto?> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            return new UserProfileDto
            {
                Email = user.Email ?? "",
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        public async Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "Usuario no encontrado" });

            return await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
        }

        public async Task<AuthResponseDto?> RefreshTokenAsync(RefreshTokenDto dto)
        {
            var principal = GetPrincipalFromExpiredToken(dto.Token);
            if (principal == null) return null;

            var email = principal.FindFirstValue(ClaimTypes.Email);
            if (email == null) return null;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || user.RefreshToken != dto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }

            return await GenerateTokenAsync(user);
        }

        private async Task<string> GenerateAndSaveRefreshTokenAsync(ApplicationUser user)
        {
            var refreshToken = Guid.NewGuid().ToString("N");
            user.RefreshToken = refreshToken;
            // El Refresh Token durará 30 días
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);
            await _userManager.UpdateAsync(user);
            return refreshToken;
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
                ValidateLifetime = false // Ignoramos la expiración porque sabemos que ya expiró
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
                var jwtSecurityToken = securityToken as JwtSecurityToken;
                
                if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
