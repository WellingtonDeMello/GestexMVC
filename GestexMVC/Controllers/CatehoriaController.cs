using GestexMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestexMVC.Controllers
{
    [Authorize] // Exige login para acessar qualquer ação deste controller
    public class CategoriasController : Controller
    {
        private readonly GestexDbContext _context;

        public CategoriasController(GestexDbContext context)
        {
            _context = context;
        }

        // Lista todas as categorias
        public async Task<IActionResult> Index()
        {
            var categorias = await _context.Categorias.ToListAsync();
            return View(categorias);
        }

        // Exibe o formulário de nova categoria
        public IActionResult Create()
        {
            return View();
        }

        // Salva a nova categoria no banco
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categoria);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

        // Exibe o formulário de edição
        public async Task<IActionResult> Edit(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null) return NotFound();
            return View(categoria);
        }

        // Salva as alterações da categoria
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Categoria categoria)
        {
            if (id != categoria.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(categoria);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

        // Exibe a tela de confirmação de exclusão
        // Carrega os produtos vinculados para verificar se pode excluir
        public async Task<IActionResult> Delete(int id)
        {
            var categoria = await _context.Categorias
                .Include(c => c.Produtos)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (categoria == null) return NotFound();
            return View(categoria);
        }

        // Executa a exclusão da categoria
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoria = await _context.Categorias
                .Include(c => c.Produtos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categoria != null)
            {
                // Bloqueia exclusão se houver produtos vinculados
                if (categoria.Produtos != null && categoria.Produtos.Any())
                {
                    ModelState.AddModelError("", "Não é possível excluir uma categoria com produtos vinculados.");
                    return View(categoria);
                }
                _context.Categorias.Remove(categoria);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}