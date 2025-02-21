using Cadastro.Data;
using Cadastro.Models.Account.Cadastro.Campos;
using Cadastro.Models.ViewModel.Application.AccessContext;
using Cadastro.Models.ViewModel.Application.Notifications.Alerts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Cadastro.Controllers
{
    public class HomeController : Controller {
    public readonly AppDbContext _context;

    public HomeController(AppDbContext context) {
      _context = context;
    }

    public async Task<IActionResult> Index(string msgResult) {
      #region Local de Acesso
      AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
      await acesso.Consultar(); ViewData["acesso"] = acesso;
      if (acesso._usuario == null)
        return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
      #endregion



      ViewData["totalUsuarios"] = await _context.Usuarios.
        Include(u => u.Dados).ThenInclude(d => d.Campo).
        Where(u => u.Dados.Any(d => d.Campo.CampoGenerico == ECampoGenerico.Email && d.Email != Program.Config.emailPrincipal)).CountAsync();
      ViewData["msgResult"] = msgResult;

      return View();
    }

    public async Task<IActionResult> Logoff() {
      #region Local de Acesso
      AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
      await acesso.Consultar(); ViewData["acesso"] = acesso;
      if (acesso._usuario == null)
        return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
      #endregion

      HttpContext.Session.Clear();
      return RedirectToAction("Index", "Login");
    }

  }
}
