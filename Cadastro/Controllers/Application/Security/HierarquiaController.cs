using Cadastro.Data;
using Cadastro.Models.ViewModel.Account;
using Cadastro.Models.ViewModel.Application.AccessContext;
using Cadastro.Models.ViewModel.Application.Notifications.Alerts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Cadastro.Models.ViewModel.Application.Security.Permissions;
using Cadastro.Models.Services.Application.Security.Autorizacao;

namespace Cadastro.Controllers.Application.Security
{
    public class HierarquiaController : Controller
    {

        public readonly AppDbContext _context;

        public HierarquiaController(AppDbContext context) { _context = context; }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        //############################################################################################
        public async Task<IActionResult> NovaHierarquia()
        {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar();
            ViewData["acesso"] = acesso;
            if (acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if (!acesso._permissao.ManipularHierarquias)
                return RedirectToAction("Index", "Home", new UsuarioVM() { msgResult = AlertMessage.Danger("Você não tem permissão!") });
            #endregion

            HierarquiaServiceVM hiSe = new HierarquiaServiceVM();

            ViewData["qtdHierarquias"] = await _context.Hierarquias.CountAsync();

            return View(hiSe);
        }

        //############################################################################################
        public async Task<IActionResult> CriarHierarquia(HierarquiaServiceVM hiSe)
        {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar(); ViewData["acesso"] = acesso;
            if (acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if (!acesso._permissao.ManipularHierarquias)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });
            #endregion
            int ordemDisponivel = 1;
            if (await _context.Hierarquias.AnyAsync())
                ordemDisponivel = await _context.Hierarquias.MaxAsync(h => h.Ordem) + 1;

            string msg = "";

            hiSe.hierarquia.Ordem = ordemDisponivel;
            hiSe.hierarquia.context = _context;

            try
            {
                if (hiSe.hierarquia.PermissaoHierarquia.SomenteAdminPrincipal && !acesso.IsAdmin())
                    throw new Exception("Você não tem permissão!");
                await _context.Hierarquias.AddAsync(hiSe.hierarquia);
                await _context.SaveChangesAsync();

                await hiSe.hierarquia.ReposicionarOrdem();
                msg = AlertMessage.Success("Permissão Criada!");
            }
            catch (Exception err)
            {
                msg = AlertMessage.Danger(err.Message);
            }

            return RedirectToAction("Index", "Configuracao", new { msgResult = msg });
        }

        //############################################################################################
        [HttpGet]
        public async Task<IActionResult> EditarHierarquia(int id, string msgResult)
        {

            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar(); ViewData["acesso"] = acesso;
            if (acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if (!acesso._permissao.ManipularHierarquias)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });
            #endregion

            Hierarquia hierarquiaConsulta = await _context.Hierarquias?.
              Include(h => h.PermissaoHierarquia).
              FirstOrDefaultAsync(h => h.HierarquiaId == id);
            if (hierarquiaConsulta == null) return RedirectToAction("Index", "Configuracao", new { msgResult = AlertMessage.Danger("Hierarquia não encontrada!") });

            //Verifica permissão "acesso somente Admin"
            if (hierarquiaConsulta.PermissaoHierarquia.SomenteAdminPrincipal && !acesso.IsAdmin())
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão") });

            HierarquiaServiceVM hiSe = new HierarquiaServiceVM();
            hiSe.hierarquia = hierarquiaConsulta;
            ViewData["msgResult"] = msgResult;

            return View(hiSe);
        }

        //############################################################################################
        [HttpPost]
        public async Task<IActionResult> EditarHierarquia(HierarquiaServiceVM hiSe)
        {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar(); ViewData["acesso"] = acesso;
            if (acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if (!acesso._permissao.ManipularHierarquias)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });
            #endregion
            hiSe.hierarquia.context = _context;

            //Apagar
            if (hiSe.Apagar)
            {
                Hierarquia hierarquiaApagar = await _context.Hierarquias.
                  Include(h => h.PermissaoHierarquia).
                  Include(h => h.HierarquiasUsuario).
                  FirstOrDefaultAsync(h => h.HierarquiaId == hiSe.hierarquia.HierarquiaId);

                //Verifica permissão "acesso somente Admin"
                if (hierarquiaApagar.PermissaoHierarquia.SomenteAdminPrincipal && !acesso.IsAdmin())
                    return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão") });

                if (hierarquiaApagar.HierarquiaId == 1 || hierarquiaApagar.HierarquiaId == 2)
                    return RedirectToAction("Index", "Configuracao", new { msgResult = AlertMessage.Warning("Este nível é estático e não é possível ser removido!") });


                _context.HierarquiasUsuario.RemoveRange(hierarquiaApagar.HierarquiasUsuario);

                _context.Hierarquias.Remove(hierarquiaApagar);
                await _context.SaveChangesAsync();

                await hiSe.hierarquia.ReposicionarOrdem();

                return RedirectToAction("Index", "Configuracao", new { msgResult = AlertMessage.Success("O Nível foi Removido da Hierarquia!") });
            }//Fim Apagar


            string msgResult = "";
            Hierarquia hierarquia = await _context.Hierarquias.Include(h => h.HierarquiasUsuario).Include(h => h.PermissaoHierarquia).
              FirstOrDefaultAsync(h => h.HierarquiaId == hiSe.hierarquia.HierarquiaId);
            hierarquia.context = _context;
            if (hierarquia == null) return RedirectToAction("Index", "Configuracao", new { msgResult = AlertMessage.Danger("Permissão não encontrada!") });

            if (hierarquia.PermissaoHierarquia.SomenteAdminPrincipal && !acesso.IsAdmin()) return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });

            //Hierarquia hierarquiaOld;
            try
            {

                PermissaoHierarquia permissao = await _context.PermissoesHierarquias.FirstOrDefaultAsync(ph => ph.PermissaoHierarquiaId == hiSe.permissao.PermissaoHierarquiaId);
                hiSe.hierarquia.PermissaoHierarquia.CopyTo(permissao, acesso._usuario);

                _context.PermissoesHierarquias.Update(permissao);
                hiSe.hierarquia.CopyTo(hierarquia);
                _context.Hierarquias.Update(hierarquia);

                await _context.SaveChangesAsync();

                msgResult = AlertMessage.Success("As alterações foram salvas!");
            }
            catch (Exception err)
            {
                msgResult = AlertMessage.Danger("Erro: " + err.Message);
            }

            if (hiSe.Apagar)
            {
                return RedirectToAction("Index", "Configuracao", new { msgResult });
            }

            return RedirectToAction("EditarHierarquia", "Hierarquia", new { id = hiSe.hierarquia.HierarquiaId, msgResult });
        }

        //############################################################################################
        public async Task<IActionResult> Subir(int id)
        {
            Hierarquia h = await _context.Hierarquias.FirstOrDefaultAsync(h => h.HierarquiaId == id);
            if (h == null) return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Este Nível não foi encontrado, pode ter sido removido!") });

            await h.SubirOrdem(id);

            return RedirectToAction("Index", "Configuracao");
        }

        //############################################################################################
        public async Task<IActionResult> Descer(int id)
        {
            Hierarquia h = await _context.Hierarquias.FirstOrDefaultAsync(h => h.HierarquiaId == id);
            if (h == null) return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Este Nível não foi encontrado, pode ter sido removido!") });

            await h.DescerOrdem(id);

            return RedirectToAction("Index", "Configuracao");
        }
    }
}
