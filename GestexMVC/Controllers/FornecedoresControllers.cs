using GestexMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestexMVC.Controllers
{
    [Authorize] // Exige login para acessar qualquer ação deste controller
    public class FornecedoresController : Controller
    {
        private readonly GestexDbContext _context;

        public FornecedoresController(GestexDbContext context)
        {
            _context = context;
        }

        // Lista todos os fornecedores
        public async Task<IActionResult> Index()
        {
            var fornecedores = await _context.Fornecedores.ToListAsync();
            return View(fornecedores);
        }

        // Exibe o formulário de novo fornecedor
        public IActionResult Create()
        {
            return View();
        }

        // Salva o novo fornecedor no banco
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Fornecedor fornecedor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fornecedor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fornecedor);
        }

        // Exibe o formulário de edição
        public async Task<IActionResult> Edit(int id)
        {
            var fornecedor = await _context.Fornecedores.FindAsync(id);
            if (fornecedor == null) return NotFound();
            return View(fornecedor);
        }

        // Salva as alterações do fornecedor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Fornecedor fornecedor)
        {
            if (id != fornecedor.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(fornecedor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fornecedor);
        }

        // Exibe a tela de confirmação de exclusão
        public async Task<IActionResult> Delete(int id)
        {
            var fornecedor = await _context.Fornecedores.FindAsync(id);
            if (fornecedor == null) return NotFound();
            return View(fornecedor);
        }

        // Executa a exclusão do fornecedor
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fornecedor = await _context.Fornecedores.FindAsync(id);
            if (fornecedor != null)
            {
                _context.Fornecedores.Remove(fornecedor);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}