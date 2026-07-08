using GestexMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestexMVC.Controllers
{
    [Authorize]
    public class FuncionariosController : Controller
    {
        private readonly GestexDbContext _context;

        public FuncionariosController(GestexDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var funcionarios = await _context.Funcionarios.ToListAsync();
            return View(funcionarios);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Funcionario funcionario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(funcionario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(funcionario);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var funcionario = await _context.Funcionarios.FindAsync(id);
            if (funcionario == null) return NotFound();
            return View(funcionario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Funcionario funcionario)
        {
            if (id != funcionario.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(funcionario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(funcionario);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var funcionario = await _context.Funcionarios.FindAsync(id);
            if (funcionario == null) return NotFound();
            return View(funcionario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var funcionario = await _context.Funcionarios.FindAsync(id);
            if (funcionario != null)
            {
                bool temVendas = await _context.Vendas.AnyAsync(v => v.FuncionarioId == id);
                if (temVendas)
                {
                    ModelState.AddModelError("", "Não é possível excluir este funcionário pois ele possui vendas vinculadas.");
                    return View(funcionario);
                }
                _context.Funcionarios.Remove(funcionario);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}