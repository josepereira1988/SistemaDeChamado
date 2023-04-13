using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SistemaDeChamado.Entity
{
    public class Chamado
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Informe um titulo")]
        [StringLength(20)]
        [Display(Name = "Titulo")]
        public string CurtaDescricao { get; set; }

        [Required(ErrorMessage = "Informe uma Descrição")]
        [StringLength(200)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
        public int StatusChamadoId { get; set; }
        public StatusChamado StatusChamado { get; set; }
        public string NomeUsuario { get; set; }
        [Display(Name = "Usuario")]
        public int UsuarioId { get; set; }
        public Usuarios Usuario { get; set; }
        [Display(Name = "Usuario")]
        public int? TecnicoId { get; set; }
        public Usuarios Tecnico { get; set; }
        [Display(Name = "Cliente")]
        public int ClienteId { get ; set; }
        public Cliente Cliente { get; set; }
        public List<ChamadoLinhas> ChamadoLinhas { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? Abertura { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? Finalizado { get; set; }
    }
}
