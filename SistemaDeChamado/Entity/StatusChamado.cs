using System.ComponentModel.DataAnnotations;

namespace SistemaDeChamado.Entity
{
    public class StatusChamado
    {
        public int Id { get; set; }
        [StringLength(50)]
        [Display(Name ="Status")]
        public string Descricao { get; set; }
    }
}
