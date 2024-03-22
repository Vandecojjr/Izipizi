using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Comandas.Migrations
{
    /// <inheritdoc />
    public partial class Fechamento2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FechamentoCaixa_Caixas_CaixaId",
                table: "FechamentoCaixa");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FechamentoCaixa",
                table: "FechamentoCaixa");

            migrationBuilder.RenameTable(
                name: "FechamentoCaixa",
                newName: "fechamentoCaixas");

            migrationBuilder.RenameIndex(
                name: "IX_FechamentoCaixa_CaixaId",
                table: "fechamentoCaixas",
                newName: "IX_fechamentoCaixas_CaixaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_fechamentoCaixas",
                table: "fechamentoCaixas",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_fechamentoCaixas_Caixas_CaixaId",
                table: "fechamentoCaixas",
                column: "CaixaId",
                principalTable: "Caixas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_fechamentoCaixas_Caixas_CaixaId",
                table: "fechamentoCaixas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_fechamentoCaixas",
                table: "fechamentoCaixas");

            migrationBuilder.RenameTable(
                name: "fechamentoCaixas",
                newName: "FechamentoCaixa");

            migrationBuilder.RenameIndex(
                name: "IX_fechamentoCaixas_CaixaId",
                table: "FechamentoCaixa",
                newName: "IX_FechamentoCaixa_CaixaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FechamentoCaixa",
                table: "FechamentoCaixa",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FechamentoCaixa_Caixas_CaixaId",
                table: "FechamentoCaixa",
                column: "CaixaId",
                principalTable: "Caixas",
                principalColumn: "Id");
        }
    }
}
