using Microsoft.EntityFrameworkCore;

namespace GestexMVC.Models
{
    public class GestexDbContext : DbContext
    {
        public GestexDbContext(DbContextOptions<GestexDbContext> options)
            : base(options)
        {
        }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<VendaItem> VendaItens { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed do usuário admin inicial
            var senhaHash = HashSenha("admin123");

            modelBuilder.Entity<Usuario>().HasData(new Usuario
            {
                Id = 1,
                Nome = "Administrador",
                Login = "admin",
                Senha = senhaHash,
                Perfil = "Admin",
                Ativo = true
            });
        }

        private string HashSenha(string senha)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(senha);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}