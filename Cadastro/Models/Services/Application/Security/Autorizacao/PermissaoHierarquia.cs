using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cadastro.Models.Entities.DB.Account;

namespace Cadastro.Models.Services.Application.Security.Autorizacao
{
    public class PermissaoHierarquia : Permissao
    {
        public int PermissaoHierarquiaId { get; set; }

        public int HierarquiaId { get; set; }
        public Hierarquia Hierarquia { get; set; }

        public void CopyTo(PermissaoHierarquia permissao, Usuario usuario)
        {

            permissao.AcessoTotal = AcessoTotal;

            //Home
            permissao.UltimosEventos = UltimosEventos;
            permissao.ContagemUsuarios = ContagemUsuarios;
            permissao.ConteudoHome = ConteudoHome;
            permissao.ConteudoHome2 = ConteudoHome2;
            permissao.ConteudoHome3 = ConteudoHome3;
            permissao.ConteudoHome4 = ConteudoHome4;
            permissao.ConteudoHome5 = ConteudoHome5;

            //Usuários
            permissao.PesquisarUsuarios = PesquisarUsuarios;
            permissao.PesquisarFiltroIndividual = PesquisarFiltroIndividual;
            permissao.VisualizarDadosUsuarios = VisualizarDadosUsuarios;
            permissao.EditarDadosUsuarios = EditarDadosUsuarios;
            permissao.EditarHierarquiaUsuario = EditarHierarquiaUsuario;
            permissao.DarFuncoesUsuarios = DarFuncoesUsuarios;

            //Estrutura de Cadastro
            if (usuario.IsAdmin())
            {
                permissao.CriarGrupoCadastro = CriarGrupoCadastro;
                permissao.EditarGrupoCadastro = EditarGrupoCadastro;
                permissao.RemoverGrupoCadastro = RemoverGrupoCadastro;
                permissao.CriarLinhaCadastro = CriarLinhaCadastro;
                permissao.EditarLinhaCadastro = EditarLinhaCadastro;
                permissao.RemoverLinhaCadastro = RemoverLinhaCadastro;
                permissao.CriarCampoCadastro = CriarCampoCadastro;
                permissao.EditarCampoGenericoCadastro = EditarCampoGenericoCadastro;
                permissao.EditarCampoPersonalizadoCadastro = EditarCampoPersonalizadoCadastro;
                permissao.RemoverCampoCadastro = RemoverCampoCadastro;
                permissao.VisualizarEstruturaCadastro = VisualizarEstruturaCadastro;
            }

            //Configurações
            if (usuario.IsAdmin())
            {
                permissao.ManipularHierarquias = ManipularHierarquias;
                permissao.VerBotaoSuporte = VerBotaoSuporte;
                permissao.SomenteAdminPrincipal = SomenteAdminPrincipal;
            }
        }

    }
}
