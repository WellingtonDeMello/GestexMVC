using GestexMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestexMVC.Controllers
{
    [Authorize]
    public class ProdutosController : Controller
    {
        private readonly GestexDbContext _context;

        public ProdutosController(GestexDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var produtos = await _context.Produtos
                .Include(p => p.Categoria)
                .ToListAsync();
            return View(produtos);
        }

        public IActionResult Create()
        {
            ViewBag.Categorias = _context.Categorias.Where(c => c.Ativo).ToList();
            return View();
        }

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

        public async Task<IActionResult> Edit(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null) return NotFound();
            ViewBag.Categorias = _context.Categorias.Where(c => c.Ativo).ToList();
            return View(produto);
        }

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

        public async Task<IActionResult> Delete(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null) return NotFound();
            return View(produto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                _context.Produtos.Remove(produto);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var produto = await _context.Produtos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (produto == null) return NotFound();
            return View(produto);
        }

        public async Task<IActionResult> AjustarEstoque(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null) return NotFound();
            return View(produto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AjustarEstoque(int id, int quantidade)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null) return NotFound();

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
    }
}