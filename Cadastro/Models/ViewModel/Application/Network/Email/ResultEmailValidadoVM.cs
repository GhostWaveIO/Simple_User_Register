using Cadastro.Models.Entities.DB.Account;
using Cadastro.Models.Services.Application.Network.Email;

namespace Cadastro.Models.ViewModel.Application.Network.Email
{
    public class ResultEmailValidadoVM
    {

        public EVerificaConfirmacao Resultado { get; set; }
        public Usuario Usuario { get; set; }
    }
}
