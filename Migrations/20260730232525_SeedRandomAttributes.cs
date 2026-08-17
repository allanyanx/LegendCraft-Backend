using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegendCraft_Backend.Migrations
{
    /// <inheritdoc />
    public partial class SeedRandomAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO ""ArticleAttributes"" (""ArticleId"", ""AttributeValueId"")
                SELECT a.""Id"", v.""Id""
                FROM ""Articles"" a
                CROSS JOIN LATERAL (
                    SELECT ""Id"" 
                    FROM ""AttributeValues"" 
                    ORDER BY RANDOM() 
                    LIMIT 2
                ) v
                ON CONFLICT DO NOTHING;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
