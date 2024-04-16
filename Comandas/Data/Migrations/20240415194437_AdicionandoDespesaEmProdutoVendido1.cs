using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Comandas.Migrations
{
    /// <inheritdoc />
    public partial class AdicionandoDespesaEmProdutoVendido1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Dispesacao",
                table: "ProdutosVendidos",
                newName: "Despesa");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Despesa",
                table: "ProdutosVendidos",
                newName: "Dispesacao");
        }
    }
}
