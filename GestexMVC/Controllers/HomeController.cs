using GestexMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace GestexMVC.Controllers
{
    [Authorize] // Exige login para acessar qualquer ação deste controller
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GestexDbContext _context;

        public HomeController(ILogger<HomeController> logger, GestexDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Tela inicial com os dados do dashboard
        public async Task<IActionResult> Index()
        {
            var hoje = DateTime.Today;

            // Total de vendas confirmadas no dia
            ViewBag.TotalVendasHoje = await _context.Vendas
                .Where(v => v.DataVenda.Date == hoje && v.Status == "Confirmada")
                .CountAsync();

            // Valor total faturado no dia
            ViewBag.FaturamentoHoje = await _context.Vendas
                .Where(v => v.DataVenda.Date == hoje && v.Status == "Confirmada")
                .SumAsync(v => (decimal?)v.Total) ?? 0;

            // Total de clientes ativos cadastrados
            ViewBag.TotalClientes = await _context.Clientes
                .Where(c => c.Ativo)
                .CountAsync();

            // Produtos com estoque igual ou abaixo do mínimo
            ViewBag.ProdutosCriticos = await _context.Produtos
                .Where(p => p.Ativo && p.QtdEstoque <= p.EstoqueMinimo)
                .ToListAsync();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // Página de erro padrão do ASP.NET Core
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}