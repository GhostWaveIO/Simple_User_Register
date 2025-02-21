using Cadastro.Models.Entities.DB.Account;
using Cadastro.Models.Services.Account;
using Cadastro.Models.Services.Application.Network.Email;
using System;
using System.Threading.Tasks;

namespace Cadastro.Models.Services.Account.Remove.Email
{
    /// <summary>Ao remover uma conta com sucesso, esta classe tem por objetivo notificar ao usuário que o processo foi feito com sucesso!</summary>
    public class NotificatorAccountRemovedSuccessfully
    {
        public NotificatorAccountRemovedSuccessfully(Usuario usuario)
        {
            _usuario = usuario;
        }

        private Usuario _usuario { get; set; }

        //##############################################################################################################################
        public async Task SendNotificationAsync()
        {
            UserEntityExtractorData extractor = new UserEntityExtractorData(_usuario);

            string subject = "Conta Removida com Sucesso!";
            string body = GenerateBody();
            string to = extractor.ExtractEmail();

            AuthenticatedEmailSender sender = new AuthenticatedEmailSender(Program.Config, Program.Config.nomeSistema);
            await sender.SendAsync(subject, body, to);
        }

        //##############################################################################################################################
        private string GenerateBody()
        {
            UserEntityExtractorData extractor = new UserEntityExtractorData(_usuario);
            string name = extractor.ExtractName();
            string email = extractor.ExtractEmail();
            string userCode = extractor.ExtractUserCode();


            string res = "<p>&nbsp;</p>\r\n";



            return res;
        }
    }
}
