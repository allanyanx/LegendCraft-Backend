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

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto createDto)
        {
            try
            {
                // Si el usuario está autenticado, obtenemos su ID
                string? userId = null;
                if (User.Identity?.IsAuthenticated == true)
                {
                    userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                }

                var result = await _orderService.CreateOrderAsync(createDto, userId);
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
    }
}
