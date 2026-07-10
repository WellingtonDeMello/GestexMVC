using GestexMVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestexMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly GestexDbContext _context;

        public AuthController(GestexDbContext context)
        {
            _context = context;
        }

        // Exibe a tela de login
        public IActionResult Login()
        {
            return View();
        }

        // Processa o login quando o formulário é enviado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string usuario, string senha)
        {
            // Valida se os campos foram preenchidos
            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(senha))
            {
                ViewBag.Erro = "Preencha o usuário e a senha!";
                return View();
            }

            // Criptografa a senha antes de buscar no banco
            var senhaHash = HashSenha(senha);

            // Busca o usuário no banco
            var user = _context.Usuarios.FirstOrDefault(u =>
                u.Login == usuario &&
                u.Senha == senhaHash &&
                u.Ativo);

            if (user != null)
            {
                // Cria a sessão do usuário com nome, login e perfil
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Login),
                    new Claim(ClaimTypes.GivenName, user.Nome),
                    new Claim(ClaimTypes.Role, user.Perfil)
                };

                var identity = new ClaimsIdentity(claims, "CookieAuth");
                var principal = new ClaimsPrincipal(identity);

                // Grava o cookie de autenticação
                await HttpContext.SignInAsync("CookieAuth", principal);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Erro = "Usuário ou senha incorretos!";
            return View();
        }

        // Encerra a sessão e volta para o login
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Login");
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