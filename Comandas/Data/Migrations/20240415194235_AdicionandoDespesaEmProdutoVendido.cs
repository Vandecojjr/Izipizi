using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Comandas.Migrations
{
    /// <inheritdoc />
    public partial class AdicionandoDespesaEmProdutoVendido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Dispesacao",
                table: "ProdutosVendidos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dispesacao",
                table: "ProdutosVendidos");
        }
    }
}
