using Cadastro.Data;
using Cadastro.Models.Account.Cadastro.Campos;
using Cadastro.Models.Account.Cadastro.Dados;
using Cadastro.Models.Entities.DB.Account;
using Cadastro.Models.Entities.DB.Account.Remove;
using Cadastro.Models.Services.Account;
using Cadastro.Models.Services.Account.Remove;
using Cadastro.Models.Services.Account.Remove.Email;
using Cadastro.Models.ViewModel.Account.Remocao;
using Cadastro.Models.ViewModel.Application.AccessContext;
using Cadastro.Models.ViewModel.Application.Notifications.Alerts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cadastro.Controllers.Account
{
    public class ContaController : Controller {

        private readonly AppDbContext _context;

        public ContaController(AppDbContext context) {
            _context = context;
        }

        public IActionResult Index() {
            return View();
        }

        //################################################################################################
        [HttpGet]
        public async Task<IActionResult> AutenticarRemocao(string res) {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar(); ViewData["acesso"] = acesso;
            if (acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            #endregion

            //Caso já tenha uma solicitação pendente, já redirecionar para confirmação (isso acontece quando um usuário já autenticou a remoção e perdeu conexão com internet por exemplo)
            AuthToRemove auth = await _context.AuthsToRemove.FirstOrDefaultAsync(a => a.UsuarioId == acesso.UsuarioId);
            if (auth != null) return RedirectToAction("ConfirmarSolicitacaoRemocao", "Conta", new { request = auth.Code });

            AutenticarRemocaoVM model = new AutenticarRemocaoVM() {
                res = res
            };

            return View("Views/Account/Remove/AutenticarRemocao.cshtml", model);
        }

        //################################################################################################
        [HttpPost]
        public async Task<IActionResult> AutenticarRemocao(AutenticarRemocaoVM atRe) {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar(); ViewData["acesso"] = acesso;
            if (acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if (acesso.IsAdmin())
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Admin não pode ser removido!") });
            #endregion

            if (!ModelState.IsValid) {
                return RedirectToAction("AutenticarRemocao", "Conta", new AutenticarRemocaoVM() { res = AlertMessage.Danger("Informe uma senha correta!") });
            }

            Dado_Cadastro dado = await _context.DadosCadastro.Include(d => d.Campo).FirstOrDefaultAsync(d => d.Campo.CampoGenerico == ECampoGenerico.Senha && d.UsuarioId == acesso.UsuarioId);
            if (dado == null) return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Usuário não encontrado!") });
            
            //Processo de geração do autenticador no banco de dados
            AuthToRemove auth = null;
            AccountRemoverAuthenticator remover = new AccountRemoverAuthenticator(acesso._usuario, _context);


            if (dado.Senha == atRe.GetPassMD5()) {
                //Redirecionar para página de confirmação
                auth = await remover.GenerateAuthAsync();
            } else {
                //Redirecionar para Home
                HttpContext.Session.Clear();
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Danger("Senha incorreta!") });
            }


            return RedirectToAction("ConfirmarSolicitacaoRemocao", "Conta", new { request = auth.Code });
        }

        //################################################################################################
        [HttpGet]
        public async Task<IActionResult> ConfirmarSolicitacaoRemocao(string request) {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar(); ViewData["acesso"] = acesso;
            if (acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if (!await _context.AuthsToRemove.AnyAsync(a => a.Code == request && a.UsuarioId == acesso.UsuarioId)) {
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Acesso negado!") });
            }
            #endregion

            AccountRemoverAuthenticator remover = new AccountRemoverAuthenticator(acesso._usuario, _context);
            AuthToRemove auth = await remover.CollectAuthAsync(request);

            return View("Views/Account/Remove/ConfirmarSolicitacaoRemocao.cshtml", auth);
            //return View(new AuthToRemove((acesso.UsuarioId ?? 0)));//Somente para teste
        }

        //################################################################################################
        [HttpGet]
        public async Task<IActionResult> ConcluirSolicitarRemocao(bool confirmed, string request) {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar(); ViewData["acesso"] = acesso;
            if (acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if (!await _context.AuthsToRemove.AnyAsync(a => a.Code == request && a.UsuarioId == acesso.UsuarioId)) {
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Acesso negado!") });
            }
            #endregion

            NotificatorRequestRemoveAccount notificator = new NotificatorRequestRemoveAccount(acesso._usuario);

            AccountRemoverAuthenticator remover = new AccountRemoverAuthenticator(acesso._usuario, _context);
            AuthToRemove auth = await remover.CollectAuthAsync(request);
            if (auth == null) return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Acesso negado!") });

            if (confirmed) {
                //Aprova a autenticação do usuário, e inultiliza a conta do mesmo
                await remover.ApproveAuthAsync(request);
                //ENVIAR EMAIL
                await notificator.SendRequestAsync(HttpContext.Connection.RemoteIpAddress.ToString());
                HttpContext.Session.Clear();
                return Redirect("https://dash.goadopt.io/opt-out?websiteId=816c02b0-a716-44ee-838a-6011181836f5&id=d435aae5-27f6-4294-8e4c-1116b47e6f28&visitorId=baeb20ad-db18-4978-8539-8986cccff43b");
            } else {
                await remover.RemoveAllAuthsFromUser();
                return RedirectToAction("Index", "Home");
            }
        }

        //##################################################################################################
        public async Task<IActionResult> RemoverUsuario(long id) {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar(); ViewData["acesso"] = acesso;
            if (acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if (!acesso._permissao.ApagarUsuario)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });
            #endregion

            string msg = "";
            UserEntityExtractorData extractor = null;
            NotificatorAccountRemovedSuccessfully notificator = null;


            try {
                Usuario res = await _context.Usuarios.Include(u => u.Dados).ThenInclude(d => d.Campo).FirstOrDefaultAsync(u => u.UsuarioId == id);
                res._context = _context;
                if(res == null) throw new Exception("Usuário não encontrado!");

                extractor = new UserEntityExtractorData(res, _context);
                notificator = new NotificatorAccountRemovedSuccessfully(res);

                await notificator.SendNotificationAsync();

                await res.RemoverUsuario();

                msg = AlertMessage.Success("Este usuário foi removido.");
            } catch (Exception err) {
                msg = AlertMessage.Danger(err.Message);
            }

            return RedirectToAction("Index", "Home", new { msgResult = msg });
        }


    }
}
