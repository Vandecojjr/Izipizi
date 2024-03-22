using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Comandas.Migrations
{
    /// <inheritdoc />
    public partial class FechamentoDECaixa1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FechamentoCaixa",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NomeMetodo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdMetodo = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ValorApurado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorInformado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CaixaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FechamentoCaixa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FechamentoCaixa_Caixas_CaixaId",
                        column: x => x.CaixaId,
                        principalTable: "Caixas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FechamentoCaixa_CaixaId",
                table: "FechamentoCaixa",
                column: "CaixaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FechamentoCaixa");
        }
    }
}
