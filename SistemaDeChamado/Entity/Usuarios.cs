using SistemaDeChamado.Essencial;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaDeChamado.Entity
{
    public class Usuarios
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Informe o usuario")]
        [StringLength(50)]
        [Index(IsUnique =true)]
        public string Usuario { get; set; }
        [Required(ErrorMessage = "Informe a senha")]
        [StringLength(200)]
        [DataType(DataType.Password)]
        public string Senha { get; set; }
        [Required(ErrorMessage = "Informe o nome")]
        [StringLength(50)]
        public string Nome { get; set; }

        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        [Display(Name ="Email")]
        public string Email { get; set; }

        [StringLength(50)]
        public string idSistema { get; set; }
        public int? clienteId { get; set; }
        public Cliente cliente { get; set; }

        [Display(Name = "Classificação")]
        public ControleUsuario Classificacao { get; set; }
        public bool Ativo { get; set; }
        public bool ClienteMaster { get; set; }
    }
}
