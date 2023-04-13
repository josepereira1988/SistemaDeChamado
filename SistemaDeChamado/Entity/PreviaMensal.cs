using System.ComponentModel.DataAnnotations;

namespace SistemaDeChamado.Entity
{
    public class PreviaMensal
    {
        [DataType(DataType.Date)] 
        public DateTime DtInicial { get; set; }
        [DataType(DataType.Date)]
        public DateTime DtFinal { get; set; }
        public List<PreviaMensalLinha> Linhas { get; set; }
    }
}
