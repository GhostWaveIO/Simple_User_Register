using Cadastro.Models.Services.Application.Settings.Geral;
using Cadastro.Models.Services.Application.Text.Generators.Keys;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Cadastro.Models.Services.Application.Network.Email
{
    public class AuthenticatedEmailSender
    {

        //Envio de Email com as credenciais informadas
        public AuthenticatedEmailSender(string userName, string password, string sendFrom)
        {
            //Username = Program.Config.EmailAuth["usuario"];
            //Password = Program.Config.EmailAuth["senha"];
            //this.subject = subject;
            //this.body = body;
            //this.para = para;
            _username = userName;
            _password = password;
            _sendFrom = sendFrom;
        }
        //Envio de Email com as credenciais diretamente
        public AuthenticatedEmailSender(Config config, string sendFrom)
        {
            _username = config.EmailAuth["usuario"];
            _password = config.EmailAuth["senha"];
            _sendFrom = sendFrom;
        }

        //Objetos de Envio
        private SmtpClient _cliente = new SmtpClient();
        private NetworkCredential _credencial = new NetworkCredential();
        private MailMessage _msg = new MailMessage();
        private MailAddress _endereco;
        private static List<string> _listaTokens { get; set; }
        private string _sendFrom { get; set; }

        ////Conteúdo
        //private string subject { get; set; }
        //private string body { get; set; }
        //private string para { get; set; }

        //Credenciais
        private string _username { get; set; }
        private string _password { get; set; }

        private bool _mailSent = false;

        //#########################################################################################################
        private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            string token = (string)e.UserState;

            if (e.Cancelled)
            {
                Console.WriteLine("[{0}] Envio Cancelado.", token);
            }
            if (e.Error != null)
            {
                Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
            }
            else
            {
                Console.WriteLine("Mensagem cancelada.");
            }
            _mailSent = true;
        }

        //#########################################################################################################
        public async Task SendAsync(string subject, string body, string para)
        {

            if (_listaTokens == null) _listaTokens = new List<string>();

            _credencial.UserName = _username;
            _credencial.Password = _password;

            _cliente.Host = Program.Config.EmailAuth["host"];
            _cliente.Port = Convert.ToInt32(Program.Config.EmailAuth["port"]);
            _cliente.EnableSsl = Convert.ToBoolean(Program.Config.EmailAuth["ssl"]);
            _cliente.DeliveryMethod = SmtpDeliveryMethod.Network;
            _cliente.UseDefaultCredentials = false;
            _cliente.Credentials = _credencial;

            _endereco = new MailAddress(Program.Config.EmailAuth["usuario"], _sendFrom);

            _msg.From = _endereco;
            _msg.Subject = subject;
            _msg.IsBodyHtml = true;
            _msg.Body = body;
            _msg.To.Add(para);

            _cliente.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);

            string token = "";
            do
            {
                token = GeradorKey.Gerar(30, true, true);
            } while (_listaTokens.Contains(token));

            _listaTokens.Add(token);

            try
            {
                //Envia o Email
                _cliente.SendAsync(_msg, token);

                //Fica verificando se ocorreu erro no envio (Não é garantido que chegará no destinatário)
                for (int e = 0; e < 12 && _mailSent == false; e++)
                {
                    await Task.Delay(300);
                    if (_mailSent == true)
                    {
                        break;
                    }
                }

                //Caso ocorra erro ele cancelará o procedimento
                if (_mailSent == false)
                {
                    _cliente.SendAsyncCancel();
                    throw new Exception("Não foi possível enviar o Email!");
                }
                else
                {

                }

                //Reclica o cliente depois de 3 segundos (NÃO IMPLEMENTAR AWAIT)
                if (_mailSent)
                    _ = DisposeEmail(_cliente);
                else
                    _cliente.Dispose();
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        ////#########################################################################################################
        //public async Task Executar(string subject, string body, string para) {
        //    try {
        //        await EnviarEmail(subject, body, para);
        //    } catch (Exception err) {
        //        throw new Exception("Ocorreu um erro ao tentar enviar o Email!", err);
        //    }
        //}

        //##################################################################################################
        /// <summary>Recicla o ClientSmtp depois de 4 segundos. Obg: Não implemente "await" na chamada deste método</summary>
        /// <param name="cliente">o SmtpClient do email enviado</param>
        private async Task DisposeEmail(SmtpClient cliente)
        {
            await Task.Delay(4000);
            cliente.Dispose();
        }

    }
}
