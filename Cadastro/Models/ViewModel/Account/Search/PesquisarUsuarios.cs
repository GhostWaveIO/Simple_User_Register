using Cadastro.Data;
using Cadastro.Models.Account.Cadastro.Campos;
using Cadastro.Models.Account.Cadastro.Dados;
using Cadastro.Models.Entities.DB.Account;
using Cadastro.Models.Services.Application.Security.Autorizacao;
using Cadastro.Models.ViewModel.Application.AccessContext;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cadastro.Models.ViewModel.Account.Search
{
    public class PesquisarUsuarios
    {

        public AppDbContext context { get; set; }
        public AcessoServiceVM acesso { get; set; }

        public PesquisarUsuarios() { }

        public PesquisarUsuarios(AppDbContext context, AcessoServiceVM acesso)
        {
            this.context = context;
            this.acesso = acesso;
        }

        public PesquisarUsuarios(AppDbContext context, AcessoServiceVM acesso, string pesquisa, int? hierarquiaId, int? pagina)
        {
            this.context = context;
            this.acesso = acesso;
            Pesquisa = pesquisa;
            HierarquiaId = hierarquiaId;
            Pagina = pagina ?? 0;
        }

        public PesquisarUsuarios(string pesquisa, int? hierarquiaId, int? pagina)
        {
            Pesquisa = pesquisa;
            HierarquiaId = hierarquiaId;
            Pagina = pagina ?? 0;
        }

        public int QtdResultado { get; private set; }
        public string Pesquisa { get; set; }
        public int Pagina { get; set; }
        public int? HierarquiaId { get; set; }

        //####################################################################################################
        public async Task<List<Usuario>> GetResultado()
        {
            //Remover usuários não validados

            foreach (Usuario us in await context.Usuarios.ToListAsync())
                await us.VerificarLimiteValidacao();

            List<Usuario> res = new List<Usuario>(
              await context.Usuarios.
              Include(u => u.Dados).ThenInclude(d => d.Campo).
              Include(u => u.Dados).ThenInclude(d => d.Selected).ThenInclude(s => s.Select_Item).
              Include(u => u.Dados).ThenInclude(d => d.CheckBoxes).ThenInclude(c => c.CheckBox_Item).
              Include(u => u.Dados).ThenInclude(d => d.RadioButton_Checked).ThenInclude(r => r.RadioButton_Item).
              Include(u => u.HierarquiasUsuario).ThenInclude(hu => hu.Hierarquia).ThenInclude(h => h.PermissaoHierarquia).
              AsNoTracking().
              OrderByDescending(u => u.DataCadastro).
              ToListAsync()
            );
            res.RemoveAll(r => r == null);
            res.RemoveAll(r => r.IsAdmin());

            res = await GetPesquisa(res);
            res = FiltrarUsuariosPorHierarquia(res);

            QtdResultado = res.Count();

            //Aproveita somente a quantidade de itens por página de acordo com a página atual
            res.RemoveAll(r => r == null);
            res = res.Distinct().Skip(Pagina * Program.Config.qtdPPagUser).
              Take(Program.Config.qtdPPagUser).ToList();

            await context.DisposeAsync();

            return res;
        }



        //####################################################################################################
        private async Task<List<Usuario>> GetPesquisa(List<Usuario> resultado)
        {
            if (string.IsNullOrWhiteSpace(Pesquisa)) return resultado;

            string[] palavras = Pesquisa.Split(' ');
            List<long> res;
            List<long> ids = null;
            IQueryable<Dado_Cadastro> dadosResult;

            string pesquisa;
            bool modeloMatricula = false;
            if (Regex.IsMatch(Pesquisa.Trim().ToUpper().Replace('-', '/').Replace('\\', '/').Replace('_', '/'), "^[0-9]{4}[0-9A-Z]{3}[/]{1}[A-Z]{2}$"))
                modeloMatricula = true;

            if (modeloMatricula)
            {
                pesquisa = Pesquisa.Trim().ToUpper().Replace('-', '/').Replace('\\', '/').Replace('_', '/');
                return resultado.Where(u => Regex.IsMatch(u.CodigoUsuario, pesquisa)).ToList();
            }
            else
            {
                res = new List<long>();
                ids = await context.DadosCadastro.Include(d => d.Campo).
                    Where(d => d.Campo.ModeloCampo == EModeloCampo.Texto_250 || d.Campo.ModeloCampo == EModeloCampo.Email).
                    Where(d => d.Campo.PermitirPesquisa && !d.Campo.Criptografar && (d.Texto250.Trim().ToLower() == Pesquisa.Trim().ToLower() && d.Campo.ModeloCampo == EModeloCampo.Texto_250 || d.Email.Trim().ToLower() == Pesquisa.Trim().ToLower() && d.Campo.ModeloCampo == EModeloCampo.Email)).Select(d => d.UsuarioId).ToListAsync();
                if (ids.Any())
                    res.AddRange(ids);
                foreach (string pla in palavras)
                {
                    if (string.IsNullOrWhiteSpace(pla)) continue;
                    dadosResult = context.DadosCadastro.Include(d => d.Campo).
                      Where(d => d.Campo.ModeloCampo == EModeloCampo.Texto_250 || d.Campo.ModeloCampo == EModeloCampo.Email)
                      .Where(d => d.Campo.PermitirPesquisa && !d.Campo.Criptografar);
                    foreach (Dado_Cadastro dt in await dadosResult.ToListAsync())
                    {
                        if (dt.GetModeloCampo == EModeloCampo.Texto_250 && dt.GetCampoGenerico != ECampoGenerico.CPF && Regex.IsMatch(dt.Texto250.Trim().ToLower(), pla.Trim().ToLower())
                            || dt.GetModeloCampo == EModeloCampo.Texto_250 && dt.GetCampoGenerico == ECampoGenerico.CPF && Regex.IsMatch(dt.Texto250.Replace(".", "").Replace("-", "").Trim().ToLower(), pla.Replace(".", "").Replace("-", "").Trim().ToLower())
                            || dt.GetModeloCampo == EModeloCampo.Email && Regex.IsMatch(dt.Email.Trim().ToLower(), pla.Trim().ToLower()))
                        {
                            ids.Add(dt.UsuarioId);
                        }
                    }
                    foreach (Usuario us in resultado)
                    {
                        if (Regex.IsMatch(us.CodigoUsuario.Trim().ToUpper(), pla.Trim().ToUpper()))
                            ids.Add(us.UsuarioId);
                    }

                    if (ids.Any())
                        res.AddRange(ids);
                }
            }

            return resultado.Where(r => res.Any(u => u == r.UsuarioId)).ToList();
        }

        //####################################################################################################
        private List<Usuario> FiltrarUsuariosPorHierarquia(List<Usuario> resultado)
        {
            //if(acesso._permissao.PesquisarFiltroIndividual) return resultado;

            if (!resultado.Any()) return new List<Usuario>();
            if (HierarquiaId == null) return resultado;//Se não foi informado o filtro de Hierarquia, retornar lista

            List<Usuario> res;

            //Retorna os sem Hierarquia
            if (HierarquiaId == -1)
            {
                res = resultado.
                  Where(r => !r.HierarquiasUsuario.Any()).ToList();//Quando não for encontrado Hierarquia, ele não está em nenhuma Hierarquia
            }
            else
            {
                res = resultado.
                  Where(r => r.HierarquiasUsuario.Any(h => h.HierarquiaId == HierarquiaId && (!h.Hierarquia.PermissaoHierarquia.SomenteAdminPrincipal || acesso.IsAdmin()))).ToList();
            }

            return res;
        }

        //####################################################################################################
        public async Task<SelectList> GetSelectListHierarquias()
        {
            if (acesso == null) throw new Exception("Acesso não informado");

            List<Hierarquia> niveis = await context.Hierarquias.Include(h => h.PermissaoHierarquia).Where(h => !h.PermissaoHierarquia.SomenteAdminPrincipal || acesso.IsAdmin()).ToListAsync();
            niveis.Add(new Hierarquia() { Titulo = "--Sem Permissão--", HierarquiaId = -1 });
            SelectList listaHierarquias = new SelectList(niveis, "HierarquiaId", "Titulo");

            foreach (SelectListItem i in listaHierarquias)
            {
                if (i.Value == HierarquiaId.ToString())
                    i.Selected = true;
            }

            return listaHierarquias;
        }

        //####################################################################################################
        public void CopyTo(PesquisarUsuarios pesquisa)
        {
            pesquisa.QtdResultado = QtdResultado;
            pesquisa.Pesquisa = Pesquisa;
            pesquisa.Pagina = Pagina;
            pesquisa.HierarquiaId = HierarquiaId;
        }
    }
}
