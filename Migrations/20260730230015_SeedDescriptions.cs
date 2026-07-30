using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegendCraft_Backend.Migrations
{
    /// <inheritdoc />
    public partial class SeedDescriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE \"Articles\" SET \"Description\" = 'Figura 3D de alta calidad, perfecta para coleccionistas. Impresa con alto nivel de detalle y acabados precisos.' WHERE \"Description\" = '';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
