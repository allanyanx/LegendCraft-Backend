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

        public async Task<OrderResponseDto> CreateOrderAsync(OrderCreateDto createDto, string? userId)
        {
            var order = new Order
            {
                UserId = userId,
                GuestEmail = createDto.GuestEmail,
                GuestFirstName = createDto.GuestFirstName,
                GuestLastName = createDto.GuestLastName,
                ShippingAddress = createDto.ShippingAddress,
                ContactPhone = createDto.ContactPhone,
                PaymentMethod = createDto.PaymentMethod,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                TrackingNumber = Guid.NewGuid()
            };

            decimal totalAmount = 0;

            foreach (var itemDto in createDto.Items)
            {
                var article = await _context.Articles.FindAsync(itemDto.ArticleId);
                if (article == null)
                {
                    throw new Exception($"Artículo con ID {itemDto.ArticleId} no encontrado.");
                }

                if (!article.IsPrintOnDemand && article.Stock < itemDto.Quantity)
                {
                    throw new Exception($"Stock insuficiente para el artículo: {article.Name}");
                }

                // Descontar stock físico si hay disponible
                if (article.Stock >= itemDto.Quantity)
                {
                    article.Stock -= itemDto.Quantity;
                }
                else if (article.Stock > 0)
                {
                    article.Stock = 0; // Se agota el físico, el resto se imprime bajo demanda
                }

                var orderItem = new OrderItem
                {
                    ArticleId = article.Id,
                    Quantity = itemDto.Quantity,
                    UnitPrice = article.Price // Congelar precio
                };

                order.OrderItems.Add(orderItem);
                totalAmount += orderItem.UnitPrice * orderItem.Quantity;
            }

            order.TotalAmount = totalAmount;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return await GetOrderResponseDtoAsync(order.Id);
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
