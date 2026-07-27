using LegendCraft_Backend.Data;
using LegendCraft_Backend.DTOs;
using LegendCraft_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace LegendCraft_Backend.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task<Cart> GetOrCreateCartEntityAsync(string userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Article)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        private CartDto MapToDto(Cart cart)
        {
            return new CartDto
            {
                Id = cart.Id,
                UserId = cart.UserId,
                Items = cart.Items.Where(i => i.IsActive).Select(i => new CartItemDto
                {
                    Id = i.Id,
                    ArticleId = i.ArticleId,
                    ArticleName = i.Article?.Name ?? "Desconocido",
                    Price = i.Article?.Price ?? 0m,
                    Quantity = i.Quantity
                }).ToList(),
                TotalPrice = cart.Items.Where(i => i.IsActive).Sum(i => (i.Article?.Price ?? 0m) * i.Quantity)
            };
        }

        public async Task<CartDto> GetCartAsync(string userId)
        {
            var cart = await GetOrCreateCartEntityAsync(userId);
            return MapToDto(cart);
        }

        public async Task<CartDto> AddItemToCartAsync(string userId, AddToCartDto dto)
        {
            var cart = await GetOrCreateCartEntityAsync(userId);
            var article = await _context.Articles.FindAsync(dto.ArticleId);

            if (article == null || !article.IsActive)
                throw new Exception("El artículo no existe o no está activo");

            var existingItem = cart.Items.FirstOrDefault(i => i.ArticleId == dto.ArticleId && i.IsActive);

            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
                existingItem.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ArticleId = dto.ArticleId,
                    Quantity = dto.Quantity,
                    CartId = cart.Id
                });
            }

            await _context.SaveChangesAsync();
            return MapToDto(cart);
        }

        public async Task<CartDto> UpdateItemQuantityAsync(string userId, int cartItemId, UpdateCartItemDto dto)
        {
            var cart = await GetOrCreateCartEntityAsync(userId);
            var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId && i.IsActive);

            if (item == null)
                throw new Exception("El ítem no existe en tu carrito");

            if (dto.Quantity <= 0)
            {
                item.IsActive = false;
            }
            else
            {
                item.Quantity = dto.Quantity;
                item.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return MapToDto(cart);
        }

        public async Task<CartDto> RemoveItemFromCartAsync(string userId, int cartItemId)
        {
            return await UpdateItemQuantityAsync(userId, cartItemId, new UpdateCartItemDto { Quantity = 0 });
        }

        public async Task ClearCartAsync(string userId)
        {
            var cart = await GetOrCreateCartEntityAsync(userId);
            foreach (var item in cart.Items)
            {
                item.IsActive = false;
                item.UpdatedAt = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
        }
    }
}
