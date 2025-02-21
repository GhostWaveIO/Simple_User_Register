using Cadastro.Data;
using Cadastro.Models.Account.Cadastro.Campos;
using Cadastro.Models.Account.Cadastro.Dados;
using Cadastro.Models.Entities.DB.Account;
using Cadastro.Models.Entities.DB.Account.Password;
using Cadastro.Models.ViewModel.Account.Cadastro;
using Cadastro.Models.ViewModel.Application.Notifications.Alerts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cadastro.Controllers.Account.Password {
    public class RecuperacaoController : Controller {

        public readonly AppDbContext _context;
        public RecuperacaoController(AppDbContext context) {
            _context = context;
        }

        //#########################################################################################################
        public IActionResult Index() {
            return RedirectToAction("Index", "Login");
        }

        //#########################################################################################################
        [HttpGet("Recuperacao/RecuperarSenha/{msgResult?}")]
        public async Task<IActionResult> RecuperarSenha(string msgResult) {

            CadastroUsuarioVM cuSe = null;

            try {
                cuSe = new CadastroUsuarioVM() {
                    usuario = new Usuario() { Dados = new List<Dado_Cadastro>() { new Dado_Cadastro() { Campo = await _context.CamposCadastro.FirstOrDefaultAsync(c => c.CampoGenerico == ECampoGenerico.Email) } } }
                };
            } catch(Exception err) {
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Danger(err.Message) });
            }

            ViewData["msgResult"] = msgResult;

            return View(cuSe);
        }

        //#########################################################################################################
        [HttpPost]
        public async Task<IActionResult> RecuperarSenha(CadastroUsuarioVM cuSe) {

            string msg = "";
            string msgErro = "Não foi encontrado nenhum usuário com este Email";
            string urlNovaSenha = Url.ActionLink("NovaSenha", "Recuperacao");
            Usuario usuarioConsulta = null;
            Dado_Cadastro dadoEmail = null;

            try {
                dadoEmail = await _context.DadosCadastro.Include(d => d.Campo).FirstOrDefaultAsync(d => d.Campo.CampoGenerico == ECampoGenerico.Email && d.Email == cuSe.usuario.Dados[0].Email);
                if(dadoEmail == null)
                    throw new Exception(msgErro);

                usuarioConsulta = await _context.Usuarios.Include(u => u.Dados).ThenInclude(d => d.Campo).Include(u => u.EmailsValidados).FirstOrDefaultAsync(u => u.UsuarioId == dadoEmail.UsuarioId);
                if(usuarioConsulta == null || usuarioConsulta.EmailsValidados.Any(e => e.Confirmado == false && String.IsNullOrEmpty(e.EmailNovo)))
                    throw new Exception(msgErro);

                if(await usuarioConsulta.VerificarLimiteValidacao())
                    throw new Exception(msgErro);

                NovaSenhaSolicitada solicitar = new NovaSenhaSolicitada(_context, usuarioConsulta);
                solicitar.key = await solicitar.GerarKey();
                await _context.NovasSenhasSolicidatas.AddAsync(solicitar);
                await _context.SaveChangesAsync();
                await solicitar.Executar(Url);

                msg = AlertMessage.Success($"Foi enviado para seu Email \"{usuarioConsulta.Dados[0].Email}\" um link para redefinição de senha.") +
                  AlertMessage.Danger("Verifique se o Email caiu na <span class=\"font-weight-bolder\">Caixa de Spam</span> ou no <span class=\"font-weight-bolder\">Lixo Eletrônico</span>.");
            } catch(Exception err) {
                return RedirectToAction("RecuperarSenha", "Recuperacao", new { msgResult = AlertMessage.Danger(err.Message) });
            }


            return RedirectToAction("IndexResult", "Login", new { msgResult = msg });
        }

        //#########################################################################################################
        [HttpGet("Recuperacao/NovaSenha/{key}/{msgResult?}")]
        public async Task<IActionResult> NovaSenha(string key, string msgResult) {

            NovaSenhaSolicitada solicitacao;
            DateTime tempoMinLimite = DateTime.Now.AddHours(-24);

            try {
                solicitacao = await _context.NovasSenhasSolicidatas.FirstOrDefaultAsync(ns => ns.key == key);

                if(solicitacao == null)
                    throw new Exception("Este link é inválido. Solicite outra Redefinição de Senha.");

                if(solicitacao.Criado < tempoMinLimite)
                    throw new Exception("Este link expirou. Solicite outra Redefinição de Senha.");

            } catch(Exception err) {
                return RedirectToAction("RecuperarSenha", "Recuperacao", new { msgResult = AlertMessage.Danger(err.Message) });
            }

            CadastroUsuarioVM cuSe = new CadastroUsuarioVM() {
                usuario = new Usuario() {
                    Dados = new List<Dado_Cadastro>(){
            new Dado_Cadastro(){Campo = await _context.CamposCadastro.FirstOrDefaultAsync(c => c.CampoGenerico == ECampoGenerico.Senha)}
          }
                },
                key = key
            };

            ViewData["msgResult"] = msgResult;

            return View(cuSe);
        }

        //#########################################################################################################
        [HttpPost]
        public async Task<IActionResult> NovaSenha(CadastroUsuarioVM cuSe) {

            NovaSenhaSolicitada solicitacao = await _context.NovasSenhasSolicidatas?.
              Include(ns => ns.Usuario).ThenInclude(u => u.NovasSenhasSolicidadas).
              Include(ns => ns.Usuario).ThenInclude(u => u.Dados).ThenInclude(d => d.Campo).
              FirstOrDefaultAsync(nss => nss.key == cuSe.key);
            DateTime tempoMinLimite = DateTime.Now.AddHours(-24);

            //verifica se fi encontrado a solicitação
            if(solicitacao == null)
                return RedirectToAction("RecuperarSenha", "Recuperacao", new { msgResult = AlertMessage.Danger("Este link é inválido. Solicite outra Redefinição de Senha.") });

            //Verificar se Excedeu o tempo limite
            if(solicitacao.Criado < tempoMinLimite)
                return RedirectToAction("RecuperarSenha", "Recuperacao", new { msgResult = AlertMessage.Danger("Este link expirou. Solicite outra Redefinição de Senha.") });

            Dado_Cadastro dadoSenha = solicitacao.Usuario.Dados.FirstOrDefault(d => d.Campo.CampoGenerico == ECampoGenerico.Senha);

            //Se as Senhas Forem iguais, Trocar Senha
            if(cuSe.usuario.Dados[0].Senha == cuSe.usuario.Dados[0].ConfirmarSenha) {
                dadoSenha.Senha = cuSe.usuario.Dados[0].GetSenhaMD5;

                _context.DadosCadastro.Update(dadoSenha);
                _context.NovasSenhasSolicidatas.RemoveRange(solicitacao.Usuario.NovasSenhasSolicidadas);
                await _context.SaveChangesAsync();
            } else {
                return RedirectToAction("IndexResult", "Login", new { msgResult = AlertMessage.Danger("As senhas não coincidem.") });
            }

            return RedirectToAction("IndexResult", "Login", new { msgResult = AlertMessage.Success("A senha foi redefinida com sucesso.") });
        }


    }
}