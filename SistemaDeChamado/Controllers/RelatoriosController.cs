using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeChamado.Context;
using SistemaDeChamado.Essencial;
using SistemaDeChamado.Models;
using System.Data;
using System.Data.Common;
using System.Timers;

namespace SistemaDeChamado.Controllers
{
    [Authorize(Roles = "Master")]
    public class RelatoriosController : Controller
    {
        private readonly MyContext _context;
        private IMapper _mapper;
        private Essencial.GetUserType _getUserType;
        public RelatoriosController(MyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _getUserType = new Essencial.GetUserType();

        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Previa()
        {
            var razaosocial = new SistemaDeChamado.Entity.PreviaMensal();

            razaosocial.Linhas = new List<Entity.PreviaMensalLinha>();
            razaosocial.DtInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01);
            razaosocial.DtFinal = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
            razaosocial = await PreviaLinhasa(razaosocial);
            return View(razaosocial);
        }
        [HttpPost]
        public async Task<IActionResult> Previa(Entity.PreviaMensal razaosocial)
        {
            razaosocial.Linhas = new List<Entity.PreviaMensalLinha>();
            razaosocial = await PreviaLinhasa(razaosocial);
            return View(razaosocial);
        }
        [HttpGet]
        public async Task<IActionResult> Detalhes(int Id, DateTime dataInicial, DateTime Datafinal)
        {
            var chamadolinhas = _context.ChamadoLinhas.Include(c => c.Chamado).Where(l => l.Chamado.ClienteId == Id && l.DataCriacao >= dataInicial && l.DataCriacao <= Datafinal).ToList();
            //var chamados = chamadolinhas //_context.Chamados.Include(c => c.ChamadoLinhas).Include(c => c.Cliente).Where(c => c.ClienteId == Id).ToList();
            DetelheViewModel lista = new DetelheViewModel();
            TimeSpan restante = new TimeSpan();
            var detalhe = new DetelheViewModel();
            detalhe.Linhas = new List<DetelheLinhasViewModel>();
            foreach (var l in chamadolinhas)
            {
                var linha = new DetelheLinhasViewModel();
                linha.Chamado = l.ChamadoId;
                linha.Descricao = l.Descricao;
                linha.DataCriacao = l.DataCriacao;
                linha.Usuario = _context.Usuarios.Where(u => u.Id == l.UsuarioId).SingleOrDefault().Usuario;
                //restante.AddHours(1);
                //TimeSpan restante = new ;
                restante = restante.Add(new TimeSpan(l.Tempo.Hour, l.Tempo.Minute, l.Tempo.Second));
                linha.Tempo = l.Tempo;
                linha.Assunto = l.Chamado.Descricao;
                detalhe.Linhas.Add(linha);
            }
            //foreach (var c in chamados)
            //{



            //}
            detalhe.Id = Id;
            detalhe.RazaoSocial = _context.Clientes.Where(c => c.Id == Id).SingleOrDefault().RazaoSocial;
            //detalhe.Restante = (((chamados[0].Cliente.HorasContrato * 60) - restante.TotalMinutes) / 60).ToString();
            // = restante;
            if (negativo(restante))
            {
                detalhe.Tempo = $"-{negativoempositivo(restante.Hours + (restante.Days * 24)).ToString().PadLeft(2, '0')}:{negativoempositivo(restante.Minutes).ToString().PadLeft(2, '0')}:{negativoempositivo(restante.Seconds).ToString().PadLeft(2, '0')}";
            }
            else
            {
                detalhe.Tempo = $"{(restante.Hours + (restante.Days * 24)).ToString().PadLeft(2, '0')}:{(restante.Minutes).ToString().PadLeft(2, '0')}:{restante.Seconds.ToString().PadLeft(2, '0')}";
            }
            
            detalhe.DtInicial = dataInicial;
            detalhe.DtFinal = Datafinal;
            lista = detalhe;
            return View(lista);
        }
        private async Task<Entity.PreviaMensal> PreviaLinhasa(Entity.PreviaMensal razaosocial)
        {
            razaosocial.Linhas = new List<Entity.PreviaMensalLinha>();
            using (var context = _context.Database.GetDbConnection())
            {
                await context.OpenAsync();
                using (var command = context.CreateCommand())
                {
                    command.CommandText = "SELECT t3.RazaoSocial,\r\nConvert(SUBSTRING(SEC_TO_TIME( SUM( TIME_TO_SEC(t0.Tempo))),1,2),int) AS 'Horas' ,\r\nConvert(SUBSTRING(SEC_TO_TIME( SUM( TIME_TO_SEC(t0.Tempo))),4,2),int) AS 'Minutos',\r\nt3.HorasContrato,t3.Id \r\nFROM ChamadoLinhas t0 \r\nINNER JOIN Chamados t1 ON t0.ChamadoId = t1.Id\r\nINNER JOIN Usuarios t2 ON t1.UsuarioId = t2.Id\r\nINNER JOIN Clientes t3 ON t2.clienteId = t3.Id\r\n" +
                        $"WHERE t0.DataCriacao BETWEEN '{razaosocial.DtInicial.ToString("yyyyMMdd")}' AND  '{razaosocial.DtFinal.ToString("yyyyMMdd")}'\r\nGROUP BY t3.RazaoSocial";
                    DbDataReader resultado = command.ExecuteReader();

                    if (resultado.HasRows)
                    {
                        while (resultado.Read())
                        {
                            
                            var v = new Entity.PreviaMensalLinha();
                            v.RazaoSocial = resultado.GetString(0);
                            //DateTime DtHorasGastas = new DateTime(2022,11,01, resultado.GetInt32(1), resultado.GetInt32(2),00);
                            TimeSpan Horasgastas = new TimeSpan(resultado.GetInt32(1), resultado.GetInt32(2), 00);
                            TimeSpan horasCOntratadas = new TimeSpan(resultado.GetInt32(3), 00, 00);
                            v.Id= resultado.GetInt32(4);
                            v.Tempo = Horasgastas.ToString();
                            v.Contrato = resultado.GetInt32(3);
                            TimeSpan restante = horasCOntratadas.Add(-Horasgastas);

                            if (negativo(restante))
                            {
                                v.Restante = $"-{negativoempositivo((restante.Hours + (restante.Days * 24))).ToString().PadLeft(2, '0')}:{negativoempositivo(restante.Minutes).ToString().PadLeft(2, '0')}:{negativoempositivo(restante.Seconds).ToString().PadLeft(2, '0')}";
                            }
                            else
                            {
                                v.Restante = $"{(restante.Hours + (restante.Days * 24)).ToString().PadLeft(2, '0')}:{restante.Minutes.ToString().PadLeft(2, '0')}:{restante.Seconds.ToString().PadLeft(2, '0')}";
                            }
                            
                            razaosocial.Linhas.Add(v);
                        }
                    }
                }
                return razaosocial;
            }
            
        }
        private bool negativo(TimeSpan restante)
        {
            //TimeSpan restante = horasCOntratadas.Add(-Horasgastas);
            bool verificador = false;
            if (restante.Days < 0)
            {
                verificador = true;
            }
            if (restante.Minutes < 0)
            {
                verificador = true;
            }
            if (restante.Seconds < 0)
            {
                verificador = true;
            }
            return verificador;
        }
        private int negativoempositivo(int numero)
        {
            if (numero < 0)
                numero = numero * -1;
            return numero;
        }
    }
}
