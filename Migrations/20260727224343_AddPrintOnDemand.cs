using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegendCraft_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPrintOnDemand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPrintOnDemand",
                table: "Articles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PrintTimeDays",
                table: "Articles",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrintOnDemand",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "PrintTimeDays",
                table: "Articles");
        }
    }
}
