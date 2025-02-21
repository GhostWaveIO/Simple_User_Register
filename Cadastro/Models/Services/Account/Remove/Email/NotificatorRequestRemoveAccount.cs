using Cadastro.Models.Entities.DB.Account;
using Cadastro.Models.Services.Account;
using Cadastro.Models.Services.Application.Network.Email;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Cadastro.Models.Services.Account.Remove.Email
{
    /// <summary>
    /// Responsável por enviar uma notificação por Email sobre uma nova solicitação de exclusão de usuário
    /// </summary>
    public class NotificatorRequestRemoveAccount
    {
        public NotificatorRequestRemoveAccount(Usuario usuario)
        {
            _usuario = usuario;
        }

        private Usuario _usuario { get; set; }

        private string _ip { get; set; }


        //##################################################################################################################################
        public async Task SendRequestAsync(string ip)
        {
            string subject = "Solicitação de Encerramento de Conta";
            string body = GenerateBody();
            string to = "encerramento@site.com.br";
            _ip = ip;

            AuthenticatedEmailSender sender = new AuthenticatedEmailSender(Program.Config, Program.Config.nomeSistema);
            await sender.SendAsync(subject, body, to);
        }

        //##################################################################################################################################
        private string GenerateBody()
        {
            UserEntityExtractorData extractor = new UserEntityExtractorData(_usuario);

            string userCode = extractor.ExtractUserCode();
            string name = extractor.ExtractName();
            string email = extractor.ExtractEmail();

            string res = "" +
                "<h3>Solicitação de Encerramento de Conta de Usuário</h3>";

            return res;
        }
    }
}
