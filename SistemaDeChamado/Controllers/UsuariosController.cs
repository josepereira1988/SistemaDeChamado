using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaDeChamado.Context;
using SistemaDeChamado.Entity;
using SistemaDeChamado.Essencial;
using SistemaDeChamado.Models;

namespace SistemaDeChamado.Controllers
{
    
    public class UsuariosController : Controller
    {
        private readonly MyContext _context;
        private IMapper _mapper;
		private Essencial.GetUserType _getUserType;
		public UsuariosController(MyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
			_getUserType = new Essencial.GetUserType();
		}
		[Authorize(Roles = "Master")]
		// GET: Usuarios
		public async Task<IActionResult> Index()
        {
            return View(_mapper.Map<List<Models.UsuarioViewModel>>(await _context.Usuarios.ToListAsync()));
        }
		[Authorize(Roles = "Master")]
		// GET: Usuarios/Details/5
		public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuarios = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuarios == null)
            {
                return NotFound();
            }

            return View(usuarios);
        }
		[Authorize(Roles = "Master")]
		// GET: Usuarios/Create
		public IActionResult Create()
        {

            ViewData["ClienteId"] = new SelectList(Clientes(), "Id", "RazaoSocial");
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "Master")]
		public async Task<IActionResult> Create(Models.CrieateUsuarioViewModel usuarios)
        {
            if (ModelState.IsValid)
            {
                if (!(_context.Usuarios?.Any(e => e.Usuario.ToLower() == usuarios.Usuario.ToLower())).GetValueOrDefault())
                {
                    var salvar = _mapper.Map<Usuarios>(usuarios);
                    salvar.idSistema = Essencial.CriarChave.Chave();
                    if (salvar.clienteId == 0)
                        salvar.clienteId = null;
                    salvar.Senha = Criptografia.Criptografar(usuarios.Senha, salvar.idSistema);
                    _context.Add(salvar);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("CustomError", "Usuario já existe na base de dados");
            }
            ViewData["Classificacao"] = Convert.ToInt32(usuarios.Classificacao);
            ViewData["ClienteId"] = new SelectList(Clientes(), "Id", "RazaoSocial", usuarios.clienteId);
            return View(usuarios);
        }

		// GET: Usuarios/Edit/5
		[Authorize(Roles = "Master")]
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuarios = await _context.Usuarios.FindAsync(id);
            if (usuarios == null)
            {
                return NotFound();
            }
            //ViewData["Classificacao"] = Essencial.ControleUsuario;
            ViewData["ClienteId"] = new SelectList(Clientes(), "Id", "RazaoSocial", usuarios.clienteId);
            return View(_mapper.Map<Models.EditarUsuariosViewModel>(usuarios));
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "Master")]
		public async Task<IActionResult> Edit(EditarUsuariosViewModel usuarios)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var usuarioBanco = _context.Usuarios.Where(p => p.Id == usuarios.Id).SingleOrDefault();
                    if (usuarioBanco != null)
                    {
                        var Salvar = _mapper.Map<Usuarios>(usuarios);
                        Salvar.Senha = usuarioBanco.Senha;
                        Salvar.idSistema = usuarioBanco.idSistema;
                        if (Salvar.clienteId == 0)
                            Salvar.clienteId = null;
                        _context.Entry(usuarioBanco).CurrentValues.SetValues(Salvar);
                        await _context.SaveChangesAsync();
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuariosExists(usuarios.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Classificacao"] = Convert.ToInt32(usuarios.Classificacao);
            ViewData["ClienteId"] = new SelectList(Clientes(), "Id", "RazaoSocial", usuarios.clienteId);
            return View(usuarios);
        }
		[Authorize]
		public async Task<IActionResult> SalvarSenha(int id)
        {
            if(HttpContext.User.FindFirstValue(_getUserType.GetUserClassificacao()) == Essencial.ControleUsuario.Master.ToString())
            {
				var Usuario = await _context.Usuarios.Where(u => u.Id == id).SingleOrDefaultAsync();
				return View(new AlterarSenhaUsuarioViewModel { Id = id, Usuario = Usuario.Usuario });
            }
            else
            {
				var Usuario = await _context.Usuarios.Where(u => u.Id == Convert.ToInt32(HttpContext.User.FindFirstValue(_getUserType.GetUserId()))).SingleOrDefaultAsync();
				return View(new AlterarSenhaUsuarioViewModel { Id = id, Usuario = Usuario.Usuario });
			}

        }
        [HttpPost]
		[Authorize]
		public async Task<IActionResult> SalvarSenha(AlterarSenhaUsuarioViewModel senha)
        {
            if (ModelState.IsValid)
            {
                if (senha.Senha1 == senha.Senha2)
                {
                    var Salvar = await _context.Usuarios.Where(e => e.Id == senha.Id).SingleOrDefaultAsync();
                    if (Salvar != null)
                    {
                        Salvar.Senha = Essencial.Criptografia.Criptografar(senha.Senha1, Salvar.idSistema);
                        _context.Update(Salvar);
                        await _context.SaveChangesAsync();
                        return RedirectToActionPermanent(nameof(Index));
                    }
                }
            }
            return View(senha);
        }
		// GET: Usuarios/Delete/5
		[Authorize(Roles = "Master")]
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Usuarios == null)
            {
                return NotFound();
            }

            var usuarios = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuarios == null)
            {
                return NotFound();
            }

            return View(usuarios);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "Master")]
		public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Usuarios == null)
            {
                return Problem("Entity set 'MyContext.Usuarios'  is null.");
            }
            var usuarios = await _context.Usuarios.FindAsync(id);
            if (usuarios != null)
            {
                _context.Usuarios.Remove(usuarios);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuariosExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
        private List<Cliente> Clientes()
        {
            var selectCliente = new List<Cliente>();
            selectCliente.Add(new Cliente { RazaoSocial = "--" });
            selectCliente.AddRange(_context.Clientes.Where(u => u.Ativo == true).ToList());
            return selectCliente;
        }
    }
}
