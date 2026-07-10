using GestexMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestexMVC.Controllers
{
    [Authorize] // Exige login para acessar qualquer ação deste controller
    public class VendasController : Controller
    {
        private readonly GestexDbContext _context;

        public VendasController(GestexDbContext context)
        {
            _context = context;
        }

        // Lista todas as vendas com cliente e funcionário, da mais recente para a mais antiga
        public async Task<IActionResult> Index()
        {
            var vendas = await _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Funcionario)
                .OrderByDescending(v => v.DataVenda)
                .ToListAsync();
            return View(vendas);
        }

        // Exibe o formulário de nova venda com listas de clientes, funcionários e produtos disponíveis
        public IActionResult Create()
        {
            ViewBag.Clientes = _context.Clientes.Where(c => c.Ativo).ToList();
            ViewBag.Funcionarios = _context.Funcionarios.Where(f => f.Ativo).ToList();
            ViewBag.Produtos = _context.Produtos.Where(p => p.Ativo && p.QtdEstoque > 0).ToList(); // Só produtos com estoque
            return View(new Venda());
        }

        // Processa a criação da venda com os itens selecionados
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venda venda, int[] ProdutoIds, int[] Quantidades)
        {
            // Exige pelo menos um produto na venda
            if (ProdutoIds.Length == 0)
            {
                ModelState.AddModelError("", "Adicione pelo menos um produto à venda.");
            }

            if (ModelState.IsValid)
            {
                // Usa transação para garantir que tudo salva ou nada salva
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

                        // Verifica se há estoque suficiente antes de confirmar
                        if (produto.QtdEstoque < Quantidades[i])
                        {
                            ModelState.AddModelError("", $"Estoque insuficiente para o produto {produto.Nome}.");
                            await transaction.RollbackAsync();
                            ViewBag.Clientes = _context.Clientes.Where(c => c.Ativo).ToList();
                            ViewBag.Funcionarios = _context.Funcionarios.Where(f => f.Ativo).ToList();
                            ViewBag.Produtos = _context.Produtos.Where(p => p.Ativo && p.QtdEstoque > 0).ToList();
                            return View(venda);
                        }

                        // Cria o item da venda com preço e subtotal
                        var item = new VendaItem
                        {
                            VendaId = venda.Id,
                            ProdutoId = ProdutoIds[i],
                            Quantidade = Quantidades[i],
                            PrecoUnitario = produto.PrecoVenda,
                            Subtotal = produto.PrecoVenda * Quantidades[i]
                        };

                        // Baixa o estoque do produto
                        produto.QtdEstoque -= Quantidades[i];
                        total += item.Subtotal;

                        _context.Add(item);
                        _context.Update(produto);
                    }

                    // Atualiza o total da venda e confirma a transação
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

        // Exibe os detalhes completos da venda com todos os itens e produtos
        public async Task<IActionResult> Details(int id)
        {
            var venda = await _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Funcionario)
                .Include(v => v.Itens)
                .ThenInclude(i => i.Produto) // Carrega o produto de cada item
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venda == null) return NotFound();
            return View(venda);
        }

        // Cancela a venda e devolve o estoque dos produtos
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

                // Devolve a quantidade ao estoque de cada produto da venda
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

        // Relatório de vendas filtrado por período
        public async Task<IActionResult> Relatorio(DateTime? dataInicio, DateTime? dataFim)
        {
            // Se não informar datas, exibe os últimos 30 dias
            dataInicio ??= DateTime.Today.AddDays(-30);
            dataFim ??= DateTime.Today;

            var vendas = await _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Funcionario)
                .Where(v => v.DataVenda.Date >= dataInicio.Value.Date
                         && v.DataVenda.Date <= dataFim.Value.Date
                         && v.Status == "Confirmada") // Só vendas confirmadas
                .OrderByDescending(v => v.DataVenda)
                .ToListAsync();

            ViewBag.DataInicio = dataInicio.Value.ToString("yyyy-MM-dd");
            ViewBag.DataFim = dataFim.Value.ToString("yyyy-MM-dd");
            ViewBag.TotalPeriodo = vendas.Sum(v => v.Total);   // Soma do faturamento
            ViewBag.TotalVendas = vendas.Count;                 // Quantidade de vendas

            return View(vendas);
        }
    }
}