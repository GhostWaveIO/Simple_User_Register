using Cadastro.Data;
using Cadastro.Models.Account.Cadastro.Campos;
using Cadastro.Models.Account.Cadastro.Dados;
using Cadastro.Models.Entities.DB.Account;
using Cadastro.Models.Entities.DB.Account.Email;
using Cadastro.Models.Services.Application.Logs;
using Cadastro.Models.Services.Application.Text.Generators.Keys;
using Cadastro.Models.ViewModel.Application.Network.Email;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Cadastro.Models.Services.Application.Network.Email {
    public enum EErrosEmail { Sucesso, Erro, Email_nao_encontrado, Erro_de_autenticacao, Erro_ao_enviar }
    public enum EVerificaConfirmacao { Confirmado, Pendente, Vencido, Ausente, Erro }
    public class EmailService {

        public AppDbContext _context { get; set; }
        public ResultEmailValidadoVM confirmacaoResult { get; set; }
        public string Email { get; set; }
        public string msgResult { get; set; }
        public string Url { get; set; }
        public Usuario Usuario { get; private set; }

        public static List<string> listaTokens { get; set; }

        public EmailService() { }
        public EmailService(AppDbContext context) { _context = context; }

        public SmtpClient cliente = new SmtpClient();
        public NetworkCredential credencial = new NetworkCredential();
        public MailMessage msg = new MailMessage();
        public MailAddress endereco;

        private static bool mailSent = false;

        //##################################################################################################
        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e) {
            // Get the unique identifier for this asynchronous operation.
            string token = (string)e.UserState;

            if(e.Cancelled) {
                Console.WriteLine("[{0}] Send canceled.", token);
            }
            if(e.Error != null) {
                Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
            } else {
                Console.WriteLine("Message sent.");
            }
            mailSent = true;
        }

        //##################################################################################################
        private async Task EnviarEmail(string subject, string body, string para) {

            if(listaTokens == null)
                listaTokens = new List<string>();

            credencial.UserName = Program.Config.EmailAuth["usuario"];
            credencial.Password = Program.Config.EmailAuth["senha"];

            cliente.Host = Program.Config.EmailAuth["host"];
            cliente.Port = Convert.ToInt32(Program.Config.EmailAuth["port"]);
            cliente.EnableSsl = Convert.ToBoolean(Program.Config.EmailAuth["ssl"]);
            cliente.DeliveryMethod = SmtpDeliveryMethod.Network;
            cliente.UseDefaultCredentials = false;
            cliente.Credentials = credencial;

            endereco = new MailAddress(Program.Config.EmailAuth["usuario"], Program.Config.nomeSistema);

            msg.From = endereco;
            msg.Subject = subject;
            msg.IsBodyHtml = true;
            msg.Body = body;
            msg.To.Add(para);

            cliente.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);

            string token = "";
            do {
                token = GeradorKey.Gerar(30, true, true);
            } while(listaTokens.Contains(token));

            //Remover itens excedentes
            if(listaTokens.Count() >= 2000) {
                listaTokens.RemoveRange(0, 1000);
            }

            listaTokens.Add(token);

            try {
                //Envia o Email
                cliente.SendAsync(msg, token);

                //Fica verificando se ocorreu erro no envio (Não é garantido que chegará no destinatário)
                for(int e = 0; e < 50 && mailSent == false; e++) {
                    await Task.Delay(300);
                    if(mailSent == true) {
                        break;
                    }
                }

                //Caso ocorra erro ele cancelará o procedimento
                if(mailSent == false) {
                    cliente.SendAsyncCancel();
                    throw new Exception("Não foi possível enviar o Email!");
                } else {

                }

                //Reclica o cliente depois de 3 segundos (NÃO IMPLEMENTAR AWAIT)
                if(mailSent)
                    DisposeEmail(cliente);
                else
                    cliente.Dispose();
            } catch(Exception err) {
                ErroFileService.ColherErros("Erro: EmailService.EnviarEmail",
                  string.Format("Email: {0}<br/>Body: {1}<br/>Erro de Excessão: {2}", para, body, err.Message));//Colher Erros
                throw new Exception("Ocorreu um erro ao tentar enviar o email de confirmação!", err);
            }
        }

        //##################################################################################################
        public async Task EnviarConfirmacaoCadastro(Usuario usuario, IUrlHelper urlHelper) {
            if(usuario == null)
                throw new Exception("Usuario não encontrado");

            string key = await GerarConfirmacao(usuario);
            if(string.IsNullOrEmpty(key)) {
                await RemoverConfirmacao(usuario.UsuarioId);
                throw new Exception("Não foi possível gerar a chave de confirmação");
            }

            string rodape = Program.Config.rodapeEmail.Replace("#nome-sistema", Program.Config.nomeSistema).
              Replace("#logo-email", $"{urlHelper.ActionLink("", "")}Imagens/logomail.png").
              Replace("#url-site", Program.Config.urlSite).
              Replace("#url-sistema", urlHelper.ActionLink("", "")).
              Replace("#url", $"{urlHelper.ActionLink("ValidarNovoEmail", "Cadastro")}/{key}").
              Replace("#ano-atual", DateTime.Today.Year.ToString());

            //BODY
            //Novo Cadastro
            string corpoCadastro = Program.Config.corpoNovoCadastro + rodape;
            corpoCadastro = corpoCadastro.Replace("#nome-sistema", Program.Config.nomeSistema).
              Replace("#logo-email", $"{urlHelper.ActionLink("", "")}Imagens/logomail.png").
              Replace("#nome", usuario.GetNome).
              Replace("#cpf", usuario.GetCpf).
              Replace("#email", usuario.GetEmail).
              Replace("#url-site", Program.Config.urlSite).
              Replace("#url", $"{urlHelper.ActionLink("ValidarEmail", "Cadastro")}/{key}").
              Replace("#ano-atual", DateTime.Today.Year.ToString());

            //Alterar Email
            string corpoAlterarEmail = Program.Config.corpoAlterarEmail + rodape;
            corpoAlterarEmail = corpoAlterarEmail.Replace("#nome-sistema", Program.Config.nomeSistema).
              Replace("#logo-email", $"{urlHelper.ActionLink("", "")}Imagens/logomail.png").
              Replace("#nome", usuario.GetNome).
              Replace("#cpf", usuario.GetCpf).
              Replace("#email", usuario.GetEmail).
              Replace("#url-site", Program.Config.urlSite).
              Replace("#url", $"{urlHelper.ActionLink("ValidarNovoEmail", "Cadastro")}/{key}").
              Replace("#ano-atual", DateTime.Today.Year.ToString());


            string corpo;
            string email;
            string assuntoEmail = "";
            Dado_Cadastro dadoEmail = usuario.Dados.FirstOrDefault(d => d.Campo.CampoGenerico == ECampoGenerico.Email);
            //Define o modelo de verificação de Email
            if(string.IsNullOrEmpty(dadoEmail.Email_Novo)) {
                corpo = corpoCadastro;
                email = dadoEmail.Email;
                assuntoEmail = "Confirmação de Cadastro";
            } else {
                corpo = corpoAlterarEmail;
                email = dadoEmail.Email_Novo;
                assuntoEmail = "Troca de Email";
            }

            await EnviarEmail(
              assuntoEmail,
              corpo,
              email
            );
        }

        //##################################################################################################
        public async Task<string> GerarConfirmacao(Usuario usuario) {
            string key = null;

            Dado_Cadastro dadoEmail = usuario.Dados.FirstOrDefault(d => d.Campo.CampoGenerico == ECampoGenerico.Email);

            try {
                key = await GeraKey();

            } catch(Exception err) {
                throw err;
            }
            string EmailNovo = null;
            if(!string.IsNullOrEmpty(dadoEmail.Email_Novo) && dadoEmail.Email_Novo != dadoEmail.Email) {
                EmailNovo = dadoEmail.Email_Novo;
                //Remover qualquer outro pedido de alteração de Email
                _context.EmailsValidados.RemoveRange(
                  await Task.FromResult(_context.EmailsValidados.Where(ev => ev.UsuarioId == usuario.UsuarioId && !string.IsNullOrEmpty(ev.EmailNovo)))
                );
            }
            EmailValidado eConfirmado = new EmailValidado() {
                Confirmado = false,
                key = key,
                EmailNovo = EmailNovo,
                Criado = DateTime.Now,
                UsuarioId = usuario.UsuarioId
            };

            try {
                await _context.EmailsValidados.AddAsync(eConfirmado);
                await _context.SaveChangesAsync();
            } catch {
                return null;
            }

            return key;//Retornar o erro caso der um erro
        }

        //##################################################################################################
        private async Task RemoverConfirmacao(long UsuarioId) {
            EmailValidado eConfirmado = await _context.EmailsValidados.FirstOrDefaultAsync(ec => ec.UsuarioId == UsuarioId);
            _context.EmailsValidados.Remove(eConfirmado);

            await _context.SaveChangesAsync();
            GC.Collect();
        }

        //##################################################################################################
        public async Task<EVerificaConfirmacao> VerificarConfirmacaoEmail(long UsuarioId) {
            DateTime dataMinValida = DateTime.Now.AddHours(Program.Config.PrazoConfirmacaoCadastro);

            Usuario usuario = await _context.Usuarios.
              Include(u => u.EmailsValidados).
              FirstOrDefaultAsync(u => u.UsuarioId == UsuarioId);
            //Último Criado
            EmailValidado EmailC = usuario.EmailsValidados?.OrderBy(e => e.Criado).LastOrDefault();

            if(usuario == null) {//Se usuário não for encontrado
                return EVerificaConfirmacao.Erro;
            } else if(EmailC == null) {//Nenhuma validação encontrada
                return EVerificaConfirmacao.Ausente;
            } else if(usuario.EmailsValidados?.Any(e => e.Confirmado) ?? false) {//Verifica confirmados
                return EVerificaConfirmacao.Confirmado;
            } else if(EmailC.Criado <= dataMinValida) {//Passou do tempo limite de confirmação
                return EVerificaConfirmacao.Vencido;
            } else if(EmailC.Criado > dataMinValida) {//confirmação pendente
                return EVerificaConfirmacao.Pendente;
            } else {//Demais problemas são considerados erros
                return EVerificaConfirmacao.Erro;
            }
        }

        //##################################################################################################
        //Valida Key e Atende Alteração de Email
        public async Task<EVerificaConfirmacao> ValidarKey(string key) {
            EVerificaConfirmacao res = EVerificaConfirmacao.Erro;
            IQueryable<EmailValidado> listaKeys = await Task.FromResult(_context.EmailsValidados.
              Include(ev => ev.Usuario).ThenInclude(u => u.EmailsValidados).
              Include(ev => ev.Usuario).ThenInclude(u => u.Dados).ThenInclude(d => d.Campo).
              Include(ev => ev.Usuario).ThenInclude(u => u.Dados).ThenInclude(d => d.Selected).ThenInclude(s => s.Select_Item));
            EmailValidado ec = await listaKeys.FirstOrDefaultAsync(ec => ec.key == key);


            Dado_Cadastro dadoEmail = ec?.Usuario.Dados.First(d => d.Campo.CampoGenerico == ECampoGenerico.Email);

            if(ec?.Usuario != null)
                Usuario = ec.Usuario;

            if(ec == null)
                return EVerificaConfirmacao.Ausente;
            else if(ec.Confirmado) {
                return EVerificaConfirmacao.Confirmado;
            }



            //Verificar se usuário não expirou o tempo de ativação
            if(await ec.Usuario.VerificarLimiteValidacao()) {
                return EVerificaConfirmacao.Ausente;
            }

            DateTime tempoLimite = DateTime.Now.AddHours(-Program.Config.PrazoConfirmacaoCadastro);

            //Atribuir novo Email caso seja Informado até aqui
            if(!string.IsNullOrEmpty(ec.EmailNovo) && tempoLimite <= ec.Criado) {
                dadoEmail.Email = ec.EmailNovo;
                dadoEmail.Email_Novo = null;

                _context.DadosCadastro.Update(dadoEmail);
                _context.EmailsValidados.Remove(ec);
                await _context.SaveChangesAsync();
                return EVerificaConfirmacao.Pendente;
            }

            //ABAIXO somente confirmação de novo cadastro
            if(string.IsNullOrEmpty(ec.EmailNovo) && tempoLimite <= ec.Criado) {
                ec.Confirmado = true;
                ec.EmailNovo = null;
                _context.EmailsValidados.Update(ec);
                await _context.SaveChangesAsync();

                _context.EmailsValidados.RemoveRange(
                    ec.Usuario.EmailsValidados?.Where(ev => !ev.Confirmado) ?? new List<EmailValidado>()
                );
                await _context.SaveChangesAsync();
                res = EVerificaConfirmacao.Pendente;
            }

            return res;
        }

        //##################################################################################################
        /// <summary>Reclica o ClientSmtp depois de 4 segundos. Obg: Não implemente "await" na chamada deste método</summary>
        /// <param name="cliente">o SmtpClient do email enviado</param>
        private async Task DisposeEmail(SmtpClient cliente) {
            await Task.Delay(4000);
            cliente.Dispose();
        }

        //##################################################################################################
        private async Task<string> GeraKey() {//Gerar somente se não existir a Key no banco de dados
            int tamKey = 30;//Tamanho da Key
            char[] listaChars = new char[] {
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
        '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'
      };
            string res;

            do {
                res = string.Empty;
                for(int c = 0; c < tamKey; c++) {
                    Random rand = new Random();
                    res += listaChars[rand.Next(0, listaChars.Length - 1)];
                }
            } while(await _context.EmailsValidados.AnyAsync(ec => ec.key == res));

            return res;
        }

    }
}
