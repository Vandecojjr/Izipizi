using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Comandas.Migrations
{
    /// <inheritdoc />
    public partial class FKVendasCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ProdutosVendidos_VendaId",
                table: "ProdutosVendidos",
                column: "VendaId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProdutosVendidos_Vendas_VendaId",
                table: "ProdutosVendidos",
                column: "VendaId",
                principalTable: "Vendas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProdutosVendidos_Vendas_VendaId",
                table: "ProdutosVendidos");

            migrationBuilder.DropIndex(
                name: "IX_ProdutosVendidos_VendaId",
                table: "ProdutosVendidos");
        }
    }
}
