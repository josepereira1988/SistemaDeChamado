using System.ComponentModel.DataAnnotations;

namespace SistemaDeChamado.Models
{
    public class DetelheLinhasViewModel
    {
        public int Chamado { get; set; }
        public string Assunto { get; set; }
        public string Descricao { get; set; }
        public string Usuario { get; set; }
        public TimeOnly Tempo { get; set; }

        [DataType(DataType.Date)]
        public DateTime DataCriacao { get; set; }

    }
}
