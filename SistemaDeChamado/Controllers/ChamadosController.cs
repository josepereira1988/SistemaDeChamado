using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaDeChamado.Context;
using SistemaDeChamado.Entity;
using SistemaDeChamado.Models;
using X.PagedList;

namespace SistemaDeChamado.Controllers
{
    [Authorize]
    public class ChamadosController : Controller
    {
        private readonly MyContext _context;
        private IMapper _mapper;
        private Essencial.GetUserType _getUserType;
        public ChamadosController(MyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _getUserType = new Essencial.GetUserType();

        }

        // GET: Chamados
        public async Task<IActionResult> Index(int pagina = 1, int filtroId = 1, string filtro = "", bool Aberto = true, bool AguardandoAtendimento = true, bool EmAtendimento = true
            , bool AguardandoCliente = true, bool AguarandoTerceiros = true, bool Finalizado = false)
        {
            //ViewData["Status"] = new SelectList(_context.StatusChamado, "Id", "Descricao", pesquisarStatus);
            ViewBag.Aberto = Aberto;
            ViewBag.AguardandoAtendimento = AguardandoAtendimento;
            ViewBag.EmAtendimento = EmAtendimento;
            ViewBag.AguardandoCliente = AguardandoCliente;
            ViewBag.AguarandoTerceiros = AguarandoTerceiros;
            ViewBag.Finalizado = Finalizado;
            var myContext = _context.Chamados.Include(t => t.Tecnico).Include(c => c.Cliente).Include(c => c.StatusChamado).Include(c => c.Usuario).AsQueryable();
            int[] statusId = new int[6];
            if (Aberto)
            {
                statusId[0] = 1;
            }
            if (AguardandoAtendimento)
            {
                statusId[1] = 2;
            }
            if (EmAtendimento)
            {
                statusId[2] = 3;
            }
            if (AguardandoCliente)
            {
                statusId[3] = 4;
            }
            if (AguarandoTerceiros)
            {
                statusId[4] = 5;
            }
            if (Finalizado)
            {
                statusId[5] = 6;
            }
            if (filtro != "" && filtro != null)
            {
                if (filtroId == 1)
                {
                    myContext = myContext.Where(s => s.Id == Convert.ToInt32(filtro));
                }
                else
                {
                    myContext = myContext.Where(s => s.CurtaDescricao.Contains(filtro));
                }
            }
            myContext = myContext.Where(s => statusId.Contains(s.StatusChamadoId));
            if (HttpContext.User.FindFirstValue(_getUserType.GetUserClassificacao()) == Essencial.ControleUsuario.Cliente.ToString())
            {
                var usuario = _context.Usuarios.Where(p => p.Usuario == User.Identity.Name).SingleOrDefault();
                if (usuario.ClienteMaster)
                {
                    myContext = myContext.Where(p => p.Usuario.clienteId == usuario.clienteId);
                }
                else
                {
                    myContext = myContext.Where(p => p.Usuario.Usuario == User.Identity.Name);
                }
            }
            return View(await myContext.ToPagedListAsync(pagina, 10));
        }

        // GET: Chamados/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Chamados == null)
            {
                return NotFound();
            }

            var chamado = await _context.Chamados
                .Include(c => c.Cliente)
                .Include(c => c.StatusChamado)
            .Include(c => c.Usuario)
            .Include(l => l.ChamadoLinhas)
                .FirstOrDefaultAsync(m => m.Id == id);
            for (int i = 0; i < chamado.ChamadoLinhas.Count; i++)
            {
                chamado.ChamadoLinhas[i].Usuario = _context.Usuarios.Where(u => u.Id == chamado.ChamadoLinhas[i].UsuarioId).SingleOrDefault();
            }
            if (chamado == null)
            {
                return NotFound();
            }
            if (HttpContext.User.FindFirstValue(_getUserType.GetUserClassificacao()) != Essencial.ControleUsuario.Cliente.ToString())
            {
                return View(chamado);
            }
            if (chamado.Usuario.Usuario == User.Identity.Name || chamado.ClienteId == Convert.ToInt32(HttpContext.User.FindFirstValue(_getUserType.GetClienteId())))
            {
                return View(chamado);
            }
            else
            {
                var usuario = GetUsuarios();
                if (Convert.ToInt32(HttpContext.User.FindFirstValue(_getUserType.GetClienteId())) == chamado.ClienteId && usuario.ClienteMaster == true)
                {
                    return View(chamado);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Chamados/Create
        public IActionResult Create()
        {			
			int IdUsuario = Convert.ToInt32(HttpContext.User.FindFirstValue(_getUserType.GetUserId()));
			if (HttpContext.User.FindFirstValue(_getUserType.GetUserClassificacao()) == Essencial.ControleUsuario.Cliente.ToString())
            {
				int IdCliente = Convert.ToInt32(HttpContext.User.FindFirstValue(_getUserType.GetClienteId()));
				ViewData["ClienteId"] = new SelectList(_context.Clientes.Where(u => u.Id == IdCliente), "Id", "RazaoSocial", IdCliente);
                //ViewData["UsuarioId"] = new SelectList(_context.Usuarios.Where(u => u.Id == IdUsuario), "Id", "Nome", IdUsuario);
            }
            else
            {
                ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "RazaoSocial");
                //ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Nome");
                //ViewData["UsuarioId"] = _context.Usuarios.Where(u => u.Ativo == true && u.cliente.Ativo == true).ToList();
            }

			//ViewData["ClienteId"] = new SelectList(_context.Clientes.Where(u => u.Id == IdCliente), "Id", "RazaoSocial", IdCliente);
			ViewData["UsuarioId"] = Convert.ToInt32(HttpContext.User.FindFirstValue(_getUserType.GetClienteId()));

			return View();
        }

        // POST: Chamados/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.CreateChamadoViewModel chamado)
        {
            if (ModelState.IsValid)
            {
                var Salvar = _mapper.Map<Chamado>(chamado);
                Salvar.Abertura = DateTime.Now;
                Salvar.StatusChamadoId = 1;
                Salvar.UsuarioId = Convert.ToInt32(HttpContext.User.FindFirstValue(_getUserType.GetUserId()));
                //if ((User.Claims?.Any(p => p.Value == "Cliente")).GetValueOrDefault())
                //{
                //    int IdCliente = Convert.ToInt32(HttpContext.User.FindFirstValue(_getUserType.GetClienteId()));
                //    int IdUsuario = Convert.ToInt32(HttpContext.User.FindFirstValue(_getUserType.GetUserId()));
                //    Salvar.ClienteId = IdCliente;
                //    Salvar.UsuarioId = IdUsuario;
                //}
                _context.Add(Salvar);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            if (HttpContext.User.FindFirstValue(_getUserType.GetUserClassificacao()) == Essencial.ControleUsuario.Cliente.ToString())
            {
                int IdCliente = Convert.ToInt32(HttpContext.User.FindFirstValue(_getUserType.GetClienteId()));
                int IdUsuario = Convert.ToInt32(HttpContext.User.FindFirstValue(_getUserType.GetUserId()));
                ViewData["ClienteId"] = new SelectList(_context.Clientes.Where(u => u.Id == IdCliente), "Id", "RazaoSocial", IdCliente);
                ViewData["UsuarioId"] = new SelectList(_context.Usuarios.Where(u => u.Id == IdUsuario), "Id", "Nome", IdUsuario);
            }
            else
            {
                ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "RazaoSocial");
                ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Nome");
            }
            //ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "RazaoSocial", chamado.ClienteId);
            //ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Nome", chamado.UsuarioId);
            return View(chamado);
        }

        // GET: Chamados/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Chamados == null)
            {
                return NotFound();
            }

            var chamado = await _context.Chamados.FindAsync(id);
            if (chamado == null)
            {
                return NotFound();
            }
            if (HttpContext.User.FindFirstValue(_getUserType.GetUserClassificacao()) == Essencial.ControleUsuario.Cliente.ToString())
            {
                int IdCliente = Convert.ToInt32(HttpContext.User.FindFirstValue(_getUserType.GetClienteId()));
                int IdUsuario = Convert.ToInt32(HttpContext.User.FindFirstValue(_getUserType.GetUserId()));
                ViewData["ClienteId"] = new SelectList(_context.Clientes.Where(u => u.Id == IdCliente), "Id", "RazaoSocial", IdCliente);
                ViewData["UsuarioId"] = new SelectList(_context.Usuarios.Where(u => u.Id == IdUsuario), "Id", "Nome", IdUsuario);
            }
            else
            {
                ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "RazaoSocial");
                ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Nome");
            }
            //ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "CNPJ", chamado.ClienteId);
            //ViewData["StatusChamadoId"] = new SelectList(_context.StatusChamado, "Id", "Id", chamado.StatusChamadoId);
            //ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Nome", chamado.UsuarioId);
            return View(chamado);
        }

        // POST: Chamados/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Chamado chamado)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chamado);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChamadoExists(chamado.Id))
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
            if (HttpContext.User.FindFirstValue(_getUserType.GetUserClassificacao()) == Essencial.ControleUsuario.Cliente.ToString())
            {
                int IdCliente = Convert.ToInt32(HttpContext.User.FindFirstValue(_getUserType.GetClienteId()));
                int IdUsuario = Convert.ToInt32(HttpContext.User.FindFirstValue(_getUserType.GetUserId()));
                ViewData["ClienteId"] = new SelectList(_context.Clientes.Where(u => u.Id == IdCliente), "Id", "RazaoSocial", IdCliente);
                ViewData["UsuarioId"] = new SelectList(_context.Usuarios.Where(u => u.Id == IdUsuario), "Id", "Nome", IdUsuario);
            }
            else
            {
                ViewBag.ClienteId = new SelectList(_context.Clientes, "Id", "RazaoSocial");
                ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Nome");
            }
            //ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "CNPJ", chamado.ClienteId);
            //ViewData["StatusChamadoId"] = new SelectList(_context.StatusChamado, "Id", "Id", chamado.StatusChamadoId);
            //ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Nome", chamado.UsuarioId);
            return View(chamado);
        }
        public IActionResult Cliente(int id)
        {
            var usuario = _context.Usuarios.Select(s => new { s.Id, s.Nome, s.clienteId, s.Ativo }).AsQueryable();
            if (HttpContext.User.FindFirstValue(_getUserType.GetUserClassificacao()) == Essencial.ControleUsuario.Cliente.ToString())
            {
                usuario = usuario.Where(u => u.Id == Convert.ToInt32(HttpContext.User.FindFirstValue(_getUserType.GetClienteId())));
            }
            usuario = usuario.Where(p => p.clienteId == id && p.Ativo == true);
            return Json(usuario.ToList());
        }

        public async Task<IActionResult> ChamadosLinhas(int id)
        {
            ViewData["StatusChamado"] = new SelectList(_context.StatusChamado, "Id", "Descricao");
            return View(new CreateChamadoLinhasViewModel { ChamadoId = id,DataLancamento = DateTime.Now, UsuarioId = Convert.ToInt32(HttpContext.User.FindFirstValue(_getUserType.GetUserId())) });
        }
        [HttpPost]
        public async Task<IActionResult> ChamadosLinhas(CreateChamadoLinhasViewModel chamadolinhas)
        {
            if (ModelState.IsValid)
            {
                var salvar = _mapper.Map<ChamadoLinhas>(chamadolinhas);
                var chamado = _context.Chamados.Where(c => c.Id == salvar.ChamadoId).SingleOrDefault();
                salvar.DataCriacao = chamadolinhas.DataLancamento;
                if (salvar.Inicio.ToTimeSpan().TotalSeconds > 0 || salvar.Fim.ToTimeSpan().TotalSeconds > 0)
                {
                    salvar.Tempo = salvar.Fim.Add(-salvar.Inicio.ToTimeSpan());
                }
                if (salvar.StatusChamadoId != chamado.StatusChamadoId)
                {
                    var salvarChamado = chamado;
                    salvarChamado.StatusChamadoId = salvar.StatusChamadoId;
                    _context.Entry(chamado).CurrentValues.SetValues(salvarChamado);
                    await _context.SaveChangesAsync();
                }
                _context.Add(salvar);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = chamadolinhas.ChamadoId });
            }
            ViewData["StatusChamado"] = new SelectList(_context.StatusChamado, "Id", "Descricao", chamadolinhas.StatusChamadoId);
            return View(chamadolinhas);
        }

        private bool ChamadoExists(int id)
        {
            return _context.Chamados.Any(e => e.Id == id);
        }
        private Usuarios GetUsuarios()
        {
            return _context.Usuarios.Where(p => p.Usuario == User.Identity.Name).SingleOrDefault();
        }

    }
}
