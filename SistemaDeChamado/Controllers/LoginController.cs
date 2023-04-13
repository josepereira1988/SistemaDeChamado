using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeChamado.Context;
using SistemaDeChamado.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using SistemaDeChamado.Entity;
using Microsoft.AspNetCore.Identity;
using SistemaDeChamado.Essencial;

namespace SistemaDeChamado.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly MyContext _context;
        public LoginController(MyContext context)
        {
            _context = context;
 
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel login)
        {
            if (login != null)
            {
                var usuario = await _context.Usuarios.Where(u => u.Usuario.ToLower() == login.Usuario.ToLower() && u.Ativo == true).SingleOrDefaultAsync();
                if (usuario != null)
                {
                    if (Essencial.Criptografia.DesCriptografar(usuario.Senha, usuario.idSistema) == login.Senha)
                    {
                        if (usuario.Classificacao == Essencial.ControleUsuario.Cliente)
                        {
                            if ((_context.Clientes?.Any(p => p.Id == usuario.clienteId && p.Ativo == true)).GetValueOrDefault())
                            {

                                await autenticardo(usuario);
                                return RedirectToAction("Index", "Home");
                            }
                        }
                        else
                        {

                            await autenticardo(usuario);
                            return RedirectToAction("Index", "Home");

                        }
                    }

                }

                ModelState.AddModelError("Usuario", "Verifique usuario e senha");


            }
            return View(login);
        }
        public async Task<IActionResult> logou()
        {
            await HttpContext.SignOutAsync();
            return View("Index");
        }
        private async Task<bool> autenticardo(Usuarios usuario)
        {
            var claims = new List<Claim>
                           {
                //Atributos do usuário ...
                               new Claim(ClaimTypes.Name, usuario.Usuario),
                               new Claim(ClaimTypes.Role, usuario.Classificacao.ToString()),
                               new Claim("Classificacao", usuario.Classificacao.ToString()),
                               new Claim("Usuario", usuario.Nome,ClaimValueTypes.String),
                               new Claim("Id", usuario.Id.ToString(),ClaimValueTypes.String)
                           };
            if (usuario.Classificacao == Essencial.ControleUsuario.Cliente)
            {
                claims.Add(new Claim("ClienteId", usuario.clienteId.ToString(), ClaimValueTypes.String));
            }
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false
            };
            await HttpContext.SignInAsync(
              CookieAuthenticationDefaults.AuthenticationScheme,
              new ClaimsPrincipal(claimsIdentity),
              authProperties
               );
            return true;
        }
    }
}
