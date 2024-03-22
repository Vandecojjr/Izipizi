using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Comandas.Migrations
{
    /// <inheritdoc />
    public partial class transacao6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "VendaId",
                table: "transacoes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_transacoes_VendaId",
                table: "transacoes",
                column: "VendaId");

            migrationBuilder.AddForeignKey(
                name: "FK_transacoes_Vendas_VendaId",
                table: "transacoes",
                column: "VendaId",
                principalTable: "Vendas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_transacoes_Vendas_VendaId",
                table: "transacoes");

            migrationBuilder.DropIndex(
                name: "IX_transacoes_VendaId",
                table: "transacoes");

            migrationBuilder.DropColumn(
                name: "VendaId",
                table: "transacoes");
        }
    }
}
