using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaDeChamado.Context;
using SistemaDeChamado.Entity;
using SistemaDeChamado.Essencial;
using SistemaDeChamado.Models;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace SistemaDeChamado.Controllers
{
    [Authorize(Roles = "Master,Tecnico")]
    public class PainelController : Controller
    {
        private MyContext _context;
        private IMapper _mapper;
        private Essencial.GetUserType _getUserType;
        public PainelController(MyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _getUserType = new Essencial.GetUserType();
        }

        public IActionResult Index(int tecnico = 0,
            bool AguardandoAtendimento  = true,
            bool EmAndamento  = true,
            bool AguardandoCliente  = true,
            bool AguardandoTerceiro = true,
            bool Finalizado = false)
        {
            ViewBag.AguardandoAtendimento = AguardandoAtendimento;
            ViewBag.EmAndamento = EmAndamento;
            ViewBag.AguardandoCLiente = AguardandoCliente;
            ViewBag.AguardandoTerceiro = AguardandoTerceiro;
            ViewBag.Finalizado = Finalizado;
            PainelChamadoViewModel painel = new PainelChamadoViewModel();
            var myContext = _context.Chamados.Include(t => t.Tecnico).Include(c => c.Cliente).Include(c => c.StatusChamado).Include(c => c.Usuario).AsQueryable();
            painel.Chamados = myContext.Where(c => c.TecnicoId == null).ToList();

			var status = new int[] { 0, 0, 0, 0,0 };            
			if (AguardandoAtendimento)
			{
				status[0] = 2;
			}
			if (EmAndamento)
			{
				status[1] = 3;
			}
			if (AguardandoCliente)
			{
				status[2] = 4;
			}
			if (AguardandoTerceiro)
			{
				status[3] = 5;
			}
			if (Finalizado)
			{
				status[4] = 6;
			}
			if (tecnico == 0)
            {
                tecnico = Convert.ToInt32(HttpContext.User.FindFirstValue(_getUserType.GetUserId()));

                
                painel.SeuChamados = myContext.Where(c => c.TecnicoId == tecnico && status.Contains(c.StatusChamadoId)).ToList();
                
            }
            else
            {
                painel.SeuChamados = myContext.Where(c => c.TecnicoId == tecnico && status.Contains(c.StatusChamadoId)).ToList();
            }
            ViewData["Tecnico"] = new SelectList(_context.Usuarios.Where(u => u.Classificacao == ControleUsuario.Tecnico || u.Classificacao == ControleUsuario.Master),"Id","Nome",tecnico);
            return View(painel);
        }
        public async Task<IActionResult> PegarChamado(int id)
        {
            if(HttpContext.User.FindFirstValue(_getUserType.GetUserClassificacao()) == Essencial.ControleUsuario.Tecnico.ToString())
            {
                var UpdateChamado = _context.Chamados.Where(i => i.Id == id).SingleOrDefault();
                UpdateChamado.TecnicoId = Convert.ToInt32(HttpContext.User.FindFirstValue(_getUserType.GetUserId()));
                UpdateChamado.StatusChamadoId = 2;
                _context.Update(UpdateChamado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Tecnico"] = new SelectList(_context.Usuarios.Where(i => i.Classificacao == ControleUsuario.Tecnico || i.Classificacao == ControleUsuario.Master && i.Ativo == true), "Id", "Nome");
            return View(new PegaChamadoViewModel { Id = id });
        }
        [HttpPost]
        public async Task<IActionResult> PegarChamado(PegaChamadoViewModel pega)
        {
            if (HttpContext.User.FindFirstValue(_getUserType.GetUserClassificacao()) == Essencial.ControleUsuario.Master.ToString())
            {
                var UpdateChamado = _context.Chamados.Where(i => i.Id == pega.Id).SingleOrDefault();
                UpdateChamado.TecnicoId = pega.TecnicoId;
                UpdateChamado.StatusChamadoId = 2;
                _context.Update(UpdateChamado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Tecnico"] = new SelectList(_context.Usuarios.Where(i => i.Classificacao == ControleUsuario.Tecnico || i.Classificacao == ControleUsuario.Master && i.Ativo == true), "Id", "Nome");
            return View(new PegaChamadoViewModel { Id = pega.Id });
        }
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

            return View(chamado);

        }
        public async Task<IActionResult> ChamadosLinhas(int id)
        {
            ViewData["StatusChamado"] = new SelectList(_context.StatusChamado.Where(p => p.Id != 1 && p.Id != 2), "Id", "Descricao");
            return View(new CreateChamadoLinhasViewModel { ChamadoId = id, UsuarioId = Convert.ToInt32(HttpContext.User.FindFirstValue(_getUserType.GetUserId())) });
        }
        [HttpPost]
        public async Task<IActionResult> ChamadosLinhas(CreateChamadoLinhasViewModel chamadolinhas)
        {
            if (ModelState.IsValid)
            {
                var salvar = _mapper.Map<ChamadoLinhas>(chamadolinhas);
                var chamado = _context.Chamados.Where(c => c.Id == salvar.ChamadoId).SingleOrDefault();
                salvar.DataCriacao = DateTime.Today;
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
            ViewData["StatusChamado"] = new SelectList(_context.StatusChamado.Where(p => p.Id != 1 && p.Id != 2), "Id", "Descricao", chamadolinhas.StatusChamadoId);
            return View(chamadolinhas);
        }
    }
}
