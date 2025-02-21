using BrazilModels;
using Cadastro.Data;
using Cadastro.Models.Account.Cadastro.Campos;
using Cadastro.Models.Account.Cadastro.Dados;
using Cadastro.Models.Entities.DB.Account;
using Cadastro.Models.Entities.DB.Account.Form;
using Cadastro.Models.Services.Application.Network.Email;
using Cadastro.Models.ViewModel.Account;
using Cadastro.Models.ViewModel.Account.Cadastro;
using Cadastro.Models.ViewModel.Account.Cadastro.Campo;
using Cadastro.Models.ViewModel.Account.Cadastro.Grupo;
using Cadastro.Models.ViewModel.Account.Cadastro.Linha;
using Cadastro.Models.ViewModel.Application.AccessContext;
using Cadastro.Models.ViewModel.Application.Network.Email;
using Cadastro.Models.ViewModel.Application.Notifications.Alerts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
//using CadastroExclusivo;
//using CadastroExclusivo.AutoExecutaveis;

namespace Cadastro.Controllers.Account.Register {
    public class CadastroController : Controller {
        public readonly AppDbContext _context;
        public CadastroController(AppDbContext context) {
            _context = context;
        }

        //#########################################################################################################
        public async Task<IActionResult> Index(string msgResult, string parceiro, string ac) {
            CadastroUsuarioVM cuSe = new CadastroUsuarioVM() { msgResult = msgResult, Indicacao = parceiro?.Replace('-', '/') };
            List<Grupo_Cadastro> listaGrupos = null;


            try {
                listaGrupos = await _context.GruposCadastro.
                  Include(g => g.Linhas).ThenInclude(l => l.Campos).ThenInclude(l => l.Selects).
                  Include(g => g.Linhas).ThenInclude(l => l.Campos).ThenInclude(l => l.CheckBoxes).
                  Include(g => g.Linhas).ThenInclude(l => l.Campos).ThenInclude(l => l.RadioButtons).
                  OrderBy(g => g.Ordem).
                  ToListAsync();
            } catch(Exception err) {
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Danger(err.Message) });
            }


            //Coleta dados para o Auto Complete externo
            //Este mecanismo da a possibilidade de já deixar alguns campos preenchidos com base nos dados informados via url
            //Como exemplo esta url "https://site?ac=3**Paulo)(2**123.456.789-98" id**texto
            Dictionary<int, string> acDictionary = new Dictionary<int, string>();
            string[] acArray = ac?.Split(")(") ?? new string[0];
            string[] data;
            int idC = 0;
            foreach(string d in acArray) {
                data = d.Split("**");
                if(data.Length != 2 || !int.TryParse(data[0], out idC) || idC == 0)
                    continue;
                acDictionary.Add(idC, data[1]);
            }

            //Coleta os Grupos/Linhas/Campos, Gera os Dados Vazios e efetua correções
            cuSe.grupos = listaGrupos.
              Where(g => g.Linhas.Any(l => l.Campos.Any(c => c.Ativo && c.StartCriacaoCampo == EStartCriacaoCampo.Cadastro || c.Generico))).
              OrderBy(g => g.Ordem).ToList();
            foreach(Grupo_Cadastro grupo in cuSe.grupos) {
                grupo.Linhas = grupo.Linhas.
                  Where(l => l.Campos.Any(c => c.Ativo && c.StartCriacaoCampo == EStartCriacaoCampo.Cadastro || c.Generico)).
                  OrderBy(l => l.Ordem).ToList();
                foreach(Linha_Cadastro linha in grupo.Linhas.OrderBy(l => l.Ordem)) {
                    linha.Campos = linha.Campos.Where(c => c.Ativo && c.StartCriacaoCampo == EStartCriacaoCampo.Cadastro || c.Generico).OrderBy(c => c.Ordem_Cadastro).ToList();
                    //Gera os dados Vazios de cada campo
                    linha.Campos.ForEach(c => c.GerarNovoDado(acDictionary));
                    if(linha.Campos.Any(c => c.Generico && !c.Ativo))
                        return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Danger("Campo genérico inativo!") });
                }
            }

            return View(cuSe);
        }

        //##################################################################################################
        public async Task<IActionResult> Cadastrar(/*IFormCollection form, */[FromForm] CadastroUsuarioVM cadastro) {
            string msg = string.Empty;

            List<Dado_Cadastro> listaDados = cadastro.GetListaDados();

            Usuario usuario = new Usuario();
            usuario._context = _context;

            try {

                string url = Url.ActionLink("ValidarEmail", "Cadastro");//Verificar data de criação
                cadastro.usuario = await _context.Usuarios.
                  Include(u => u.Dados).ThenInclude(d => d.Campo).
                  FirstOrDefaultAsync(u => u.UsuarioId == usuario.UsuarioId);

                
                //Cria Usuário
                await usuario.VerificarCriacao(listaDados);
                await usuario.CorrigirCriacao(listaDados);
                await usuario.Criar(listaDados);


                //bool somenteTeste = false;//////////////////////////////////////////////////////////////////////////REMOVER
                bool somenteTeste = false;////////////////////////////////////////////////////////////////////////////REMOVER
                                          //await usuario.RemoverUsuario();/////////////////////////////////////////////////////////////////////REMOVER


                //verifica a disponibilidade de enviar confirmação de Email
                EmailService emSe = new EmailService(_context);
                EVerificaConfirmacao resVerificacao = 0;
                if(!somenteTeste)
                    resVerificacao = await emSe.VerificarConfirmacaoEmail(usuario.UsuarioId);
                // -> Ausente / Erro / Pendente / Vencido / Válido
                if(resVerificacao == EVerificaConfirmacao.Pendente && !somenteTeste) {
                    throw new Exception("Foi criado recentemente uma conta usando este Email, acesse seu Email para efetuar a verificação.");
                } else if(resVerificacao == EVerificaConfirmacao.Confirmado && !somenteTeste) {
                    throw new Exception("Já existe uma conta cadastrada usando este Email.");
                } else if(resVerificacao == EVerificaConfirmacao.Erro && !somenteTeste) {
                    throw new Exception("Ocorreu um erro ao verificar a validação de Email");
                }

                #region Envio de Email para confirmação
                //Enviar Email para verificação
                if(!somenteTeste) {
                    await emSe.EnviarConfirmacaoCadastro(usuario, Url);
                }

                #endregion

                msg = AlertMessage.Success("<span style=\"font-size: 15pt;\" class=\"font-weight-bolder\">Quase Pronto!</span><br><br> Acesse sua conta de Email para confirmação.<br>Verifique na <span class=\"font-weight-bolder\">Caixa de Spam</span> ou <span class=\"font-weight-bolder\">Lixo Eletrônico</span>.") + AlertMessage.Danger("<strong>A não confirmação em até 72 horas, sua conta será removida permanentemente.</strong>");


            } catch(Exception err) {

                if(await _context.Usuarios.AnyAsync(u => u.UsuarioId == usuario.UsuarioId) && usuario.UsuarioId != 0)
                    await usuario.RemoverUsuario();
                return RedirectToAction("Index", "Cadastro", new CadastroUsuarioVM() { msgResult = AlertMessage.Danger(err.Message), Indicacao = cadastro.Indicacao });
            }

            await _context.DisposeAsync();
            GC.Collect();

            return RedirectToAction("Index", "Login", new { msgResult = msg });
        }


        //##################################################################################################
        [HttpGet]
        public async Task<IActionResult> NovoGrupo() {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar();
            ViewData["acesso"] = acesso;
            if(acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if(!acesso._permissao.CriarGrupoCadastro)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });
            #endregion

            CriarGrupoVM cgSe = new CriarGrupoVM();

            try {
                cgSe.grupo = new Grupo_Cadastro() { Ordem = await _context.GruposCadastro.MaxAsync(g => g.Ordem) + 1 };

            } catch(Exception err) {
                return RedirectToAction("Index", "Configuracao", new { msgResult = err.Message });
            }


            return View("Grupo/NovoGrupo", cgSe);
        }

        [HttpPost]
        //##################################################################################################
        public async Task<IActionResult> NovoGrupo(CriarGrupoVM cgSe) {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar();
            ViewData["acesso"] = acesso;
            if(acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if(!acesso._permissao.CriarGrupoCadastro)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });
            #endregion

            string msg = string.Empty;

            try {
                cgSe.grupo._context = _context;
                cgSe.grupo.VerificarCriacao();
                cgSe.grupo.CorrigirCriacao();
                await cgSe.grupo.Criar();

                msg = AlertMessage.Success("Grupo criado com sucesso!");
            } catch(Exception err) {
                msg = AlertMessage.Danger(err.Message);
            }

            return RedirectToAction("Index", "Configuracao", new { msgResult = msg });
        }

        //##################################################################################################
        [AcceptVerbs("GET")]
        public async Task<IActionResult> EditarGrupo(int id) {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar();
            ViewData["acesso"] = acesso;
            if(acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if(!acesso._permissao.EditarGrupoCadastro)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });
            #endregion

            EditarGrupoVM egSe = new EditarGrupoVM();
            egSe.grupo = await _context.GruposCadastro.FirstOrDefaultAsync(g => g.Grupo_CadastroId == id);
            if(egSe.grupo == null)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Grupo não Encontrado!") });

            return View("Grupo/EditarGrupo", egSe);
        }

        //##################################################################################################
        public async Task<IActionResult> EditarGrupo(EditarGrupoVM egSe) {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar();
            ViewData["acesso"] = acesso;
            if(acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if(!acesso._permissao.EditarGrupoCadastro)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });
            #endregion

            string msg = string.Empty;

            Grupo_Cadastro grupo = await _context.GruposCadastro.FirstOrDefaultAsync(g => g.Grupo_CadastroId == egSe.grupo.Grupo_CadastroId);
            if(grupo == null)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Grupo não encontrado") });

            try {
                grupo._context = _context;
                grupo.VerificarEdicao(egSe);
                grupo.CorrigirEdicao(egSe);
                egSe.grupo.CopyToUpdate(grupo);
                await grupo.Editar(egSe);

                msg = AlertMessage.Success("Alterações salvas com sucesso!");
            } catch(Exception err) {
                msg = AlertMessage.Danger(err.Message);
            }

            return RedirectToAction("Index", "Configuracao", new { msgResult = msg });
        }

        //##################################################################################################
        public async Task<IActionResult> RemoverGrupo(int id) {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar();
            ViewData["acesso"] = acesso;
            if(acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if(!acesso._permissao.RemoverGrupoCadastro)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });
            #endregion

            string msg = string.Empty;
            Grupo_Cadastro grupo = null;

            try {
                grupo = await _context.GruposCadastro.Include(g => g.Linhas).ThenInclude(l => l.Campos).FirstOrDefaultAsync(g => g.Grupo_CadastroId == id);
                if(grupo == null)
                    throw new Exception("Grupo não encontrado!");
                if(grupo.Generico)
                    throw new Exception("Grupos genéricos não podem ser removidos!");

                await grupo.Remover();

                msg = AlertMessage.Success("Grupo removido com sucesso!");
            } catch(Exception err) {
                msg = AlertMessage.Danger(err.Message);
            }

            return RedirectToAction("Index", "Configuracao", new { msgResult = msg });
        }

        //##################################################################################################
        [HttpGet]
        public async Task<IActionResult> NovaLinha(int id) {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar();
            ViewData["acesso"] = acesso;
            if(acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if(!acesso._permissao.CriarLinhaCadastro)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });
            #endregion

            CriarLinhaVM clSe = new CriarLinhaVM();
            clSe.linha = new Linha_Cadastro() {
                Grupo_CadastroId = id
            };
            //Atribui última ordem disponível deste grupo
            if(await _context.LinhasCadastro.AnyAsync(l => l.Grupo_CadastroId == id)) {
                clSe.linha.Ordem = await _context.LinhasCadastro.Where(l => l.Grupo_CadastroId == id).MaxAsync(l => l.Ordem) + 1;

                if(clSe.linha.Ordem < 0)
                    clSe.linha.Ordem = 0;
            }

            return View("Linha/NovaLinha", clSe);
        }

        [HttpPost]
        //##################################################################################################
        public async Task<IActionResult> NovaLinha(CriarLinhaVM clSe) {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar();
            ViewData["acesso"] = acesso;
            if(acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if(!acesso._permissao.CriarLinhaCadastro)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });
            #endregion

            string msg = string.Empty;

            try {
                //Salva a linha
                clSe.linha._context = _context;
                clSe.linha.VerificarCriacao();
                clSe.linha.CorrigirCriacao();
                await clSe.linha.Criar(clSe);

                msg = AlertMessage.Success("Linha criada com sucesso!");
            } catch(Exception err) {
                msg = AlertMessage.Danger(err.Message);
            }

            return RedirectToAction("Index", "Configuracao", new { msgResult = msg });
        }

        //##################################################################################################
        [AcceptVerbs("GET")]
        public async Task<IActionResult> EditarLinha(int id, string msgResult) {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar();
            ViewData["acesso"] = acesso;
            if(acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if(!acesso._permissao.EditarLinhaCadastro)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });
            #endregion

            EditarLinhaVM elSe = new EditarLinhaVM();
            try {
                elSe.linha = await _context.LinhasCadastro.FirstOrDefaultAsync(g => g.Linha_CadastroId == id);
                if(elSe.linha == null)
                    throw new Exception("Linha não Encontrada!");
            } catch(Exception err) {
                return RedirectToAction("Index", "Configuracao", new { msgResult = AlertMessage.Danger(err.Message) });
            }

            ViewData["msgResult"] = msgResult;

            return View("Linha/EditarLinha", elSe);
        }

        //##################################################################################################
        public async Task<IActionResult> EditarLinha(EditarLinhaVM elSe) {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar();
            ViewData["acesso"] = acesso;
            if(acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if(!acesso._permissao.EditarLinhaCadastro)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });
            #endregion

            string msg = string.Empty;

            Linha_Cadastro linha = await _context.LinhasCadastro.FirstOrDefaultAsync(l => l.Linha_CadastroId == elSe.linha.Linha_CadastroId);
            if(linha == null)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Linha não encontrada!") });

            try {
                linha._context = _context;
                linha.VerificarEdicao(elSe);
                linha.CorrigirEdicao(elSe);
                elSe.linha.CopyToUpdate(linha);
                await linha.Editar(elSe);

                msg = AlertMessage.Success("Alterações salvas com sucesso!");
            } catch(Exception err) {
                msg = AlertMessage.Danger(err.Message);
            }

            return RedirectToAction("Index", "Configuracao", new { msgResult = msg });
        }

        //##################################################################################################
        public async Task<IActionResult> RemoverLinha(int id) {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar();
            ViewData["acesso"] = acesso;
            if(acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if(!acesso._permissao.RemoverLinhaCadastro)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });
            #endregion

            string msg = string.Empty;
            Linha_Cadastro linha = null;

            try {

                linha = await _context.LinhasCadastro.Include(l => l.Campos).FirstOrDefaultAsync(l => l.Linha_CadastroId == id);
                if(linha == null)
                    throw new Exception("Linha não encontrada!");
                if(linha.IsGenerico())
                    throw new Exception("Linhas genéricas não podem ser removidas!");

                await linha.Remover();

                msg = AlertMessage.Success("Linha removida com sucesso!");
            } catch(Exception err) {
                msg = AlertMessage.Danger(err.Message);
            }

            return RedirectToAction("Index", "Configuracao", new { msgResult = msg });
        }

        //##################################################################################################
        public async Task<IActionResult> AplicarModeloCampo(CriarCampoVM ccSe) {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar();
            ViewData["acesso"] = acesso;
            if(acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if(!acesso._permissao.CriarCampoCadastro)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });
            #endregion

            return RedirectToAction("NovoCampo", new { ccSe_Json = JsonSerializer.Serialize(ccSe) });// JsonConvert.SerializeObject(ccSe) });
        }

        //##################################################################################################
        [HttpGet]
        public async Task<IActionResult> NovoCampo(string ccSe_Json) {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar();
            ViewData["acesso"] = acesso;
            if(acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if(!acesso._permissao.CriarCampoCadastro)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });
            #endregion

            CriarCampoVM ccSe = JsonSerializer.Deserialize<CriarCampoVM>(ccSe_Json);//JsonConvert.DeserializeObject<CriarCampoVM>(ccSe_Json);

            //Verifica se id da linha foi informado
            if(ccSe?.campo == null)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Linha não encontrada!") });
            //Verifica se linha existe
            Linha_Cadastro linha = await _context.LinhasCadastro.FirstOrDefaultAsync(l => l.Linha_CadastroId == ccSe.campo.Linha_CadastroId);
            if(linha == null)
                return RedirectToAction("Index", "Configuracao", new { msgResult = AlertMessage.Danger("Linha não encontrada!") });
            //Verifica se é uma Linha genérica
            if(linha.IsGenerico())
                return RedirectToAction("Index", "Configuracao", new { msgResult = AlertMessage.Danger("Não é possível criar campos em linhas genéricas") });

            //#### Correções ####

            //Coleta a última ordem do cadastro disponível na linha informada
            if(ccSe?.campo != null && await _context.CamposCadastro.AnyAsync(c => c.Linha_CadastroId == ccSe.campo.Linha_CadastroId)) {
                ccSe.campo.Ordem_Cadastro = await _context.CamposCadastro.Where(c => c.Linha_CadastroId == ccSe.campo.Linha_CadastroId)?.MaxAsync(c => c.Ordem_Cadastro) + 1;

                if(ccSe.campo.Ordem_Cadastro < 0)
                    ccSe.campo.Ordem_Cadastro = 0;

                ccSe.campo.Ordem_Perfil = ccSe.campo.Ordem_Cadastro;
            }
            if(ccSe?.campo != null)
                ccSe.campo.ColunasXS_Cadastro = 12;


            ViewData["SLAutorizadoEditar"] = new SelectList(CampoCadastroUtilitiesVM.GetColecaoAutorizadoEditar(), "Key", "Value");
            ViewData["SLStartCriacaoCampos"] = new SelectList(CampoCadastroUtilitiesVM.GetColecaoStartCriacaoCampos(), "Key", "Value");
            ViewData["SLModelosCampos"] = new SelectList(CampoCadastroUtilitiesVM.GetColecaoModelosCampos(), "Key", "Value");
            ViewData["SLDirecaoItens"] = new SelectList(CampoCadastroUtilitiesVM.GetColecaoDirecaoItens(), "Key", "Value");

            if(ccSe?.campo?.ModeloCampo_Nullable == null) {
                return View("Campo/NovoCampo_SelecaoModelo", ccSe);
            } else {
                ccSe.campo.ModeloCampo = ccSe.campo.ModeloCampo_Nullable ?? 0;
                return View("Campo/NovoCampo", ccSe);
            }
        }

        //##################################################################################################
        [HttpPost]
        public async Task<IActionResult> NovoCampo([FromForm] CriarCampoVM ccSe) {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar();
            ViewData["acesso"] = acesso;
            if(acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if(!acesso._permissao.CriarCampoCadastro)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });
            #endregion

            string msg = string.Empty;

            try {
                ccSe.campo._context = _context;
                ccSe.campo.VerificarCriacao(ccSe);
                ccSe.campo.CorrigirCriacao(ccSe);
                await ccSe.campo.Criar(ccSe);

                msg = AlertMessage.Success("Campo criado com sucesso!");
            } catch(Exception err) {
                msg = AlertMessage.Danger(err.Message);
            }

            return RedirectToAction("Index", "Configuracao", new { msgResult = msg });
        }

        //##################################################################################################
        [HttpGet]
        public async Task<IActionResult> EditarCampo(int id, string msgResult) {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar();
            ViewData["acesso"] = acesso;
            if(acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            #endregion

            EditarCampoVM ecSe = new EditarCampoVM();

            try {
                ecSe.campo = await _context.CamposCadastro.
                  Include(m => m.Selects).
                  Include(m => m.CheckBoxes).
                  Include(m => m.RadioButtons).
                  FirstOrDefaultAsync(c => c.Campo_CadastroId == id);
                if(ecSe.campo == null)
                    throw new Exception("Campo não encontrado");
                //Verifica se o usuário tem permissão para editar o campo
                if(!(
                  ecSe.campo.Generico && acesso._permissao.EditarCampoGenericoCadastro ||
                  !ecSe.campo.Generico && acesso._permissao.EditarCampoPersonalizadoCadastro
                ))
                    throw new Exception("Você não tem permissão!");

            } catch(Exception err) {
                return RedirectToAction("Index", "Configuracao", new { msgResult = AlertMessage.Danger(err.Message) });
            }

            ViewData["SLAutorizadoEditar"] = new SelectList(CampoCadastroUtilitiesVM.GetColecaoAutorizadoEditar(), "Key", "Value");
            ViewData["SLStartCriacaoCampos"] = new SelectList(CampoCadastroUtilitiesVM.GetColecaoStartCriacaoCampos(), "Key", "Value");
            ViewData["SLModelosCampos"] = new SelectList(CampoCadastroUtilitiesVM.GetColecaoModelosCampos(), "Key", "Value");
            ViewData["SLDirecaoItens"] = new SelectList(CampoCadastroUtilitiesVM.GetColecaoDirecaoItens(), "Key", "Value");
            ViewData["msgResult"] = msgResult;

            return View("Campo/EditarCampo", ecSe);
        }

        //##################################################################################################
        [HttpPost]
        public async Task<IActionResult> EditarCampo(EditarCampoVM ecSe) {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar();
            ViewData["acesso"] = acesso;
            if(acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            #endregion

            string msg = null;
            Campo_Cadastro campo = null;

            try {

                campo = await _context.CamposCadastro.Include(c => c.Selects).Include(c => c.CheckBoxes).Include(c => c.RadioButtons).FirstOrDefaultAsync(c => c.Campo_CadastroId == ecSe.campo.Campo_CadastroId);
                if(campo == null)
                    throw new Exception("Campo não encontrado!");

                //Verifica se o usuário tem permissão para editar o campo
                if(!(
                  campo.Generico && acesso._permissao.EditarCampoGenericoCadastro ||
                  !campo.Generico && acesso._permissao.EditarCampoPersonalizadoCadastro
                ))
                    throw new Exception("Você não tem permissão!");

                campo._context = _context;

                //Atribuições pendentes
                ecSe.campo.Generico = campo.Generico;
                ecSe.campo.ModeloCampo = campo.ModeloCampo;
                ecSe.campo.CampoGenerico = campo.CampoGenerico;

                ecSe.campo._context = _context;
                ecSe.campo.VerificarEdicao(ecSe);
                ecSe.campo.CorrigirEdicao(ecSe);

                ecSe.campo.CopyToUpdate(campo);

                await campo.Update(ecSe);

                if(ecSe.DeportarCampo)
                    await campo.DeportarCampoGenerico();

                msg = AlertMessage.Success("Alterações salvas com sucesso!");
            } catch(Exception err) {
                msg = AlertMessage.Danger(err.Message);
            }

            return RedirectToAction("EditarCampo", "Cadastro", new { id = campo.Campo_CadastroId, msgResult = msg });
        }

        //##################################################################################################
        [HttpGet]
        public async Task<IActionResult> ImportarCampo(int id) {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar();
            ViewData["acesso"] = acesso;
            if(acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if(!acesso._permissao.EditarLinhaCadastro)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });
            #endregion

            string msg = string.Empty;
            ImportarCampoVM icSe = new ImportarCampoVM();
            List<Campo_Cadastro> campos = null;

            try {
                icSe.linha = await _context.LinhasCadastro.FirstOrDefaultAsync(l => l.Linha_CadastroId == id);
                if(icSe.linha == null)
                    throw new Exception("Linha não encontrada.");
                if(icSe.linha.IsGenerico())
                    throw new Exception("Linhas genéricas são exclusivas para campos genéricos.");

                campos = await _context.CamposCadastro.Where(c => c.Linha_CadastroId != id).ToListAsync();

            } catch(Exception err) {
                msg = AlertMessage.Danger(err.Message);
                return RedirectToAction("Index", "Configuracao", new { msgResult = msg });
            }

            ViewData["campos"] = campos;

            return View("Linha/ImportarCampo", icSe);
        }

        //##################################################################################################
        [HttpPost]
        public async Task<IActionResult> ImportarCampo(ImportarCampoVM icSe) {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar();
            ViewData["acesso"] = acesso;
            if(acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if(!acesso._permissao.EditarLinhaCadastro)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });
            #endregion

            string msg = string.Empty;
            Linha_Cadastro linha = null;

            try {
                linha = await _context.LinhasCadastro.FirstOrDefaultAsync(l => l.Linha_CadastroId == icSe.linha.Linha_CadastroId);
                if(linha == null)
                    throw new Exception("Linha não encontrada.");

                await linha.ImportarCampo(icSe.novoCampo.Campo_CadastroId);

                msg = AlertMessage.Success("Campo importado com sucesso!");
            } catch(Exception err) {
                msg = AlertMessage.Danger(err.Message);
                return RedirectToAction("Index", "Configuracao", new { msgResult = msg });
            }

            return RedirectToAction("EditarLinha", "Cadastro", new { id = icSe.linha.Linha_CadastroId, msgResult = msg });
        }

        //##################################################################################################
        public async Task<IActionResult> RemoverCampo(int id) {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar();
            ViewData["acesso"] = acesso;
            if(acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if(!acesso._permissao.RemoverCampoCadastro)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });
            #endregion

            string msg = string.Empty;
            Campo_Cadastro campo = null;

            try {
                campo = await _context.CamposCadastro.FirstOrDefaultAsync(c => c.Campo_CadastroId == id);
                if(campo == null)
                    throw new Exception("Campo não encontrado");

                campo._context = _context;
                await campo.Remover();

                msg = AlertMessage.Success("Campo removido com sucesso!");
            } catch(Exception err) {
                msg = AlertMessage.Danger(err.Message);
                return RedirectToAction("Index", "Configuracao", new { msgResult = msg });
            }


            return RedirectToAction("Index", "Configuracao", new { msgResult = msg });
        }

        //##################################################################################################
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> VerificarEmail(CadastroUsuarioVM cuSe) {
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //return Json(true);
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            bool res = true;
            List<Dado_Cadastro> lista = cuSe.GetListaDados();
            Dado_Cadastro dadoEmail = cuSe.GetListaDados().FirstOrDefault();


            if(dadoEmail == null)
                return Json("Ocorreu um erro ao procurar o campo de Email no banco de dados");

            cuSe._context = _context;
            res = await cuSe.verificarEmailExists(dadoEmail.Email_Novo, dadoEmail.Email_Generico, dadoEmail.Dado_CadastroId_Email);

            if(res) {
                return Json($"Este email já está em uso.");
            }

            return Json(true);
        }

        //##################################################################################################
        [HttpGet("Cadastro/ValidarEmail/{k}")]
        public async Task<IActionResult> ValidarEmail(string k) {

            EmailService es = new EmailService(_context);
            try {
                EVerificaConfirmacao ec = await es.ValidarKey(k);
                es.confirmacaoResult = new ResultEmailValidadoVM() { Resultado = ec };
            } catch(Exception err) {
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Danger(err.Message) });
            }


            if(es.confirmacaoResult.Resultado == EVerificaConfirmacao.Confirmado) {
                es.msgResult = AlertMessage.Info("Este Email já foi confirmado!");
            } else if(es.confirmacaoResult.Resultado == EVerificaConfirmacao.Ausente) {
                es.msgResult = AlertMessage.Danger("Não há nenhuma conta para ser validada.");
            } else if(es.confirmacaoResult.Resultado == EVerificaConfirmacao.Pendente) {
                es.msgResult = AlertMessage.Success("Seu Email foi confirmado com sucesso!") + AlertMessage.Info("Efetue login para ter acesso ao Painel!");
            }


            return RedirectToAction("Index", "Login", new { es.msgResult });
        }

        //##################################################################################################
        [HttpGet("Cadastro/ValidarNovoEmail/{k}")]
        public async Task<IActionResult> ValidarNovoEmail(string k) {
            EmailService es = new EmailService(_context);
            EVerificaConfirmacao ec = await es.ValidarKey(k);

            if(ec == EVerificaConfirmacao.Pendente) {
                es.msgResult = AlertMessage.Info("Email alterado com sucesso!");
            } else if(ec == EVerificaConfirmacao.Ausente) {
                es.msgResult = AlertMessage.Danger("Nenhuma solicitação encontrada!");
            } else {
                es.msgResult = AlertMessage.Danger("Ocorreu um erro ou nenhuma solicitação foi encontrada!");
            }

            //Consulta usuário da sessão
            long UsuarioId = HttpContext.Session.GetInt32("UsuarioId") ?? 0;

            if(await _context.Usuarios.AnyAsync(u => u.UsuarioId == UsuarioId)) {
                return RedirectToAction("Index", "Home", new { es.msgResult });
            }

            return RedirectToAction("Index", "Login", new { es.msgResult });
        }

        //##################################################################################################
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> VerificarCpf(CadastroUsuarioVM cuSe) {

            bool res = true;
            List<Dado_Cadastro> lista = cuSe.GetListaDados();
            Dado_Cadastro dadoCpf = cuSe.GetListaDados().FirstOrDefault();


            if(dadoCpf == null)
                return Json("Ocorreu um erro ao procurar o campo de CPF no banco de dados");
            if(!Cpf.Validate(dadoCpf.Cpf_Generico))
                return Json("CPF inválido.");

            cuSe._context = _context;
            res = await cuSe.verificarCpfExists(dadoCpf.Cpf_Generico, dadoCpf.Dado_CadastroId_Cpf);

            if(res) {
                return Json($"Este CPF já está em uso.");
            }

            return Json(true);
        }

        //##################################################################################################
        [HttpGet]
        public IActionResult ReenviarValidacao() {
            return View(new EmailService());
        }

        //##################################################################################################
        [HttpPost]
        public async Task<IActionResult> ReenviarValidacao(EmailService emSe) {
            CadastroUsuarioVM cus = new CadastroUsuarioVM(_context);

            //Pega o Id do usuário pelo Email
            cus.usuario = await cus.GetUsuarioByEmail(emSe.Email);
            EmailService es = new EmailService(_context);

            try {
                if(cus.usuario != null)
                    if(cus.usuario.EmailsValidados?.Any(ev => ev.Confirmado) ?? false)
                        throw new Exception("Este Email já foi validado, efetue login para ter acesso ao painel.");

                await es.EnviarConfirmacaoCadastro(cus.usuario, Url);


                emSe.msgResult = AlertMessage.Success("O Email de confirmação foi enviado com sucesso!") +
                  AlertMessage.Info("Acesse seu Email para efetuar a confirmação de Email.");
            } catch(Exception err) {
                emSe.msgResult = AlertMessage.Danger("Ocorreu um Erro: " + err.Message);
            }

            return RedirectToAction("Index", "Login", new { emSe.msgResult });
        }


    }
}
