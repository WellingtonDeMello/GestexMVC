using GestexMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestexMVC.Controllers
{
    [Authorize] // Exige login para acessar qualquer ação deste controller
    public class ProdutosController : Controller
    {
        private readonly GestexDbContext _context;

        public ProdutosController(GestexDbContext context)
        {
            _context = context;
        }

        // Lista todos os produtos com a categoria vinculada
        public async Task<IActionResult> Index()
        {
            var produtos = await _context.Produtos
                .Include(p => p.Categoria) // Carrega a categoria junto com o produto
                .ToListAsync();
            return View(produtos);
        }

        // Exibe o formulário de novo produto com a lista de categorias ativas
        public IActionResult Create()
        {
            ViewBag.Categorias = _context.Categorias.Where(c => c.Ativo).ToList();
            return View();
        }

        // Salva o novo produto no banco
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Produto produto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(produto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(produto);
        }

        // Exibe o formulário de edição com a lista de categorias ativas
        public async Task<IActionResult> Edit(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null) return NotFound();
            ViewBag.Categorias = _context.Categorias.Where(c => c.Ativo).ToList();
            return View(produto);
        }

        // Salva as alterações do produto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Produto produto)
        {
            if (id != produto.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(produto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(produto);
        }

        // Exibe a tela de confirmação de exclusão
        public async Task<IActionResult> Delete(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null) return NotFound();
            return View(produto);
        }

        // Executa a exclusão do produto
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                // Bloqueia exclusão se o produto estiver vinculado a alguma venda
                bool temVendas = await _context.VendaItens.AnyAsync(v => v.ProdutoId == id);
                if (temVendas)
                {
                    ModelState.AddModelError("", "Não é possível excluir este produto pois ele possui vendas vinculadas.");
                    return View(produto);
                }
                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Exibe os detalhes completos do produto com categoria
        public async Task<IActionResult> Details(int id)
        {
            var produto = await _context.Produtos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (produto == null) return NotFound();
            return View(produto);
        }

        // Exibe o formulário de ajuste de estoque
        public async Task<IActionResult> AjustarEstoque(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null) return NotFound();
            return View(produto);
        }

        // Aplica o ajuste de estoque (positivo para adicionar, negativo para remover)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AjustarEstoque(int id, int quantidade)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null) return NotFound();

            // Impede que o estoque fique negativo
            if (produto.QtdEstoque + quantidade < 0)
            {
                ModelState.AddModelError("", "Estoque não pode ficar negativo!");
                return View(produto);
            }

            produto.QtdEstoque += quantidade;
            _context.Update(produto);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Relatório com posição atual do estoque e totais
        public async Task<IActionResult> RelatorioEstoque()
        {
            var produtos = await _context.Produtos
                .Include(p => p.Categoria)
                .OrderBy(p => p.Nome)
                .ToListAsync();

            ViewBag.TotalProdutos = produtos.Count;
            ViewBag.ProdutosZerados = produtos.Count(p => p.QtdEstoque == 0);
            ViewBag.ProdutosCriticos = produtos.Count(p => p.QtdEstoque > 0 && p.QtdEstoque <= p.EstoqueMinimo);
            ViewBag.ValorTotalEstoque = produtos.Sum(p => p.PrecoCusto * p.QtdEstoque); // Custo × quantidade

            return View(produtos);
        }
    }
}