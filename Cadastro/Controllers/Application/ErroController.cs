using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cadastro.Data;
using Cadastro.Models;
using Cadastro.Models.ViewModel.Application.Notifications.Alerts;
using Cadastro.Models.ViewModel.Application.AccessContext;
using Cadastro.Models.Services.Application.Logs;

namespace Cadastro.Controllers.Application
{
    public class ErroController : Controller
    {

        public readonly AppDbContext _context;
        public ErroController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar(); ViewData["acesso"] = acesso;
            if (acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            if (!acesso._permissao.AcessoTotal)
                return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });
            #endregion

            return View(ErroFileService.GetListaErros());
        }
    }
}
