using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cadastro.Data;
using Cadastro.Models.Entities.DB.Account.Form;
using Cadastro.Models.ViewModel.Application.Settings.Configurations;
using Cadastro.Models.ViewModel.Application.Notifications.Alerts;
using Cadastro.Models.ViewModel.Application.AccessContext;

namespace Cadastro.Controllers.Application.Settings
{
    public class ConfiguracaoController : Controller
    {

        public readonly AppDbContext _context;

        public ConfiguracaoController(AppDbContext context) { _context = context; }

        //############################################################################################
        public async Task<IActionResult> Index(string msgResult)
        {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar(); ViewData["acesso"] = acesso;
            if (acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if (!(acesso._permissao.ManipularHierarquias || acesso._permissao.AnyEstruturaCadastro()))
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });
            #endregion

            ConfiguracaoVM coSe = new ConfiguracaoVM(_context)
            {
                msgResult = string.Empty,
                listaHierarquias = await new ConfiguracaoVM(_context).listaHierarquia(acesso._usuario),
                listaGrupos_Cadastro = await _context.GruposCadastro?.Include(g => g.Linhas).ThenInclude(l => l.Campos).ToListAsync() ?? new List<Grupo_Cadastro>()
            };

            ViewData["msgResult"] = msgResult;

            await _context.DisposeAsync();
            GC.Collect(0, GCCollectionMode.Forced);

            return View(coSe);
        }

        //############################################################################################
        //[HttpGet("Configuracao/IndexResult/{msgResult}")]
        public async Task<IActionResult> IndexResult(string msgResult)
        {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar(); ViewData["acesso"] = acesso;
            if (acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if (!(acesso._permissao.ManipularHierarquias || acesso._permissao.AnyEstruturaCadastro()))
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });
            #endregion

            ConfiguracaoVM coSe = new ConfiguracaoVM(_context)
            {
                listaHierarquias = await new ConfiguracaoVM(_context).listaHierarquia(acesso._usuario)
            };

            ViewData["msgResult"] = msgResult;

            return View("Index", coSe);
        }


    }
}
