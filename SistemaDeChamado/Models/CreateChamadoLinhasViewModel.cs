using SistemaDeChamado.Entity;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SistemaDeChamado.Models
{
    public class CreateChamadoLinhasViewModel
    {
        public int ChamadoId { get; set; }
        [Required(ErrorMessage = "Informe uma Descrição")]
        [StringLength(200)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
        [Display(Name = "Status")]
        public int StatusChamadoId { get; set; }
        public int UsuarioId { get; set; }
        [Display(Name = "Data")]
        [DataType(DataType.Date)]
		public DateTime DataLancamento { get; set; }

		[DataType(DataType.Time)]
        public DateTime Inicio { get; set; }
        [DataType(DataType.Time)]
        public DateTime Fim { get; set; }
    }
}
