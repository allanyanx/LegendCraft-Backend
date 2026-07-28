using LegendCraft_Backend.DTOs;
using LegendCraft_Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LegendCraft_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);

            if (!result.Succeeded)
            {
                // Si falla, extraemos los errores y devolvemos un 400 Bad Request
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { Message = "Error al registrar el usuario", Errors = errors });
            }

            return StatusCode(201, new { Message = "Usuario registrado con éxito" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var response = await _authService.LoginAsync(dto);

            if (response == null)
            {
                // 401 Unauthorized para credenciales inválidas
                return Unauthorized(new { Message = "Correo o contraseña incorrectos" });
            }

            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            var response = await _authService.RefreshTokenAsync(dto);

            if (response == null)
            {
                return Unauthorized(new { Message = "Token inválido o expirado. Inicia sesión nuevamente." });
            }

            return Ok(response);
        }

        [HttpGet("me")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var profile = await _authService.GetProfileAsync(userId);
            if (profile == null) return NotFound();

            return Ok(profile);
        }

        [HttpPost("make-admin/{email}")]
        public async Task<IActionResult> MakeAdmin(string email)
        {
            var success = await _authService.MakeAdminAsync(email);
            if (!success)
            {
                return NotFound(new { Message = "Usuario no encontrado." });
            }

            return Ok(new { Message = $"El usuario {email} ahora tiene rol de Administrador." });
        }

        [HttpPut("update-profile")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var (result, newToken) = await _authService.UpdateProfileAsync(userId, dto);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { Message = "Error al actualizar perfil", Errors = errors });
            }

            return Ok(new { Message = "Perfil actualizado con éxito", Token = newToken });
        }

        [HttpPut("change-password")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var result = await _authService.ChangePasswordAsync(userId, dto);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { Message = "Error al cambiar la contraseña", Errors = errors });
            }

            return Ok(new { Message = "Contraseña cambiada con éxito" });
        }
    }
}
