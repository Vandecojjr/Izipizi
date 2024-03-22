using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Comandas.Migrations
{
    /// <inheritdoc />
    public partial class ProdutoCusto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MargemLucro",
                table: "Produtos",
                type: "int",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomeDaCategoria",
                table: "Produtos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorDeCusto",
                table: "Produtos",
                type: "decimal(5,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MargemLucro",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "NomeDaCategoria",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "ValorDeCusto",
                table: "Produtos");
        }
    }
}
