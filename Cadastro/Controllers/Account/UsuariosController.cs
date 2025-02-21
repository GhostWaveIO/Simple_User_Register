using Cadastro.Data;
using Cadastro.Models.Account.Cadastro.Campos;
using Cadastro.Models.Account.Cadastro.Dados;
using Cadastro.Models.Entities.DB.Account;
using Cadastro.Models.Entities.DB.Account.Form;
using Cadastro.Models.Services.Application.Security.Autorizacao;
using Cadastro.Models.ViewModel.Account;
using Cadastro.Models.ViewModel.Account.Cadastro;
using Cadastro.Models.ViewModel.Account.Search;
using Cadastro.Models.ViewModel.Application.AccessContext;
using Cadastro.Models.ViewModel.Application.Notifications.Alerts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cadastro.Controllers.Account {
    public class UsuariosController : Controller {

        public readonly AppDbContext _context;
        public UsuariosController(AppDbContext context) {
            _context = context;
        }

        //################################################################################################
        [HttpGet("Usuarios/Index/")]
        [HttpPost("Usuarios/Index/{request?}/")]
        public async Task<IActionResult> Index(PesquisarUsuarios request = null) {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar();
            ViewData["acesso"] = acesso;
            if(acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if(!acesso._permissao.PesquisarUsuarios)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });
            #endregion

            if(request == null)
                request = new PesquisarUsuarios();

            request.context = _context;
            request.acesso = acesso;

            //Gera Selectlist de fatura vencida
            SelectList SLFaturaVencida = new SelectList(new List<SelectListItem>(){
                new SelectListItem("Vencida", true.ToString()),
                new SelectListItem("Em dia", false.ToString())
            }, "Value", "Text");

            ViewData["UserVM"] = new UsuariosVM();
            ViewData["SLHierarquias"] = await request.GetSelectListHierarquias();
            ViewData["SLFaturaVencida"] = SLFaturaVencida;


            return View("PesquisarMembros", request);
        }

        //################################################################################################
        public async Task<IActionResult> DadosUsuario(long usuarioResult, string msgResult) {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar();
            ViewData["acesso"] = acesso;
            if(acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if(!acesso._permissao.VisualizarDadosUsuarios)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });
            if(usuarioResult == 0)
                return RedirectToAction("Index", "Home", new UsuarioVM() { msgResult = AlertMessage.Danger("Erro: Usuário inválido!") });
            #endregion

            Usuario usuario = await _context.Usuarios.
              Include(u => u.Dados).ThenInclude(d => d.Campo).
              Include(u => u.Dados).ThenInclude(d => d.Selected).ThenInclude(s => s.Select_Item).
              Include(u => u.Dados).ThenInclude(d => d.RadioButton_Checked).ThenInclude(r => r.RadioButton_Item).
              Include(u => u.Dados).ThenInclude(d => d.CheckBoxes).ThenInclude(c => c.CheckBox_Item).
              Include(u => u.EmailsValidados).
              FirstOrDefaultAsync(u => u.UsuarioId == usuarioResult);

            List<Grupo_Cadastro> grupos = await _context.GruposCadastro.
              Include(u => u.Linhas).ThenInclude(l => l.Campos).ThenInclude(c => c.Selects).
              Include(u => u.Linhas).ThenInclude(l => l.Campos).ThenInclude(c => c.CheckBoxes).
              Include(u => u.Linhas).ThenInclude(l => l.Campos).ThenInclude(c => c.RadioButtons).
              ToListAsync();

            if(usuario == null || (usuario?.IsAdmin() ?? true))
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Usuário não encontrado!") });

            //Correções
            grupos = grupos.OrderBy(g => g.Ordem).ToList();
            foreach(Grupo_Cadastro grupo in grupos) {
                grupo.Linhas = grupo.Linhas.OrderBy(l => l.Ordem).ToList();
                foreach(Linha_Cadastro linha in grupo.Linhas) {
                    linha.Campos = linha.Campos.OrderBy(c => c.Ordem_Perfil).ToList();
                }
            }
            //Descriptografar dados criptografados
            foreach(Dado_Cadastro dd in usuario.Dados) {
                if(dd.Campo.Criptografar)
                    dd.Descriptografar();
            }

            //Níveis que o usuário está
            List<HierarquiaUsuario> listaHu = (await _context.Usuarios.
              Include(u => u.HierarquiasUsuario).ThenInclude(hu => hu.Hierarquia).ThenInclude(h => h.PermissaoHierarquia).
              Include(u => u.HierarquiasUsuario).ThenInclude(hu => hu.Hierarquia).ThenInclude(h => h.HierarquiasUsuario).
              FirstOrDefaultAsync(u => u.UsuarioId == usuario.UsuarioId)).HierarquiasUsuario.Where(h => !h.Hierarquia.PermissaoHierarquia.SomenteAdminPrincipal || acesso.IsAdmin()).ToList();
            List<Usuario> listaIndicoes = null;


            ViewData["NiveisDoUsuario"] = listaHu;
            ViewData["grupos"] = grupos;
            ViewData["usuario"] = usuario;
            ViewData["listaIndicacoes"] = listaIndicoes;

            //Lista de níveis que o usuário não possui
            UsuarioVM usSe = new UsuarioVM(_context);
            usSe.listaHierarquias = new List<Hierarquia>();
            //usSe.usuario = usuario;
            foreach(Hierarquia h in new List<Hierarquia>(await _context.Hierarquias?.Include(h => h.HierarquiasUsuario).Include(hu => hu.PermissaoHierarquia).Where(h => !h.PermissaoHierarquia.SomenteAdminPrincipal || acesso.IsAdmin()).ToListAsync() ?? new List<Hierarquia>())) {
                //filtra os níveis que o usuário já está
                if(!await usSe.ConsultarHierarquiaUsuario(usuario, h))
                    usSe.listaHierarquias.Add(h);
            }

            usSe.msgResult = AlertMessage.CorrigeUrl(msgResult);
            if(usuario == null || (usuario?.IsAdmin() ?? false))
                return RedirectToAction("IndexResult", "Login", new { msgResult = AlertMessage.Danger("Erro: Usuário inválido!") });

            await _context.DisposeAsync();
            GC.Collect();

            return View(usSe);
        }

        //################################################################################################
        [HttpPost]
        public async Task<IActionResult> AlterarHierarquia(UsuarioVM usSe) {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar();
            ViewData["acesso"] = acesso;
            if(acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if(!acesso._permissao.EditarHierarquiaUsuario)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });
            #endregion

            if(usSe.hierarquia?.HierarquiaId == null || usSe.hierarquia?.HierarquiaId == -1) {
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Info("Erro ao Ingressar nível!") });
            }
            usSe._context = _context;

            string msgResult = string.Empty;
            Usuario usuario = await _context.Usuarios?.
              Include(u => u.HierarquiasUsuario).ThenInclude(hu => hu.Hierarquia).
              FirstOrDefaultAsync(u => u.UsuarioId == usSe.usuario.UsuarioId);

            //Atribuir entidade da hierarquia
            Hierarquia hierarquia = await _context.Hierarquias.Include(h => h.PermissaoHierarquia).FirstOrDefaultAsync(h => h.HierarquiaId == usSe.hierarquia.HierarquiaId);
            //Verifica se esta hierarquia está disponível para o operador
            if(hierarquia.PermissaoHierarquia.SomenteAdminPrincipal && !acesso.IsAdmin())
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });

            HierarquiaUsuario hiUs = new HierarquiaUsuario() { UsuarioId = usuario.UsuarioId, HierarquiaId = usSe.hierarquia.HierarquiaId };

            try {

                if(await usSe.ConsultarHierarquiaUsuario(usuario, usSe.hierarquia))
                    throw new Exception("Este usuário já foi ingressado nesta permissão.");

                //Insere nível ao usuário
                await _context.HierarquiasUsuario.AddAsync(hiUs);
                await _context.SaveChangesAsync();

                msgResult = AlertMessage.Success("A permissão foi adicionada a este usuário.");
            } catch(Exception err) {
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Erro: " + err.Message) });
            }

            return RedirectToAction("DadosUsuario", "Usuarios", new {
                usSe.pesquisa,
                pagina = usSe.pagPesquisa,
                usuarioResult = usSe.usuario.UsuarioId,
                msgResult
            }
            );
        }

        //################################################################################################
        public async Task<IActionResult> RemoverNivelHierarquia(long usuarioResult, uint huId) {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar();
            ViewData["acesso"] = acesso;
            if(acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if(!acesso._permissao.EditarHierarquiaUsuario)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });
            #endregion

            HierarquiaUsuario hu = await _context.HierarquiasUsuario?.
              Include(hu => hu.Hierarquia).ThenInclude(h => h.PermissaoHierarquia).
              FirstOrDefaultAsync(hus => hus.HierarquiaUsuarioId == huId);
            hu.Hierarquia.context = _context;
            if(hu == null)
                return RedirectToAction("DadosUsuario", "Usuarios", new { usuarioResult, msgResult = AlertMessage.Danger("Erro ao tentar remover permissão!") });

            if(hu.Hierarquia.PermissaoHierarquia.SomenteAdminPrincipal && !acesso.IsAdmin())
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });

            //Consulta Usuário
            Usuario usuario = await _context.Usuarios.
              FirstOrDefaultAsync(u => u.UsuarioId == usuarioResult);

            _context.HierarquiasUsuario.Remove(hu);
            await _context.SaveChangesAsync();

            return RedirectToAction("DadosUsuario", "Usuarios", new { usuarioResult, msgResult = AlertMessage.Success("A permissão foi removido!") });
        }

        //##################################################################################################
        public async Task<IActionResult> DefinirStatus(long uRes, EStatus status) {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar();
            ViewData["acesso"] = acesso;
            if(acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if(!acesso._permissao.EditarDadosUsuarios)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });
            #endregion

            Usuario usuario = await _context.Usuarios?.FirstOrDefaultAsync(u => u.UsuarioId == uRes);
            if(usuario == null)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Este usuário não foi encontrado!") });

            usuario.Status = status;
            await _context.SaveChangesAsync();

            return RedirectToAction("DadosUsuario", "Usuarios", new { UsuarioResult = uRes, msgResult = AlertMessage.Success("Status definido!") });
        }



        //################################################################################################
        public async Task<IActionResult> Perfil(long? usuarioResult, string msgResult) {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar();
            ViewData["acesso"] = acesso;
            if(usuarioResult == null && acesso._usuario != null)
                usuarioResult = acesso._usuario.UsuarioId;
            if(acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if(acesso._usuario.UsuarioId != usuarioResult && !acesso._permissao.EditarDadosUsuarios)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });
            #endregion

            //Preenche os dados do usuário
            Usuario usuario = await _context.Usuarios?.
              Include(u => u.Dados).ThenInclude(d => d.Campo).
              Include(u => u.Dados).ThenInclude(d => d.Selected).ThenInclude(s => s.Select_Item).
              Include(u => u.Dados).ThenInclude(d => d.CheckBoxes).ThenInclude(c => c.CheckBox_Item).
              Include(u => u.Dados).ThenInclude(d => d.RadioButton_Checked).ThenInclude(r => r.RadioButton_Item).
              FirstOrDefaultAsync(u => u.UsuarioId == usuarioResult);
            usuario._context = _context;

            CadastroUsuarioVM cuSe = new CadastroUsuarioVM();
            cuSe.usuario = usuario;

            cuSe.grupos = (await usuario.GetGruposEReferenciasAsync()).
              Where(g => g.Linhas.Any(l => l.Campos.Any(c => c.Ativo && c.ModeloCampo != EModeloCampo.Html || c.Ativo && c.ModeloCampo == EModeloCampo.Html && c.StartCriacaoCampo == EStartCriacaoCampo.Perfil || c.Generico))).
                Where(g => g.Linhas.Any(l => l.Campos.Any(c => usuario.IsAdmin() && c.CampoGenerico != ECampoGenerico.Email || !usuario.IsAdmin()))).
              OrderBy(g => g.Ordem).ToList();
            foreach(Grupo_Cadastro grupo in cuSe.grupos) {
                grupo.Linhas = grupo.Linhas.
                  Where(l => l.Campos.Any(c => c.Ativo && c.ModeloCampo != EModeloCampo.Html || c.Ativo && c.ModeloCampo == EModeloCampo.Html && c.StartCriacaoCampo == EStartCriacaoCampo.Perfil || c.Generico)).
                  Where(l => l.Campos.Any(c => usuario.IsAdmin() && c.CampoGenerico != ECampoGenerico.Email || !usuario.IsAdmin())).
                  OrderBy(l => l.Ordem).ToList();
                foreach(Linha_Cadastro linha in grupo.Linhas.OrderBy(l => l.Ordem)) {
                    linha.Campos = linha.Campos.
                      Where(c => c.Ativo && c.ModeloCampo != EModeloCampo.Html || c.Ativo && c.ModeloCampo == EModeloCampo.Html && c.StartCriacaoCampo == EStartCriacaoCampo.Perfil || c.Generico).
                      Where(c => usuario.IsAdmin() && c.CampoGenerico != ECampoGenerico.Email || !usuario.IsAdmin()).
                      OrderBy(c => c.Ordem_Cadastro).ToList();
                    //Gera os dados Vazios de cada campo caso "Campo.Dados" seja nulo
                    linha.Campos.ForEach(c => c.GerarNovoDado());
                    if(linha.Campos.Any(c => c.Generico && !c.Ativo))
                        return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Danger("Campo genérico inativo!") });
                    foreach(Campo_Cadastro campo in linha.Campos) {

                        campo.Selects = campo.Selects.OrderBy(s => s.Ordem).ToList();
                        campo.CheckBoxes = campo.CheckBoxes.OrderBy(c => c.Ordem).ToList();
                        campo.RadioButtons = campo.RadioButtons.OrderBy(r => r.Ordem).ToList();
                        campo.Dados[0]._context = _context;
                        if(campo.CampoGenerico == ECampoGenerico.Email) {
                            campo.Dados[0].Email_Novo = campo.Dados[0].Email;
                        }
                        //Descriptografar dados criptografados
                        if(campo.Criptografar)
                            campo.Dados[0].Descriptografar();
                    }
                }
            }

            ViewData["msgResult"] = msgResult;
            ViewData["usuario"] = usuario;

            return View(cuSe);
        }

        //################################################################################################
        public async Task<IActionResult> SalvarPerfil([FromForm] CadastroUsuarioVM cuSe) {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar();
            ViewData["acesso"] = acesso;
            if(acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if(acesso._usuario.UsuarioId != cuSe.usuario.UsuarioId && !acesso._permissao.EditarDadosUsuarios)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });
            #endregion

            string msg = "";

            IEnumerable<Dado_Cadastro> dadosEditados = cuSe.GetListaDados();
            Dictionary<Dado_Cadastro, Dado_Cadastro> dadosExistentes = new Dictionary<Dado_Cadastro, Dado_Cadastro>();
            List<Dado_Cadastro> dadosNaoExistentes = new List<Dado_Cadastro>();
            Usuario proprietario;

            IEnumerable<Campo_Cadastro> campos = null;

            try {

                proprietario = await _context.Usuarios.Include(u => u.Dados).ThenInclude(d => d.Campo).FirstOrDefaultAsync(u => u.UsuarioId == cuSe.usuario.UsuarioId);
                proprietario.DescriptografarDados();


                campos = await _context.CamposCadastro.
                  Include(cmp => cmp.CheckBoxes).
                  Include(cmp => cmp.Selects).
                  Include(cmp => cmp.RadioButtons).
                  Where(c => (c.Ativo || c.Generico) && c.ModeloCampo != EModeloCampo.Confirmar_Senha).ToListAsync();

                Dado_Cadastro dadoDoForm = null;
                Dado_Cadastro dadoDoBD = null;
                //Verificar se Não ficou dados pendentes
                foreach(Campo_Cadastro campo in campos) {
                    if(campo.ModeloCampo == EModeloCampo.Select && !campo.Selects.Any())
                        continue;
                    if(campo.ModeloCampo == EModeloCampo.CheckBox && !campo.CheckBoxes.Any())
                        continue;
                    if(campo.ModeloCampo == EModeloCampo.RadioButton && !campo.RadioButtons.Any())
                        continue;
                    if(campo.ModeloCampo == EModeloCampo.Html)
                        continue;
                    if(campo.ModeloCampo == EModeloCampo.Vídeo_Youtube)
                        continue;
                    if(campo.CampoGenerico == ECampoGenerico.Email && proprietario.IsAdmin())
                        continue;
                    if(campo.CampoGenerico == ECampoGenerico.ConfirmarSenha)
                        continue;
                    //if(campo.Generico && !acesso._usuario.IsAdmin()) continue;//Somente Admin

                    if(dadosEditados.Count(d => d.Campo_CadastroId == campo.Campo_CadastroId) != 1)
                        throw new Exception("Ocorreu um erro ao tentar salvar seus dados. Quantidade de campos incorreto!");
                    dadoDoForm = dadosEditados.First(d => d.Campo_CadastroId == campo.Campo_CadastroId);
                    if(dadoDoForm.Campo.Label == null)
                        dadoDoForm.Campo.Label = campo.Label;
                    dadoDoForm.Campo.Generico = campo.Generico;
                    dadoDoForm.Campo.AutorizadoEditar = campo.AutorizadoEditar;
                    dadoDoForm._context = _context;
                    dadoDoBD = await _context.DadosCadastro.
                      Include(d => d.Usuario).
                      Include(d => d.Selected).ThenInclude(s => s.Select_Item).
                      Include(d => d.CheckBoxes).ThenInclude(c => c.CheckBox_Item).
                      Include(d => d.RadioButton_Checked).ThenInclude(r => r.RadioButton_Item).
                      Include(d => d.Campo).FirstOrDefaultAsync(d => d.Dado_CadastroId == dadoDoForm.Dado_CadastroId);

                    //Verifica possível fraude. Verifica se o dado deste usuário referente a este campo existe caso não tenha sido informado o id deste Dado
                    if(dadoDoBD == null && await _context.DadosCadastro.AnyAsync(d => d.UsuarioId == cuSe.usuario.UsuarioId && d.Campo_CadastroId == dadoDoForm.Campo_CadastroId))
                        throw new Exception($"Ocorreu um erro ao tentar salvar os dados do campo \"{campo.Label}\"", new Exception("O dado Existente já existe no cadastro deste usuário!"));

                    //Adiciona o dado à lista adequada (Edição ou Criação)
                    if(dadoDoBD == null) {//Se dado não existe no banco de dados
                        dadoDoForm.UsuarioId = cuSe.usuario.UsuarioId;
                        dadoDoForm.UsuarioId = cuSe.usuario.UsuarioId;
                        dadosNaoExistentes.Add(dadoDoForm);
                    } else {//Se dado existe no banco de dados
                        dadoDoBD._context = _context;
                        dadosExistentes.Add(dadoDoBD, dadoDoForm);
                    }
                }


                //Processar dados existentes
                foreach(KeyValuePair<Dado_Cadastro, Dado_Cadastro> dado in dadosExistentes) {
                    if(!await dado.Value.VerificarAutorizacaoEdicao(proprietario, acesso))
                        continue;
                    await dado.Value.VerificarEdicao(ELocalDeOperacao.Perfil);
                    dado.Value.CorrigirEdicao();
                    dado.Value.CopyToUpdate(dado.Key);
                    if(dado.Key.Campo.Criptografar)
                        dado.Key.Criptografar();
                    //Gera url de validação de email
                    dado.Key.Url = Url;
                    if(dado.Key.Campo.CampoGenerico == ECampoGenerico.Senha && !proprietario.IsAdmin()) {
                        await dado.Key.Update(proprietario.GetCpf, dadosEditados.FirstOrDefault(d => d.Campo.CampoGenerico == ECampoGenerico.Senha).Senha_Perfil);
                    } else {
                        await dado.Key.Update();
                    }
                    //Se confirmação de troca de email foi enviado, acionar mensagem de envio
                    if(dado.Key.ConfirmacaoDeEmailEnviado)
                        msg += AlertMessage.Info("Foi enviado uma solicitação de troca de email para o novo email informado, acesse seu email para confirmar!");
                }

                //Processar dados não existentes
                foreach(Dado_Cadastro dado in dadosNaoExistentes) {
                    if(!await dado.VerificarAutorizacaoEdicao(proprietario, acesso))
                        continue;
                    await dado.VerificarCriacao(ELocalDeOperacao.Perfil);
                    dado.CorrigirCriacao();
                    if(dado.Campo.Criptografar)
                        dado.Criptografar();
                    await dado.Criar();
                }

                msg += AlertMessage.Success("Alterações salvas com sucesso!");

            } catch(Exception err) {
                return RedirectToAction("Perfil", "Usuarios", new { usuarioResult = cuSe.usuario.UsuarioId, msgResult = msg + AlertMessage.Danger(err.Message) });
            }

            await _context.DisposeAsync();
            GC.Collect();

            return RedirectToAction("Perfil", "Usuarios", new { usuarioResult = cuSe.usuario.UsuarioId, msgResult = msg });
        }

        //##################################################################################################
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> VerificarAlterarEmail(CadastroUsuarioVM cuSe) {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar();
            ViewData["acesso"] = acesso;
            if(acesso._usuario == null)
                return Json($"Efetue Login.");
            #endregion

            bool res = true;

            List<Dado_Cadastro> dados = cuSe.GetListaDados();

            dados[0].Email_Novo = dados[0].Email_Novo.Trim();
            cuSe._context = _context;
            res = await cuSe.verificarEmailExists(dados[0].Email_Novo, dados[0].Email_Generico, dados[0].Dado_CadastroId_Email);

            if(res) {
                return Json($"Este email já está em uso.");
            }

            return Json(true);
        }

    }
}