using Cadastro.Data;
using Cadastro.Models.Entities.DB.Account;
using Cadastro.Models.Services.Application.Network.Email;
using Cadastro.Models.Services.Application.Text.Generators.Keys;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace Cadastro.Models.Entities.DB.Account.Password
{
    public class NovaSenhaSolicitada
    {

        public int NovaSenhaSolicitadaId { get; set; }
        public AppDbContext context { get; set; }
        private Usuario usuarioEntity { get; set; }

        [Required]
        [StringLength(30)]
        public string key { get; set; }
        [Required]
        public DateTime Criado { get; set; }

        public long UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        [NotMapped]
        public string msgResult { get; private set; }

        public NovaSenhaSolicitada() { }

        public NovaSenhaSolicitada(AppDbContext context, Usuario usuario)
        {
            this.context = context;
            usuarioEntity = usuario;
            UsuarioId = usuario.UsuarioId;
            Criado = DateTime.Now;
        }

        //#########################################################################################################
        public async Task<string> GerarKey()
        {

            string key;
            do
            {
                key = GeradorKey.Gerar(30, true, true);//Program.Config.GeraKey(30, false);
            } while (await context.NovasSenhasSolicidatas.AnyAsync(ns => ns.key == key));

            return key;
        }

        //#########################################################################################################
        private async Task EnviarPedidoNovaSenha(IUrlHelper urlHelper)
        {

            string url = $"{urlHelper.ActionLink("NovaSenha", "Recuperacao")}/{key}/";

            string rodape = Program.Config.rodapeEmail.Replace("#nome-sistema", Program.Config.nomeSistema).
                Replace("#logo-email", $"{urlHelper.ActionLink("", "")}Imagens/logomail.png").
                Replace("#url-site", Program.Config.urlSite).
                Replace("#url-sistema", urlHelper.ActionLink("", "")).
                Replace("#url", url).
                Replace("#ano-atual", DateTime.Today.Year.ToString());

            string subject = "Solicitação de Recuperação de Senha - " + Program.Config.nomeSistema;
            string body = Program.Config.corpoRecuperarSenha + rodape;
            body = body.Replace("#nome-sistema", Program.Config.nomeSistema).
                Replace("#logo-email", $"{urlHelper.ActionLink("", "")}Imagens/logomail.png").
                Replace("#nome", usuarioEntity.GetNome).
                Replace("#cpf", usuarioEntity.GetCpf).
                Replace("#email", usuarioEntity.GetEmail).
                Replace("#url-site", Program.Config.urlSite).
                Replace("#url", url).
                Replace("#ano-atual", DateTime.Today.Year.ToString());


            string para = usuarioEntity.GetEmail;

            AuthenticatedEmailSender envio = new AuthenticatedEmailSender(Program.Config, Program.Config.nomeSistema);
            await envio.SendAsync(subject, body, para);
        }

        //#########################################################################################################
        public async Task Executar(IUrlHelper urlHelper)
        {
            await EnviarPedidoNovaSenha(urlHelper);
        }
    }
}
