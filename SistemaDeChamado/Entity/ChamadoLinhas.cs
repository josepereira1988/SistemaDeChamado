using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SistemaDeChamado.Entity
{
    public class ChamadoLinhas
    {
        public int Id { get; set; }
        public int ChamadoId { get; set; }
        public Chamado Chamado { get; set; }
        [Required(ErrorMessage = "Informe uma Descrição")]
        [StringLength(200)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
        [Display(Name = "Status")]
        public int StatusChamadoId { get; set; }
        public StatusChamado StatusChamado { get; set; }
        public int UsuarioId { get; set; }
        public Usuarios Usuario { get; set; }
        [DataType(DataType.Date)]
        public DateTime DataCriacao { get; set; }
        [DataType(DataType.Time)]
        public TimeOnly Inicio { get; set; }
        [DataType(DataType.Time)]
        public TimeOnly Fim { get; set; }
        public TimeOnly Tempo { get; set; }
    }
}
