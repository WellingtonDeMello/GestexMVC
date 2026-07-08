using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestexMVC.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarEnderecoFuncionario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Endereco",
                table: "Funcionarios",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Endereco",
                table: "Funcionarios");
        }
    }
}
