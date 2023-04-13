using SistemaDeChamado.Context;
using SistemaDeChamado.Entity;
using System.Security.Claims;

namespace SistemaDeChamado.Essencial
{
    public class GetUserType
    {
        private GetUserType Usuario { get; set; }
        public GetUserType()
        {
        }

        public string GetClienteId()
        {
            return "clienteId";
        }
        public string GetUserId()
        {
            return "Id";
        }
        public string GetUserClassificacao()
        {
            return "Classificacao";
        }
        //private Usuarios GetUsuarios()
        //{
        //    return _context.Usuarios.Where(u => u.Usuario == User.Identity.Name).SingleOrDefault();
        //}

    }
}
