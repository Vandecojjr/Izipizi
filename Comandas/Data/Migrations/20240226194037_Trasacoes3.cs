using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Comandas.Migrations
{
    /// <inheritdoc />
    public partial class Trasacoes3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CaixaId",
                table: "transacoes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_transacoes_CaixaId",
                table: "transacoes",
                column: "CaixaId");

            migrationBuilder.AddForeignKey(
                name: "FK_transacoes_Caixas_CaixaId",
                table: "transacoes",
                column: "CaixaId",
                principalTable: "Caixas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_transacoes_Caixas_CaixaId",
                table: "transacoes");

            migrationBuilder.DropIndex(
                name: "IX_transacoes_CaixaId",
                table: "transacoes");

            migrationBuilder.DropColumn(
                name: "CaixaId",
                table: "transacoes");
        }
    }
}
