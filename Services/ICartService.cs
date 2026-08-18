using LegendCraft_Backend.DTOs;
using System.Threading.Tasks;

namespace LegendCraft_Backend.Services
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(string identifier, bool isGuest = false);
        Task<CartDto> AddItemToCartAsync(string identifier, AddToCartDto dto, bool isGuest = false);
        Task<CartDto> UpdateItemQuantityAsync(string identifier, int cartItemId, UpdateCartItemDto dto, bool isGuest = false);
        Task<CartDto> RemoveItemFromCartAsync(string identifier, int cartItemId, bool isGuest = false);
        Task ClearCartAsync(string identifier, bool isGuest = false);
    }
}
