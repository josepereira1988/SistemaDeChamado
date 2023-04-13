using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SistemaDeChamado.Models
{
    public class DetelheViewModel
    {
        public int Id { get; set; }
        public string RazaoSocial { get; set; }
        public string Tempo { get; set; }
        public string Restante { get; set; }

        [DataType(DataType.Date)]
        public DateTime DtInicial { get; set; }
        [DataType(DataType.Date)]
        public DateTime DtFinal { get; set; }
        public List<DetelheLinhasViewModel> Linhas { get; set; }
    }
}
