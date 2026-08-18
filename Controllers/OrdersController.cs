using LegendCraft_Backend.DTOs;
using LegendCraft_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LegendCraft_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private (string Id, bool IsGuest) GetIdentifier()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId)) return (userId, false);
            if (Request.Headers.TryGetValue("X-Guest-Id", out var guestId)) return (guestId.ToString(), true);
            return (string.Empty, false);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto createDto)
        {
            try
            {
                var (id, isGuest) = GetIdentifier();
                if (string.IsNullOrEmpty(id)) return BadRequest(new { message = "Identificador no válido." });

                if (!isGuest)
                {
                    createDto.GuestEmail ??= User.FindFirstValue(ClaimTypes.Email);
                    createDto.GuestFirstName ??= User.FindFirstValue(ClaimTypes.GivenName);
                    createDto.GuestLastName ??= User.FindFirstValue(ClaimTypes.Surname);
                }

                var result = await _orderService.CreateOrderAsync(createDto, id, isGuest);
                return CreatedAtAction(nameof(GetOrderById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _orderService.GetOrderByIdAsync(id, userId);

            if (order == null)
            {
                return NotFound(new { message = "Orden no encontrada o no te pertenece." });
            }

            return Ok(order);
        }

        [HttpGet("track/{trackingNumber}")]
        [AllowAnonymous]
        public async Task<IActionResult> TrackOrder(Guid trackingNumber)
        {
            var order = await _orderService.GetOrderByTrackingNumberAsync(trackingNumber);

            if (order == null)
            {
                return NotFound(new { message = "Orden no encontrada." });
            }

            return Ok(order);
        }

        [HttpGet("my-orders")]
        [Authorize]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var orders = await _orderService.GetUserOrdersAsync(userId);
            return Ok(orders);
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] LegendCraft_Backend.Models.OrderStatus status)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, status);
            if (!result)
            {
                return NotFound(new { message = "Orden no encontrada." });
            }

            return Ok(new { message = "Estado actualizado con éxito." });
        }
    }
}
