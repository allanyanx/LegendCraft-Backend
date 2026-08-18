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

        private async Task<Cart> GetOrCreateCartEntityAsync(string identifier, bool isGuest)
        {
            Cart? cart = null;

            if (isGuest)
            {
                cart = await _context.Carts
                    .Include(c => c.Items)
                    .ThenInclude(i => i.Article)
                    .ThenInclude(a => a.Images)
                    .FirstOrDefaultAsync(c => c.GuestId == identifier && c.IsActive);

                if (cart == null)
                {
                    cart = new Cart { GuestId = identifier };
                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                cart = await _context.Carts
                    .Include(c => c.Items)
                    .ThenInclude(i => i.Article)
                    .ThenInclude(a => a.Images)
                    .FirstOrDefaultAsync(c => c.UserId == identifier && c.IsActive);

                if (cart == null)
                {
                    cart = new Cart { UserId = identifier };
                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync();
                }
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
                    Quantity = i.Quantity,
                    ImageUrl = i.Article?.Images?.FirstOrDefault(img => img.IsMain)?.ImageUrl ?? i.Article?.Images?.FirstOrDefault()?.ImageUrl ?? ""
                }).ToList(),
                TotalPrice = cart.Items.Where(i => i.IsActive).Sum(i => (i.Article?.Price ?? 0m) * i.Quantity)
            };
        }

        public async Task<CartDto> GetCartAsync(string identifier, bool isGuest = false)
        {
            var cart = await GetOrCreateCartEntityAsync(identifier, isGuest);
            return MapToDto(cart);
        }

        public async Task<CartDto> AddItemToCartAsync(string identifier, AddToCartDto dto, bool isGuest = false)
        {
            var cart = await GetOrCreateCartEntityAsync(identifier, isGuest);
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

        public async Task<CartDto> UpdateItemQuantityAsync(string identifier, int cartItemId, UpdateCartItemDto dto, bool isGuest = false)
        {
            var cart = await GetOrCreateCartEntityAsync(identifier, isGuest);
            var item = cart.Items.FirstOrDefault(i => i.Id == cartItemId && i.IsActive);

            if (item == null)
                throw new Exception("El ítem no existe en tu carrito");

            if (dto.Quantity <= 0)
            {
                _context.CartItems.Remove(item);
            }
            else
            {
                item.Quantity = dto.Quantity;
                item.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return MapToDto(cart);
        }

        public async Task<CartDto> RemoveItemFromCartAsync(string identifier, int cartItemId, bool isGuest = false)
        {
            return await UpdateItemQuantityAsync(identifier, cartItemId, new UpdateCartItemDto { Quantity = 0 }, isGuest);
        }

        public async Task ClearCartAsync(string identifier, bool isGuest = false)
        {
            var cart = await GetOrCreateCartEntityAsync(identifier, isGuest);
            if (cart.Items.Any())
            {
                _context.CartItems.RemoveRange(cart.Items);
                await _context.SaveChangesAsync();
            }
        }
    }
}
