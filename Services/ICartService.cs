using LegendCraft_Backend.DTOs;
using System.Threading.Tasks;

namespace LegendCraft_Backend.Services
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(string userId);
        Task<CartDto> AddItemToCartAsync(string userId, AddToCartDto dto);
        Task<CartDto> UpdateItemQuantityAsync(string userId, int cartItemId, UpdateCartItemDto dto);
        Task<CartDto> RemoveItemFromCartAsync(string userId, int cartItemId);
        Task ClearCartAsync(string userId);
    }
}
