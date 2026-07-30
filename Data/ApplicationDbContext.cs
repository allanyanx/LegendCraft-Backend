using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LegendCraft_Backend.Models;

namespace LegendCraft_Backend.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // REGISTRO DE TABLAS (DbSets)
        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticleImage> ArticleImages { get; set; }
        public DbSet<ArticleHighlight> ArticleHighlights { get; set; }
        public DbSet<AttributeType> AttributeTypes { get; set; }
        public DbSet<AttributeValue> AttributeValues { get; set; }
        public DbSet<ArticleAttributeValue> ArticleAttributes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // CONFIGURACIONES DE FLUENT API

            // Clave compuesta para la tabla intermedia (Relación Artículo - Atributo)
            // Esto asume que ArticleAttributeValue junta el Id del Artículo y el Id del Atributo
            builder.Entity<ArticleAttributeValue>()
                .HasKey(aav => new { aav.ArticleId, aav.AttributeValueId });

            // Filtros Globales (Soft Delete)
            // Automáticamente ignorará los registros donde IsActive sea false en cualquier SELECT
            builder.Entity<Article>().HasQueryFilter(a => a.IsActive);
            builder.Entity<AttributeType>().HasQueryFilter(a => a.IsActive);
            builder.Entity<AttributeValue>().HasQueryFilter(a => a.IsActive);
            builder.Entity<ArticleImage>().HasQueryFilter(a => a.IsActive);
            builder.Entity<ArticleHighlight>().HasQueryFilter(a => a.IsActive);
            builder.Entity<Order>().HasQueryFilter(a => a.IsActive);
            builder.Entity<OrderItem>().HasQueryFilter(a => a.IsActive);
            builder.Entity<Cart>().HasQueryFilter(a => a.IsActive);
            builder.Entity<CartItem>().HasQueryFilter(a => a.IsActive);
        }
    }
}