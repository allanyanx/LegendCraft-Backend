using LegendCraft_Backend.Data;
using LegendCraft_Backend.DTOs;
using LegendCraft_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace LegendCraft_Backend.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OrderResponseDto> CreateOrderAsync(OrderCreateDto createDto, string identifier, bool isGuest)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                Cart? cart = null;
                if (isGuest)
                {
                    cart = await _context.Carts
                        .Include(c => c.Items).ThenInclude(i => i.Article)
                        .FirstOrDefaultAsync(c => c.GuestId == identifier && c.IsActive);
                }
                else
                {
                    cart = await _context.Carts
                        .Include(c => c.Items).ThenInclude(i => i.Article)
                        .FirstOrDefaultAsync(c => c.UserId == identifier && c.IsActive);
                }

                if (cart == null || !cart.Items.Any())
                {
                    throw new Exception("El carrito está vacío o no existe.");
                }

                var fullAddress = $"{createDto.ShippingAddress}, {createDto.City}, {createDto.Zip}";

                var order = new Order
                {
                    UserId = isGuest ? null : identifier,
                    GuestEmail = createDto.GuestEmail,
                    GuestFirstName = createDto.GuestFirstName,
                    GuestLastName = createDto.GuestLastName,
                    ShippingAddress = fullAddress,
                    ContactPhone = createDto.ContactPhone,
                    PaymentMethod = createDto.PaymentMethod,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                    TrackingNumber = Guid.NewGuid()
                };

                decimal totalAmount = 0;

                foreach (var item in cart.Items)
                {
                    var article = item.Article;

                    if (!article.IsPrintOnDemand && article.Stock < item.Quantity)
                    {
                        throw new Exception($"Stock insuficiente para el artículo: {article.Name}");
                    }

                    if (article.Stock >= item.Quantity)
                    {
                        article.Stock -= item.Quantity;
                    }
                    else if (article.Stock > 0)
                    {
                        article.Stock = 0;
                    }

                    var orderItem = new OrderItem
                    {
                        ArticleId = article.Id,
                        Quantity = item.Quantity,
                        UnitPrice = article.Price 
                    };

                    order.OrderItems.Add(orderItem);
                    totalAmount += orderItem.UnitPrice * orderItem.Quantity;
                }

                order.TotalAmount = totalAmount;

                _context.Orders.Add(order);
                _context.CartItems.RemoveRange(cart.Items);
                await _context.SaveChangesAsync();
                
                await transaction.CommitAsync();

                return await GetOrderResponseDtoAsync(order.Id);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<OrderResponseDto?> GetOrderByIdAsync(int id, string? userId)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Article)
                .AsQueryable();

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(o => o.UserId == userId);
            }

            var order = await query.FirstOrDefaultAsync(o => o.Id == id);
            
            if (order == null) return null;

            return MapToResponseDto(order);
        }

        public async Task<OrderResponseDto?> GetOrderByTrackingNumberAsync(Guid trackingNumber)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Article)
                .FirstOrDefaultAsync(o => o.TrackingNumber == trackingNumber);

            if (order == null) return null;

            return MapToResponseDto(order);
        }

        public async Task<List<OrderResponseDto>> GetUserOrdersAsync(string userId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Article)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return orders.Select(MapToResponseDto).ToList();
        }

        public async Task<List<OrderResponseDto>> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Article)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return orders.Select(MapToResponseDto).ToList();
        }

        public async Task<bool> UpdateOrderStatusAsync(int id, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return false;

            order.Status = status;
            await _context.SaveChangesAsync();

            return true;
        }

        // Helpers privados
        private async Task<OrderResponseDto> GetOrderResponseDtoAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Article)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            return MapToResponseDto(order!);
        }

        private OrderResponseDto MapToResponseDto(Order order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                UserId = order.UserId,
                GuestEmail = order.GuestEmail,
                GuestFirstName = order.GuestFirstName,
                GuestLastName = order.GuestLastName,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString(),
                ShippingAddress = order.ShippingAddress,
                ContactPhone = order.ContactPhone,
                PaymentMethod = order.PaymentMethod,
                TrackingNumber = order.TrackingNumber,
                Items = order.OrderItems.Select(oi => new OrderItemResponseDto
                {
                    ArticleId = oi.ArticleId,
                    ArticleName = oi.Article.Name,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    SubTotal = oi.Quantity * oi.UnitPrice
                }).ToList()
            };
        }
    }
}
