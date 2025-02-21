using Cadastro.Models.Entities.DB.Account;

namespace Cadastro.Models.Services.Application.Security.Autorizacao
{
    public class FuncaoUsuario
    {
        public long FuncaoUsuarioId { get; set; }

        public long UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public int HierarquiaId { get; set; }
        public Hierarquia Hierarquia { get; set; }
    }
}
