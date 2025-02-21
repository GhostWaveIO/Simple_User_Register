using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Cadastro.Models.Services.Application.Settings.Geral {
    public class Config {

        #region Statics Properties
        public static readonly string PathConfig = Path.Combine(Environment.CurrentDirectory, "StaticFiles", "internal", "settings", "general.json");
        #endregion FIM | Statics Properties

        public string Versao { get; private set; }



        #region Emails e Autenticações
        //Configurações de Envio de E-mail
        [JsonPropertyName("AutenticacaoEmail")]
        public Dictionary<string, string> EmailAuth { get; set; }

        [JsonPropertyName("CorpoNovoCadastro")]
        public string corpoNovoCadastro { get; set; }

        [JsonPropertyName("CorpoRecuperacaoSenha")]
        public string corpoRecuperarSenha { get; set; }

        [JsonPropertyName("CorpoAlterarEmail")]
        public string corpoAlterarEmail { get; set; }

        [JsonPropertyName("RodapeEmail")]
        public string rodapeEmail { get; set; }

        public int PrazoConfirmacaoCadastro { get; set; }
        #endregion

        #region Cores

        //Cores
        [JsonPropertyName("CoresMenu")]
        public Dictionary<string, string> corMenu { get; set; }

        [JsonPropertyName("CoresRodape")]
        public Dictionary<string, string> corRodape { get; set; }

        #endregion Fim| Cores

        #region Dados de Sobreposição
        //Dados diretos do sistema
        [JsonPropertyName("NomeSistema")]
        public string nomeSistema { get; set; }

        [JsonPropertyName("UrlSitePrincipal")]
        public string urlSite { get; set; }

        [JsonPropertyName("CorpoRodape")]
        public string txtFooter { get; set; }

        //Nome para definição membro
        [JsonPropertyName("UsuarioSingular")]
        public string nomeUserSingular { get; set; }

        [JsonPropertyName("UsuarioPlural")]
        public string nomeUserPlural { get; set; }

        //Dados de cadastro
        //[JsonPropertyName("CodigoCliente")]
        [JsonIgnore]
        public string codigoCliente { get; set; }

        //Fone
        [JsonPropertyName("Telefone")]
        public string fone { get; set; }

        [JsonPropertyName("UrlWhatsapp")]
        public string foneUrl { get; set; }

        //Contato
        [JsonPropertyName("PaginaFaleConosco")]
        public string faleConoscoUrl { get; set; }
        #endregion

        #region Conteúdo Home
        [JsonPropertyName("ConteudoHome1")]
        public string conteudoHome1 { get; set; }

        [JsonPropertyName("ConteudoHome2")]
        public string conteudoHome2 { get; set; }

        [JsonPropertyName("ConteudoHome3")]
        public string conteudoHome3 { get; set; }

        [JsonPropertyName("ConteudoHome4")]
        public string conteudoHome4 { get; set; }

        [JsonPropertyName("ConteudoHome5")]
        public string conteudoHome5 { get; set; }/////////////

        [JsonPropertyName("ConteudoHome6")]
        public string conteudoHome6 { get; set; }

        [JsonPropertyName("ConteudoHome7")]
        public string conteudoHome7 { get; set; }

        [JsonPropertyName("ConteudoHome8")]
        public string conteudoHome8 { get; set; }

        [JsonPropertyName("ConteudoHome9")]
        public string conteudoHome9 { get; set; }

        [JsonPropertyName("ConteudoHome10")]
        public string conteudoHome10 { get; set; }

        [JsonPropertyName("ConteudoHome11")]
        public string conteudoHome11 { get; set; }

        [JsonPropertyName("ConteudoHome12")]
        public string conteudoHome12 { get; set; }

        [JsonPropertyName("ConteudoHome13")]
        public string conteudoHome13 { get; set; }

        [JsonPropertyName("ConteudoHome14")]
        public string conteudoHome14 { get; set; }

        [JsonPropertyName("ConteudoHome15")]
        public string conteudoHome15 { get; set; }

        [JsonPropertyName("ConteudoHome16")]
        public string conteudoHome16 { get; set; }

        [JsonPropertyName("ConteudoHome17")]
        public string conteudoHome17 { get; set; }

        [JsonPropertyName("ConteudoHome18")]
        public string conteudoHome18 { get; set; }

        [JsonPropertyName("ConteudoHome19")]
        public string conteudoHome19 { get; set; }

        [JsonPropertyName("ConteudoHome20")]
        public string conteudoHome20 { get; set; }


        #endregion FIM| Conteúdo



        //Status de Usuário
        [JsonPropertyName("StatusUsuario")]
        public Dictionary<string, bool> statusUser { get; private set; }

        //Definições do Sistema
        [JsonPropertyName("QuantidadeUsuariosPorPagina")]
        public int qtdPPagUser { get; set; }

        [JsonPropertyName("CaminhoArquivos")]
        public string pathArquivos { get; private set; }

        [JsonPropertyName("EmailPrincipal")]
        public string emailPrincipal { get; set; }


        //###################################################################################################
        public void SetAllDefault(bool novaConf = false) {

            string versao = "1.0.0";

            if(Versao != versao)
                Versao = versao;

            //Configurações de Envio de E-mail
            Dictionary<string, string> dadosEmailDefault = new Dictionary<string, string>(){
                {"host", "smtp.site.com.br"},//Ex: smtp.gmail.com
                {"port", "587"},//Ex: 587
                {"usuario", "user@site.com.br"},//Ex: user@domain.com.br
                {"senha", "123456789"},
                {"ssl", "true"},
            };
            if(EmailAuth == null || dadosEmailDefault.Count != EmailAuth?.Count())
                EmailAuth = dadosEmailDefault;

            try {
                Convert.ToBoolean(EmailAuth["ssl"]);
            } catch {
                EmailAuth["ssl"] = "true";
            }

            if(PrazoConfirmacaoCadastro == 0)
                PrazoConfirmacaoCadastro = 72;

            if(corpoNovoCadastro == null)
                corpoNovoCadastro = "<p>&nbsp;</p>";

            if(corpoRecuperarSenha == null)
                corpoRecuperarSenha = "<p>&nbsp;</p>";

            if(corpoAlterarEmail == null)
                corpoAlterarEmail = "<p>&nbsp;</p>";

            if(rodapeEmail == null)
                rodapeEmail = $"<p>&nbsp;</p>";

            Dictionary<string, string> corMenuDefault = new Dictionary<string, string>() {
                {"bg", "#FFD011"},//Background Menu
                {"title", "#336699"},//Cor título Menu
                {"titleHover", "#e6f4ff"},//Cor Hover do título menu
                {"item", "#336699"},//Cor dos itens
                {"itemHover", "#fff"},
                {"icone", "#336699"},//cor dos ícones
                {"iconeAtivo", "#fff"},
                {"iconeHover", "#fff"},
                {"iconeDropMenu", "rgba(255,255,255,.7)"},
                {"iconeDropMenuBorda", "rgba(255,255,255,.5)"},//borda
                {"badge", "rgba(255,255,255,.5)"},
                {"badgeNumero", "rgba(255,255,255,.5)"}
            };
            //Cores
            if(corMenu == null || corMenuDefault.Count() != corMenu?.Count())
                corMenu = corMenuDefault;

            Dictionary<string, string> corRodapeDefault = new Dictionary<string, string>() {
          {"bg", "#222d31"},
          {"texto", "white"}
        };

            if(corRodape == null || corMenuDefault.Count != corMenu?.Count())
                corRodape = corRodapeDefault;

            #region Dados de Sobreposição
            //Dados diretos do sistema
            if(nomeSistema == null)
                nomeSistema = "SUR";

            if(urlSite == null)
                urlSite = "https://site.com.br";
            if(txtFooter == null)
                txtFooter = "<p></p>";

            //Nome para definição membro
            if(nomeUserSingular == null)
                nomeUserSingular = "Usuário";//Usuário
            if(nomeUserPlural == null)
                nomeUserPlural = "Usuários";//Usuários

            //Dados de cadastro
            if(novaConf)
                codigoCliente = "-1";

            //Fone
            if(fone == null)
                fone = "(11) 9.9999-9999";
            if(foneUrl == null)
                foneUrl = "https://api.whatsapp.com/send?1=pt_BR&phone=5511999999999";

            //Contato
            if(faleConoscoUrl == null)
                faleConoscoUrl = "https://api.whatsapp.com/send?1=pt_BR&phone=5511999999999";
            #endregion



            if(conteudoHome1 == null)
                conteudoHome1 = "<div class=\"text-center\"><p class=\"text-success\"></p></div>";
            if(conteudoHome2 == null)
                conteudoHome2 = "";
            if(conteudoHome3 == null)
                conteudoHome3 = "";
            if(conteudoHome4 == null)
                conteudoHome4 = "";
            if(conteudoHome5 == null)
                conteudoHome5 = "";
            if(conteudoHome6 == null)
                conteudoHome6 = "";
            if(conteudoHome7 == null)
                conteudoHome7 = "";
            if(conteudoHome8 == null)
                conteudoHome8 = "";
            if(conteudoHome9 == null)
                conteudoHome9 = "";
            if(conteudoHome10 == null)
                conteudoHome10 = "";
            if(conteudoHome11 == null)
                conteudoHome11 = "";
            if(conteudoHome12 == null)
                conteudoHome12 = "";
            if(conteudoHome13 == null)
                conteudoHome13 = "";
            if(conteudoHome14 == null)
                conteudoHome14 = "";
            if(conteudoHome15 == null)
                conteudoHome15 = "";
            if(conteudoHome16 == null)
                conteudoHome16 = "";
            if(conteudoHome17 == null)
                conteudoHome17 = "";
            if(conteudoHome18 == null)
                conteudoHome18 = "";
            if(conteudoHome19 == null)
                conteudoHome19 = "";
            if(conteudoHome20 == null)
                conteudoHome20 = "";


            Dictionary<string, bool> statusUserDefault = new Dictionary<string, bool>(){
                {"Ativo", true},
                {"Inativo", false},
                {"Apto", false},
                {"Inapto", false},
                {"Observacao", false},
                {"Aguardando", false},
                {"Suspenso", false},
                {"Excluido", false},
                {"Indisciplinado", false},
                {"Inadimplente", false},
                {"Invalido", false}
            };

            //Status de Usuário
            if(statusUser == null || !statusUser.Equals(statusUserDefault))
                statusUser = statusUserDefault;


            //Definições do Sistema
            if(qtdPPagUser == 0)
                qtdPPagUser = 15;//Quantidade de resultados na página de pesquisar membros
            if(pathArquivos == null)
                pathArquivos = Path.Combine(Environment.CurrentDirectory, "StaticFiles", "wwwroot", "Files");
            if(emailPrincipal == null)
                emailPrincipal = "site@site.com.br";


        }

        //################################################################################################################
        public string GetJsonString() {
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });

            return json;
        }

        //################################################################################################################
        public async Task SaveChanges() {
            string json = GetJsonString();

            StreamWriter fileW = new StreamWriter(PathConfig, false, Encoding.UTF8);
            await fileW.WriteAsync(json);
            await fileW.DisposeAsync();
        }

    }
}