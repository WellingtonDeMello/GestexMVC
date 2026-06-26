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
    }
}