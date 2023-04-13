using SistemaDeChamado.Entity;

namespace SistemaDeChamado.Models
{
    public class PainelChamadoViewModel
    {
        public PainelChamadoViewModel()
        {
            Chamados = new List<Chamado>();
            SeuChamados = new List<Chamado>();
        }
        public List<Chamado> Chamados { get; set; }
        public List<Chamado> SeuChamados { get; set; }
    }
}
