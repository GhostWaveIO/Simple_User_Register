using Cadastro.Data;
using Cadastro.Models.Entities.DB.Account;
using Cadastro.Models.Services.Application.Security.Autorizacao;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cadastro.Models.ViewModel.Application.AccessContext
{
    public class AcessoServiceVM
    {

        public AppDbContext _context { get; set; }
        private HttpContext _sessao { get; set; }

        public Usuario _usuario { get; private set; }
        public PermissaoUsuario _permissao { get; private set; }//Permissão do usuário
        public long? UsuarioId { get; private set; }

        public AcessoServiceVM(AppDbContext context, long usuarioId)
        {
            _context = context;
            UsuarioId = usuarioId;
        }

        public AcessoServiceVM(AppDbContext context, HttpContext sessao)
        {
            _context = context;
            _sessao = sessao;
            UsuarioId = sessao.Session.GetInt32("UsuarioId");
        }

        public async Task Consultar()
        {
            _usuario = await ConsultaUsuario();
            if (_usuario == null) return;
            _permissao = await DefinePermissoes();
        }

        //Retorna o usuário da sessão
        public async Task<Usuario> ConsultaUsuario()
        {

            // ####  SOMENTE PARA TESTES  ####
            //_sessao.Session.SetInt32("UsuarioId", (int)_context.Usuarios.First().UsuarioId);
            //_sessao.Session.SetInt32("UsuarioId", (int)_context.Usuarios.First(u => u.Nome.StartsWith("pa")).UsuarioId);

            if (UsuarioId == null) return null;

            Usuario res = await _context.Usuarios.
              Include(u => u.Dados).ThenInclude(d => d.Campo).
              Include(u => u.Dados).ThenInclude(d => d.Selected).ThenInclude(s => s.Select_Item).
              Include(u => u.Dados).ThenInclude(d => d.CheckBoxes).ThenInclude(c => c.CheckBox_Item).
              Include(u => u.Dados).ThenInclude(d => d.RadioButton_Checked).ThenInclude(r => r.RadioButton_Item).
              Include(u => u.HierarquiasUsuario).ThenInclude(hu => hu.Hierarquia).ThenInclude(h => h.PermissaoHierarquia).
              FirstOrDefaultAsync(u => u.UsuarioId == UsuarioId);

            return res;
        }

        //################################################################################################################
        public bool IsAdmin()
        {
            return _usuario.IsAdmin();
        }

        //################################################################################################################
        //Retorna permissão de acordo em todas hierarquia e Permissões do usuário
        public async Task<PermissaoUsuario> DefinePermissoes()
        {
            if (_usuario == null) return new PermissaoUsuario();
            PermissaoUsuario res = new PermissaoUsuario();
            List<Permissao> listaPermissoes = new List<Permissao>();
            foreach (HierarquiaUsuario hu in await Task.FromResult(_usuario.HierarquiasUsuario))
            {
                listaPermissoes.Add(hu.Hierarquia.PermissaoHierarquia);
            }
            return AtribuirTrue(res, listaPermissoes);
        }

        //################################################################################################################
        //Retorna as propriedades que forem true de acordo com a hierarquia ou função
        public PermissaoUsuario AtribuirTrue(PermissaoUsuario res, List<Permissao> p)
        {
            //Admin geral tem Acesso Total
            if (_usuario.IsAdmin()) return AcessoTotal();

            if (res == null) res = new PermissaoUsuario();

            foreach (Permissao pm in p)
            {
                if (pm == null) continue;

                if (pm.AcessoTotal) return AcessoTotal();
                //Home
                if (pm.UltimosEventos) res.UltimosEventos = true;
                if (pm.ContagemUsuarios) res.ContagemUsuarios = true;
                if (pm.ConteudoHome) res.ConteudoHome = true;
                if (pm.ConteudoHome2) res.ConteudoHome2 = true;
                if (pm.ConteudoHome3) res.ConteudoHome3 = true;
                if (pm.ConteudoHome4) res.ConteudoHome4 = true;
                if (pm.ConteudoHome5) res.ConteudoHome5 = true;
                if (pm.ConteudoHome6) res.ConteudoHome6 = true;
                if (pm.ConteudoHome7) res.ConteudoHome7 = true;
                if (pm.ConteudoHome8) res.ConteudoHome8 = true;
                if (pm.ConteudoHome9) res.ConteudoHome9 = true;
                if (pm.ConteudoHome10) res.ConteudoHome10 = true;
                if (pm.ConteudoHome11) res.ConteudoHome11 = true;
                if (pm.ConteudoHome12) res.ConteudoHome12 = true;
                if (pm.ConteudoHome13) res.ConteudoHome13 = true;
                if (pm.ConteudoHome14) res.ConteudoHome14 = true;
                if (pm.ConteudoHome15) res.ConteudoHome15 = true;
                if (pm.ConteudoHome16) res.ConteudoHome16 = true;
                if (pm.ConteudoHome17) res.ConteudoHome17 = true;
                if (pm.ConteudoHome18) res.ConteudoHome18 = true;
                if (pm.ConteudoHome19) res.ConteudoHome19 = true;
                if (pm.ConteudoHome20) res.ConteudoHome20 = true;
                //Usuários
                if (pm.PesquisarUsuarios) res.PesquisarUsuarios = true;
                if (pm.PesquisarFiltroIndividual) res.PesquisarFiltroIndividual = true;
                if (pm.VisualizarDadosUsuarios) res.VisualizarDadosUsuarios = true;
                if (pm.EditarDadosUsuarios) res.EditarDadosUsuarios = true;
                if (pm.EditarHierarquiaUsuario) res.EditarHierarquiaUsuario = true;
                if (pm.DarFuncoesUsuarios) res.DarFuncoesUsuarios = true;
                if (pm.ApagarUsuario) res.ApagarUsuario = true;
                //Estrutura de Cadastro
                if (pm.CriarGrupoCadastro) res.CriarGrupoCadastro = true;
                if (pm.EditarGrupoCadastro) res.EditarGrupoCadastro = true;
                if (pm.RemoverGrupoCadastro) res.RemoverGrupoCadastro = true;
                if (pm.CriarLinhaCadastro) res.CriarLinhaCadastro = true;
                if (pm.EditarLinhaCadastro) res.EditarLinhaCadastro = true;
                if (pm.RemoverLinhaCadastro) res.RemoverLinhaCadastro = true;
                if (pm.CriarCampoCadastro) res.CriarCampoCadastro = true;
                if (pm.EditarCampoGenericoCadastro) res.EditarCampoGenericoCadastro = true;
                if (pm.EditarCampoPersonalizadoCadastro) res.EditarCampoPersonalizadoCadastro = true;
                if (pm.RemoverCampoCadastro) res.RemoverCampoCadastro = true;
                if (pm.VisualizarEstruturaCadastro) res.VisualizarEstruturaCadastro = true;
                
                //Configurações
                if (pm.ManipularHierarquias) res.ManipularHierarquias = true;
                if (pm.VerBotaoSuporte) res.VerBotaoSuporte = true;
                if (pm.SomenteAdminPrincipal) res.SomenteAdminPrincipal = true;
            }

            return res;
        }

        //################################################################################################################
        //retorna todas propriedades como true
        public PermissaoUsuario AcessoTotal()
        {

            PermissaoUsuario u = new PermissaoUsuario();

            u.AcessoTotal = true;
            //Home
            u.UltimosEventos = true;
            u.ContagemUsuarios = true;
            u.ConteudoHome = true;
            u.ConteudoHome2 = true;
            u.ConteudoHome3 = true;
            u.ConteudoHome4 = true;
            u.ConteudoHome5 = true;
            u.ConteudoHome6 = true;
            u.ConteudoHome7 = true;
            u.ConteudoHome8 = true;
            u.ConteudoHome9 = true;
            u.ConteudoHome10 = true;
            u.ConteudoHome11 = true;
            u.ConteudoHome12 = true;
            u.ConteudoHome13 = true;
            u.ConteudoHome14 = true;
            u.ConteudoHome15 = true;
            u.ConteudoHome16 = true;
            u.ConteudoHome17 = true;
            u.ConteudoHome18 = true;
            u.ConteudoHome19 = true;
            u.ConteudoHome20 = true;
            //Usuários
            u.PesquisarUsuarios = true;
            u.PesquisarFiltroIndividual = true;
            u.VisualizarDadosUsuarios = true;
            u.EditarDadosUsuarios = true;
            u.EditarHierarquiaUsuario = true;
            u.DarFuncoesUsuarios = true;
            u.ApagarUsuario = true;
            //Estrutura de cadastro
            u.CriarGrupoCadastro = true;
            u.EditarGrupoCadastro = true;
            u.RemoverGrupoCadastro = true;
            u.CriarLinhaCadastro = true;
            u.EditarLinhaCadastro = true;
            u.RemoverLinhaCadastro = true;
            u.CriarCampoCadastro = true;
            u.EditarCampoGenericoCadastro = true;
            u.EditarCampoPersonalizadoCadastro = true;
            u.RemoverCampoCadastro = true;
            u.VisualizarEstruturaCadastro = true;
            //Configurações
            u.ManipularHierarquias = true;
            u.VerBotaoSuporte = true;
            u.SomenteAdminPrincipal = true;

            return u;
        }

    }
}
