using Cadastro.Models.Entities.DB.Account;

namespace Cadastro.Models.Services.Application.Security.Autorizacao
{

    public enum ENivelH { Iniciante, Intermediário, Avançado }
    public enum ENivelHPopular { Diamante, Ouro, Prata }

    public class HierarquiaUsuario
    {
        public long HierarquiaUsuarioId { get; set; }

        public long UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        public ENivelH? NivelHorizontal { get; set; }

        public int HierarquiaId { get; set; }
        public Hierarquia Hierarquia { get; set; }
    }
}
