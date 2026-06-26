using GestexMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestexMVC.Controllers
{
    public class VendasController : Controller
    {
        private readonly GestexDbContext _context;

        public VendasController(GestexDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vendas = await _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Funcionario)
                .OrderByDescending(v => v.DataVenda)
                .ToListAsync();
            return View(vendas);
        }

        public IActionResult Create()
        {
            ViewBag.Clientes = _context.Clientes.Where(c => c.Ativo).ToList();
            ViewBag.Funcionarios = _context.Funcionarios.Where(f => f.Ativo).ToList();
            ViewBag.Produtos = _context.Produtos.Where(p => p.Ativo && p.QtdEstoque > 0).ToList();
            return View(new Venda());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venda venda, int[] ProdutoIds, int[] Quantidades)
        {
            if (ProdutoIds.Length == 0)
            {
                ModelState.AddModelError("", "Adicione pelo menos um produto à venda.");
            }

            if (ModelState.IsValid)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    venda.DataVenda = DateTime.Now;
                    venda.Status = "Confirmada";
                    _context.Add(venda);
                    await _context.SaveChangesAsync();

                    decimal total = 0;
                    for (int i = 0; i < ProdutoIds.Length; i++)
                    {
                        var produto = await _context.Produtos.FindAsync(ProdutoIds[i]);
                        if (produto == null) continue;

                        if (produto.QtdEstoque < Quantidades[i])
                        {
                            ModelState.AddModelError("", $"Estoque insuficiente para o produto {produto.Nome}.");
                            await transaction.RollbackAsync();
                            ViewBag.Clientes = _context.Clientes.Where(c => c.Ativo).ToList();
                            ViewBag.Funcionarios = _context.Funcionarios.Where(f => f.Ativo).ToList();
                            ViewBag.Produtos = _context.Produtos.Where(p => p.Ativo && p.QtdEstoque > 0).ToList();
                            return View(venda);
                        }

                        var item = new VendaItem
                        {
                            VendaId = venda.Id,
                            ProdutoId = ProdutoIds[i],
                            Quantidade = Quantidades[i],
                            PrecoUnitario = produto.PrecoVenda,
                            Subtotal = produto.PrecoVenda * Quantidades[i]
                        };

                        produto.QtdEstoque -= Quantidades[i];
                        total += item.Subtotal;

                        _context.Add(item);
                        _context.Update(produto);
                    }

                    venda.Total = total;
                    _context.Update(venda);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", "Erro ao salvar a venda. Tente novamente.");
                }
            }

            ViewBag.Clientes = _context.Clientes.Where(c => c.Ativo).ToList();
            ViewBag.Funcionarios = _context.Funcionarios.Where(f => f.Ativo).ToList();
            ViewBag.Produtos = _context.Produtos.Where(p => p.Ativo && p.QtdEstoque > 0).ToList();
            return View(venda);
        }

        public async Task<IActionResult> Details(int id)
        {
            var venda = await _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Funcionario)
                .Include(v => v.Itens)
                .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venda == null) return NotFound();
            return View(venda);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var venda = await _context.Vendas
                    .Include(v => v.Itens)
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (venda == null) return NotFound();

                foreach (var item in venda.Itens)
                {
                    var produto = await _context.Produtos.FindAsync(item.ProdutoId);
                    if (produto != null)
                    {
                        produto.QtdEstoque += item.Quantidade;
                        _context.Update(produto);
                    }
                }

                venda.Status = "Cancelada";
                _context.Update(venda);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}