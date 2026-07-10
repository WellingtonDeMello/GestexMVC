using GestexMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestexMVC.Controllers
{
    [Authorize] // Exige login para acessar qualquer ação deste controller
    public class FuncionariosController : Controller
    {
        private readonly GestexDbContext _context;

        public FuncionariosController(GestexDbContext context)
        {
            _context = context;
        }

        // Lista todos os funcionários
        public async Task<IActionResult> Index()
        {
            var funcionarios = await _context.Funcionarios.ToListAsync();
            return View(funcionarios);
        }

        // Exibe o formulário de novo funcionário
        public IActionResult Create()
        {
            return View();
        }

        // Salva o novo funcionário no banco
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

        // Exibe o formulário de edição
        public async Task<IActionResult> Edit(int id)
        {
            var funcionario = await _context.Funcionarios.FindAsync(id);
            if (funcionario == null) return NotFound();
            return View(funcionario);
        }

        // Salva as alterações do funcionário
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

        // Exibe a tela de confirmação de exclusão
        public async Task<IActionResult> Delete(int id)
        {
            var funcionario = await _context.Funcionarios.FindAsync(id);
            if (funcionario == null) return NotFound();
            return View(funcionario);
        }

        // Executa a exclusão do funcionário
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var funcionario = await _context.Funcionarios.FindAsync(id);
            if (funcionario != null)
            {
                // Bloqueia exclusão se o funcionário tiver vendas vinculadas
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