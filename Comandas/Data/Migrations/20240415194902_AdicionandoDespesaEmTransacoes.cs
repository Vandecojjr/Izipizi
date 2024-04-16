using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Comandas.Migrations
{
    /// <inheritdoc />
    public partial class AdicionandoDespesaEmTransacoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Despesa",
                table: "transacoes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Despesa",
                table: "transacoes");
        }
    }
}
