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
    }
}