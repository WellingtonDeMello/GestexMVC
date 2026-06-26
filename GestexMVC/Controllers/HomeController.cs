using System.Diagnostics;
using GestexMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestexMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly GestexDbContext _context;

        public HomeController(ILogger<HomeController> logger, GestexDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var hoje = DateTime.Today;

            ViewBag.TotalVendasHoje = await _context.Vendas
                .Where(v => v.DataVenda.Date == hoje && v.Status == "Confirmada")
                .CountAsync();

            ViewBag.FaturamentoHoje = await _context.Vendas
                .Where(v => v.DataVenda.Date == hoje && v.Status == "Confirmada")
                .SumAsync(v => (decimal?)v.Total) ?? 0;

            ViewBag.TotalClientes = await _context.Clientes
                .Where(c => c.Ativo)
                .CountAsync();

            ViewBag.ProdutosCriticos = await _context.Produtos
                .Where(p => p.Ativo && p.QtdEstoque <= p.EstoqueMinimo)
                .ToListAsync();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}