using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LegendCraft_Backend.Data
{
    // Heredamos de IdentityDbContext para integrar el sistema de usuarios desde el inicio
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        // El constructor recibe las opciones (como la cadena de conexión a PostgreSQL) 
        // y se las pasa a la clase base
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Aquí agregaremos los DbSet<T> más adelante

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Cuando usas IdentityDbContext, es OBLIGATORIO llamar al base.OnModelCreating primero
            // para que configure las tablas de seguridad (AspNetUsers, AspNetRoles, etc.)
            base.OnModelCreating(builder);

            // Aquí agregaremos nuestras configuraciones de Fluent API más adelante
        }
    }
}
