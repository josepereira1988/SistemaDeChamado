using SistemaDeChamado.Essencial;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SistemaDeChamado.Models
{
    public class UsuarioViewModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Informe o usuario")]
        [StringLength(50)]

        public string Usuario { get; set; }

        [Required(ErrorMessage = "Informe o nome")]
        [StringLength(50)]
        public string Nome { get; set; }

        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Classificação")]
        public ControleUsuario Classificacao { get; set; }
    }
}
