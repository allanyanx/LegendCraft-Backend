using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegendCraft_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscountPercentageToArticle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercentage",
                table: "Articles",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountPercentage",
                table: "Articles");
        }
    }
}
