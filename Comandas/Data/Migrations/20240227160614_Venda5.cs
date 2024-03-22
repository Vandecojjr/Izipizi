using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Comandas.Migrations
{
    /// <inheritdoc />
    public partial class Venda5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CaixaId",
                table: "Vendas",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_CaixaId",
                table: "Vendas",
                column: "CaixaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_Caixas_CaixaId",
                table: "Vendas",
                column: "CaixaId",
                principalTable: "Caixas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Caixas_CaixaId",
                table: "Vendas");

            migrationBuilder.DropIndex(
                name: "IX_Vendas_CaixaId",
                table: "Vendas");

            migrationBuilder.DropColumn(
                name: "CaixaId",
                table: "Vendas");
        }
    }
}
