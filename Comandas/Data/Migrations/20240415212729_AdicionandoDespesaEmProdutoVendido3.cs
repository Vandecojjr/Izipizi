using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Comandas.Migrations
{
    /// <inheritdoc />
    public partial class AdicionandoDespesaEmProdutoVendido3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdDoUsuario",
                table: "ProdutosVendidos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdDoUsuario",
                table: "ProdutosVendidos");
        }
    }
}
