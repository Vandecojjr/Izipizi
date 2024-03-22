using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Comandas.Migrations
{
    /// <inheritdoc />
    public partial class FormasDePagamento2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NomeDoMetodo",
                table: "FormaDePagamento",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_FormaDePagamento_VendaId",
                table: "FormaDePagamento",
                column: "VendaId");

            migrationBuilder.AddForeignKey(
                name: "FK_FormaDePagamento_Vendas_VendaId",
                table: "FormaDePagamento",
                column: "VendaId",
                principalTable: "Vendas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormaDePagamento_Vendas_VendaId",
                table: "FormaDePagamento");

            migrationBuilder.DropIndex(
                name: "IX_FormaDePagamento_VendaId",
                table: "FormaDePagamento");

            migrationBuilder.DropColumn(
                name: "NomeDoMetodo",
                table: "FormaDePagamento");
        }
    }
}
