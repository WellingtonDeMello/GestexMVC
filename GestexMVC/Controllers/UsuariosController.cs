using GestexMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestexMVC.Controllers
{
    [Authorize(Roles = "Admin")] // Somente Admin pode acessar este controller
    public class UsuariosController : Controller
    {
        private readonly GestexDbContext _context;

        public UsuariosController(GestexDbContext context)
        {
            _context = context;
        }

        // Lista todos os usuários do sistema
        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            return View(usuarios);
        }

        // Exibe o formulário de novo usuário
        public IActionResult Create()
        {
            return View();
        }

        // Salva o novo usuário com a senha já criptografada
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                usuario.Senha = HashSenha(usuario.Senha); // Criptografa antes de salvar
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // Exibe o formulário de edição com a senha em branco por segurança
        public async Task<IActionResult> Edit(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();
            usuario.Senha = ""; // Não exibe a senha atual no formulário
            return View(usuario);
        }

        // Salva as alterações do usuário
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Usuario usuario)
        {
            if (id != usuario.Id) return NotFound();
            if (ModelState.IsValid)
            {
                var usuarioExistente = await _context.Usuarios.FindAsync(id);
                if (usuarioExistente == null) return NotFound();

                // Atualiza apenas os campos editáveis
                usuarioExistente.Nome = usuario.Nome;
                usuarioExistente.Login = usuario.Login;
                usuarioExistente.Perfil = usuario.Perfil;
                usuarioExistente.Ativo = usuario.Ativo;

                // Só atualiza a senha se uma nova foi digitada
                if (!string.IsNullOrEmpty(usuario.Senha))
                    usuarioExistente.Senha = HashSenha(usuario.Senha);

                _context.Update(usuarioExistente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // Exibe a tela de confirmação de exclusão
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        // Executa a exclusão do usuário
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Criptografa a senha usando SHA-256
        private string HashSenha(string senha)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(senha);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}