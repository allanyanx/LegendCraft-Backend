using LegendCraft_Backend.DTOs;
using LegendCraft_Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}
