using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestexMVC.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarFormaPagamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FormaPagamento",
                table: "Vendas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "ValorRecebido",
                table: "Vendas",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormaPagamento",
                table: "Vendas");

            migrationBuilder.DropColumn(
                name: "ValorRecebido",
                table: "Vendas");
        }
    }
}
