using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SistemaDeChamado.Entity
{
    public class Cliente
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Digite a Razao Social")]
        [StringLength(100)]
        [Display(Name = "Razao Social")]
        public string RazaoSocial { get; set; }
        [Required(ErrorMessage = "Digite o CNPJ")]
        [StringLength(20)]
        [Display(Name = "CNPJ")]
        public string CNPJ { get; set; }
        [Required(ErrorMessage = "Digite o Telefone")]
        [StringLength(20)]
        [Display(Name = "Telefone")]
        [DataType(DataType.PhoneNumber)]
        public string Telefone { get; set; }

        [StringLength(50)]
        [Display(Name = "Site")]
        [DataType(DataType.Url)]
        public string Site { get; set; }
        public int HorasContrato { get; set; }
        public bool Ativo { get; set; }
    }
}
