using Microsoft.AspNetCore.Identity;

namespace SistemaDeChamado.Essencial
{
    public class UserApp : IdentityUser
    {
        public ControleUsuario ControleUsuario { get; set; }
        public int Id { get; set; }
    }
}
