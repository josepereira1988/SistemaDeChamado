using System.ComponentModel.DataAnnotations;

namespace SistemaDeChamado.Models
{
    public class AlterarSenhaUsuarioViewModel
    {
        public int Id { get; set; }
        public string Usuario { get; set; }

        [Required(ErrorMessage = "Informe a senha")]
        [StringLength(200)]
        [DataType(DataType.Password)]
        [Display(Name = "Nova senha")]
        public string Senha1 { get; set; }
        [Required(ErrorMessage = "Informe a senha")]
        [StringLength(200)]
        [DataType(DataType.Password)]
        [Display(Name = "Repete a senha")]
        public string Senha2 { get; set; }
    }
}
