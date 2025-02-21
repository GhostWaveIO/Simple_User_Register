namespace Cadastro.Models.Services.Application.Security.Autorizacao
{
    public abstract class Permissao
    {

        public bool AcessoTotal { get; set; }

        //Home
        public bool UltimosEventos { get; set; }
        public bool ContagemUsuarios { get; set; }
        public bool ConteudoHome { get; set; }
        public bool ConteudoHome2 { get; set; }
        public bool ConteudoHome3 { get; set; }
        public bool ConteudoHome4 { get; set; }
        public bool ConteudoHome5 { get; set; }
        public bool ConteudoHome6 { get; set; }
        public bool ConteudoHome7 { get; set; }
        public bool ConteudoHome8 { get; set; }
        public bool ConteudoHome9 { get; set; }
        public bool ConteudoHome10 { get; set; }
        public bool ConteudoHome11 { get; set; }
        public bool ConteudoHome12 { get; set; }
        public bool ConteudoHome13 { get; set; }
        public bool ConteudoHome14 { get; set; }
        public bool ConteudoHome15 { get; set; }
        public bool ConteudoHome16 { get; set; }
        public bool ConteudoHome17 { get; set; }
        public bool ConteudoHome18 { get; set; }
        public bool ConteudoHome19 { get; set; }
        public bool ConteudoHome20 { get; set; }


        //Usuários
        public bool PesquisarUsuarios { get; set; }
        public bool PesquisarFiltroIndividual { get; set; }
        public bool VisualizarDadosUsuarios { get; set; }
        public bool EditarDadosUsuarios { get; set; }
        public bool EditarHierarquiaUsuario { get; set; }
        public bool DarFuncoesUsuarios { get; set; }
        public bool ApagarUsuario { get; set; }

        //Estrutura de cadastro
        public bool CriarGrupoCadastro { get; set; }
        public bool EditarGrupoCadastro { get; set; }
        public bool RemoverGrupoCadastro { get; set; }
        public bool CriarLinhaCadastro { get; set; }
        public bool EditarLinhaCadastro { get; set; }
        public bool RemoverLinhaCadastro { get; set; }
        public bool CriarCampoCadastro { get; set; }
        public bool EditarCampoGenericoCadastro { get; set; }
        public bool EditarCampoPersonalizadoCadastro { get; set; }
        public bool RemoverCampoCadastro { get; set; }
        public bool VisualizarEstruturaCadastro { get; set; }


        //Configurações
        public bool ManipularHierarquias { get; set; }
        public bool VerBotaoSuporte { get; set; }
        public bool SomenteAdminPrincipal { get; set; }


        public bool AnyEstruturaCadastro()
        {
            bool res = false;

            if (CriarGrupoCadastro ||
              EditarGrupoCadastro ||
              RemoverGrupoCadastro ||
              CriarLinhaCadastro ||
              EditarLinhaCadastro ||
              RemoverLinhaCadastro ||
              CriarCampoCadastro ||
              EditarCampoGenericoCadastro ||
              EditarCampoPersonalizadoCadastro ||
              RemoverCampoCadastro ||
              VisualizarEstruturaCadastro)
                res = true;


            return res;
        }
    }
}
