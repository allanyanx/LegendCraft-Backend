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

            // Preparamos los datos que irán DENTRO del Token (Payload)
            // Fíjate que ahora tomamos el FirstName y LastName directamente de las propiedades del usuario
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.GivenName, user.FirstName), // Lo leemos directo de la columna
                new Claim(ClaimTypes.Surname, user.LastName),    // Lo leemos directo de la columna
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Obtenemos los roles del usuario y los inyectamos en el Token
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            // Leemos las llaves del appsettings
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            int expireHours = Convert.ToInt32(_configuration["Jwt:ExpireHours"] ?? "2");

            // Generamos el Token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.UtcNow.AddHours(expireHours), // Expiración en horas
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            // Devolvemos el DTO
            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Email = user.Email!,
                FirstName = user.FirstName, 
                Expiration = token.ValidTo
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
    }
}
