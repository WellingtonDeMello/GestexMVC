using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestexMVC.Migrations
{
    /// <inheritdoc />
    public partial class SeedUsuarioAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Ativo", "Login", "Nome", "Perfil", "Senha" },
                values: new object[] { 1, true, "admin", "Administrador", "Admin", "JAvlGPq9JyTdtvBO6x2llnRI1+gxwIyPqCKAn3THIKk=" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
