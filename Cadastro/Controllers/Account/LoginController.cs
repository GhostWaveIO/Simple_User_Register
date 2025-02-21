using Cadastro.Data;
using Cadastro.Models.Account.Cadastro.Campos;
using Cadastro.Models.Account.Cadastro.Dados;
using Cadastro.Models.Entities.DB.Account;
using Cadastro.Models.Services.Application;
using Cadastro.Models.Services.Application.Security.Autorizacao;
using Cadastro.Models.ViewModel.Account;
using Cadastro.Models.ViewModel.Application;
using Cadastro.Models.ViewModel.Application.Notifications.Alerts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Cadastro.Controllers.Account {
    public class LoginController : Controller {

        private readonly AppDbContext _context;

        public LoginController(AppDbContext context) {
            _context = context;
        }

        //###########################################################################################################################################
        [Route("Login/")]
        [Route("Login/Index")]
        [Route("Login/Index/{msgResult?}")]
        public async Task<IActionResult> Index(string msgResult) {
            //HttpContext.Connection.RemoteIpAddress.ToString();
#if DEBUG
            if(await _context.Usuarios.AnyAsync(u => u.UsuarioId == 1) && false) {
                HttpContext.Session.SetInt32("UsuarioId", 1);
                return Redirect("http://localhost:51205/Usuarios/ConfirmarSolicitacaoRemocao?request=gkIGYuM1yAdfCCusAEpJE25erDP8PBXunlOcINke4cs9yLDCnS9f7D5jTtQk8ET71XvAVl1gb0laDYWcPaxQge41WFW9VmoSYW7Rxg94cTAQjnHJDDldG8o4");
            }

#endif

            try {

                if(string.IsNullOrEmpty(msgResult)) {
                    if(!await _context.CamposCadastro.AnyAsync())
                        throw new Exception("Não existe nenhum campo cadastrado!");
                }

            } catch(Exception err) {
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Danger(err.Message) });
            }

            ViewData["context"] = _context;


            UsuarioVM usSe = new UsuarioVM() {
                usuario = new Usuario() { Dados = new List<Dado_Cadastro>() }
            };
            usSe.usuario.Dados.Add(
              new Dado_Cadastro() { Campo = new Campo_Cadastro() { CampoGenerico = ECampoGenerico.Email } }
            );
            usSe.usuario.Dados.Add(
              new Dado_Cadastro() { Campo = new Campo_Cadastro() { CampoGenerico = ECampoGenerico.Senha } }
            );

            ViewData["msgresult"] = msgResult;

            return View(usSe);
        }

        //###########################################################################################################################################
        [HttpGet("Login/IndexResult/{msgResult}")]
        public IActionResult IndexResult(string msgResult) {

            return RedirectToAction("Index", new { msgResult });
        }

        //###########################################################################################################################################
        [ResponseCache(Duration = 100, NoStore = false)]
        [HttpPost]
        public async Task<IActionResult> ValidarLogin(UsuarioVM us) {
            string senhaMd5 = us.usuario.GetSenhaMD5;
            string msgErroSenha = "Email ou Senha incorreta!";
            Usuario usuario = null;

            Dado_Cadastro dadoEmail = us.usuario.Dados.FirstOrDefault(d => d.Campo.CampoGenerico == ECampoGenerico.Email);
            Dado_Cadastro dadoSenha = us.usuario.Dados.FirstOrDefault(d => d.Campo.CampoGenerico == ECampoGenerico.Senha);

            try {
                //Verifica se dado de email e senha foram informados
                if(!us.usuario.Dados.Any(d => d.Campo.CampoGenerico == ECampoGenerico.Email) && us.usuario.Dados.Any(d => d.Campo.CampoGenerico == ECampoGenerico.Senha))
                    return RedirectToAction("IndexResult", "Login", new { msgResult = AlertMessage.Danger(msgErroSenha) });

                if(dadoEmail?.Campo == null || dadoSenha?.Campo == null)
                    return RedirectToAction("IndexResult", "Login", new { msgResult = AlertMessage.Danger(msgErroSenha) });

                foreach(Usuario user in await Task.FromResult(_context.Usuarios.Include(u => u.Dados).ThenInclude(d => d.Campo).Include(u => u.EmailsValidados).Include(u => u.AuthsToRemove))) {
                    if(user.Dados.SingleOrDefault(d => d.Campo.CampoGenerico == ECampoGenerico.Email)?.Email == dadoEmail.Email)//Verifica Email
                        if(user.Dados.SingleOrDefault(d => d.Campo.CampoGenerico == ECampoGenerico.Senha)?.Senha == dadoSenha.GetSenhaMD5)//Verifica Senha
                            usuario = user;
                }

                if(usuario != null && usuario.AuthsToRemove.Any(a => a.Approved == true)) {
                    usuario = null;
                    throw new Exception("Usuário não contrado ou inativo!");
                }

            } catch(Exception err) {
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Danger(err.Message) });
            }



            if(usuario == null)
                return RedirectToAction("IndexResult", "Login", new { msgResult = AlertMessage.Danger(msgErroSenha) });

            bool emailConfirmado = usuario.EmailsValidados.Any(ev => ev.Confirmado);
            //Se Usuário for Admin, não precisa verificar E-mail
            if(usuario.IsAdmin()) {
                emailConfirmado = true;
            } else {

                usuario.DescriptografarDados();

                int idCampoNomeCompleto = 19;
                int idCampoTelefone = 8;

                string[] teste = new string[] {
                    usuario.GetDado_CadastroByCampo_CadastroId(idCampoNomeCompleto).Texto250,
                    usuario.GetDado_CadastroByCampo_CadastroId(idCampoTelefone).Texto250
                };

            }




            //Verifica se usuario foi validado
            us._context = _context;
            if(emailConfirmado) {
                HttpContext.Session.SetInt32("UsuarioId", (int)usuario.UsuarioId);
                return RedirectToAction("Index", "Home");
            } else {
                us.msgResult = AlertMessage.Danger("Confirme o E-mail para logar, ou" +
                  " <a href=\"" + Url.ActionLink("ReenviarValidacao", "Cadastro") + "\">clique aqui</a> para reenviar a confirmação de E-mail!");
                return RedirectToAction("IndexResult", "Login", new { us.msgResult });
            }
        }

        //###########################################################################################################################################
        public IActionResult Admin() {
            //Criar Admin caso não exista
            //UsuarioService admin = new UsuarioService(_context);
            //admin.CriarAdmin();
            return View(new UsuarioVM());
        }

        //###########################################################################################################################################
        public async Task<string> CorrecaoGeral(int id) {

            string msg = string.Empty;

            try {

                //Criar Campos de Cadastro
                StartSession start = new StartSession(_context);
                await start.CorrecaoGeral();

                //Criar conta Admin
                Usuario admin = new Usuario();
                admin._context = _context;
                await admin.CriarAdmin();

                //Novos Usuários
                //if (!(await _context.Hierarquias.AnyAsync(h => h.HierarquiaId == 2609906)))
                if(!await _context.Hierarquias.AnyAsync(h => h.HierarquiaId == 1)) {
                    await _context.Hierarquias.AddAsync(new Hierarquia() { Titulo = "Novos Usuários", PermissaoHierarquia = new PermissaoHierarquia() });
                    await _context.SaveChangesAsync();
                }

                //if (!_context.Hierarquias.Any(h => h.HierarquiaId == 2609907))
                if(!_context.Hierarquias.Any(h => h.HierarquiaId == 2))
                    await _context.Hierarquias.AddAsync(new Hierarquia() { Titulo = "Presidente", PermissaoHierarquia = new PermissaoHierarquia() });

                await new Hierarquia(_context).ReposicionarOrdem();
                await _context.SaveChangesAsync();

                msg += "ok";
            } catch(Exception err) {
                msg = err.Message + err.InnerException.ToString();
            }

            return msg;
        }

        //###########################################################################################################################################
        public async Task<IActionResult> Reload() {
            string msg = "";


            try {
                await Program.LoadConfigs();
                msg = $"ok<br/><br/><a href=\"{Url.ActionLink("Index", "Login")}\">Pagina de Login</a>";
            } catch(Exception err) {
                msg = err.Message;
            }

            return Content(msg, "text/html");
        }

        //###########################################################################################################################################
        public string Uso() {
            Process[] process = Process.GetProcessesByName("Cadastro.exe");
            long bytes = process.ToList().Sum(p => p.PrivateMemorySize64);

            return (bytes / 1024 / 1024).ToString() + " MB de uso de memória";
        }

        //###########################################################################################################################################
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorVM { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

