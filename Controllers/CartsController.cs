using LegendCraft_Backend.DTOs;
using LegendCraft_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LegendCraft_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartsController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private (string Id, bool IsGuest) GetIdentifier()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId)) return (userId, false);

            if (Request.Headers.TryGetValue("X-Guest-Id", out var guestId)) return (guestId.ToString(), true);

            return (string.Empty, false);
        }

        [HttpGet]
        public async Task<IActionResult> GetMyCart()
        {
            var (id, isGuest) = GetIdentifier();
            if (string.IsNullOrEmpty(id)) return BadRequest(new { Message = "No se proporcionó un identificador de usuario o invitado válido." });

            var cart = await _cartService.GetCartAsync(id, isGuest);
            return Ok(cart);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItemToCart([FromBody] AddToCartDto dto)
        {
            var (id, isGuest) = GetIdentifier();
            if (string.IsNullOrEmpty(id)) return BadRequest(new { Message = "No se proporcionó un identificador de usuario o invitado válido." });

            try
            {
                var cart = await _cartService.AddItemToCartAsync(id, dto, isGuest);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("items/{itemId}")]
        public async Task<IActionResult> UpdateItemQuantity(int itemId, [FromBody] UpdateCartItemDto dto)
        {
            var (id, isGuest) = GetIdentifier();
            if (string.IsNullOrEmpty(id)) return BadRequest(new { Message = "No se proporcionó un identificador de usuario o invitado válido." });

            try
            {
                var cart = await _cartService.UpdateItemQuantityAsync(id, itemId, dto, isGuest);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("items/{itemId}")]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            var (id, isGuest) = GetIdentifier();
            if (string.IsNullOrEmpty(id)) return BadRequest(new { Message = "No se proporcionó un identificador de usuario o invitado válido." });

            try
            {
                var cart = await _cartService.RemoveItemFromCartAsync(id, itemId, isGuest);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var (id, isGuest) = GetIdentifier();
            if (string.IsNullOrEmpty(id)) return BadRequest(new { Message = "No se proporcionó un identificador de usuario o invitado válido." });

            await _cartService.ClearCartAsync(id, isGuest);
            return Ok(new { Message = "Carrito limpiado correctamente" });
        }
    }
}
