using LegendCraft_Backend.DTOs;
using LegendCraft_Backend.Models;

namespace LegendCraft_Backend.Services
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrderAsync(OrderCreateDto createDto, string identifier, bool isGuest);
        Task<OrderResponseDto?> GetOrderByIdAsync(int id, string? userId);
        Task<OrderResponseDto?> GetOrderByTrackingNumberAsync(Guid trackingNumber);
        Task<List<OrderResponseDto>> GetUserOrdersAsync(string userId);
        Task<List<OrderResponseDto>> GetAllOrdersAsync();
        Task<bool> UpdateOrderStatusAsync(int id, OrderStatus status);
    }
}
