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

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string usuario, string senha)
        {
            var senhaHash = HashSenha(senha);

            var user = _context.Usuarios.FirstOrDefault(u =>
                u.Login == usuario &&
                u.Senha == senhaHash &&
                u.Ativo);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Login),
                    new Claim(ClaimTypes.GivenName, user.Nome),
                    new Claim(ClaimTypes.Role, user.Perfil)
                };

                var identity = new ClaimsIdentity(claims, "CookieAuth");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("CookieAuth", principal);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Erro = "Usuário ou senha incorretos!";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Login");
        }

        private string HashSenha(string senha)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(senha);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}