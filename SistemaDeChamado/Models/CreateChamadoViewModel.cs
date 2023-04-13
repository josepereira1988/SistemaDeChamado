using SistemaDeChamado.Entity;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SistemaDeChamado.Models
{
    public class CreateChamadoViewModel
    {
        [Required(ErrorMessage = "Informe um titulo")]
        [StringLength(20)]
        [Display(Name = "Titulo")]
        public string CurtaDescricao { get; set; }

        [Required(ErrorMessage = "Informe uma Descrição")]
        [StringLength(200)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }
        [Display(Name = "Nome")]
        [StringLength(200)]
        public string NomeUsuario { get; set; }
        [Display(Name = "Cliente")]
        
        public int ClienteId { get; set; }
    }
}
