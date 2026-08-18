using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LegendCraft_Backend.Migrations
{
    /// <inheritdoc />
    public partial class SeedCategoriasSql : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$
                DECLARE 
                    categoria_id int;
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM ""AttributeTypes"" WHERE ""Name"" = 'Categoría') THEN
                        INSERT INTO ""AttributeTypes"" (""Name"", ""IsActive"", ""CreatedAt"") 
                        VALUES ('Categoría', true, NOW())
                        RETURNING ""Id"" INTO categoria_id;

                        INSERT INTO ""AttributeValues"" (""AttributeTypeId"", ""Value"", ""IsActive"", ""CreatedAt"") VALUES 
                        (categoria_id, 'Figuras', true, NOW()),
                        (categoria_id, 'Cosplay', true, NOW()),
                        (categoria_id, 'Collares', true, NOW()),
                        (categoria_id, 'Mascaras', true, NOW());
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$
                DECLARE 
                    categoria_id int;
                BEGIN
                    SELECT ""Id"" INTO categoria_id FROM ""AttributeTypes"" WHERE ""Name"" = 'Categoría' LIMIT 1;
                    IF categoria_id IS NOT NULL THEN
                        DELETE FROM ""AttributeValues"" WHERE ""AttributeTypeId"" = categoria_id;
                        DELETE FROM ""AttributeTypes"" WHERE ""Id"" = categoria_id;
                    END IF;
                END $$;
            ");
        }
    }
}
