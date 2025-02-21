using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Cadastro.Data;
using Microsoft.EntityFrameworkCore;
using Cadastro.Models.Account.Cadastro.Dados;
using Cadastro.Models.Entities.DB.Account;
using Cadastro.Models.ViewModel.Application.Notifications.Alerts;

namespace Cadastro.Controllers.Account
{
    public class CredencialController : Controller
    {

        public readonly AppDbContext _context;

        //#############################################################################################
        public CredencialController(AppDbContext context)
        {
            _context = context;
        }

        //#############################################################################################
        public IActionResult Index()
        {
            return RedirectToAction("Consulta");
        }

        //#############################################################################################
        public IActionResult Consulta(string msgResult = null)
        {
            ViewData["msgResult"] = msgResult;
            return View();
        }

        //#############################################################################################
        [HttpGet("Credencial/ResultadoConsulta/{codigo}")]
        [HttpPost("Credencial/ResultadoConsulta/")]
        public async Task<IActionResult> ResultadoConsulta(string codigo)
        {
            codigo = codigo.Replace("%2F", "/").Replace('-', '/');
            if (string.IsNullOrEmpty(codigo))
                return RedirectToAction("Consultar", new { msgResult = AlertMessage.Danger("Informe um número de matrícula.") });

            Usuario res = await _context.Usuarios.Include(u => u.Dados).ThenInclude(d => d.Campo).FirstOrDefaultAsync(u => u.CodigoUsuario == codigo.Trim().ToUpper());
            foreach (Dado_Cadastro dd in res.Dados)
            {
                if (dd.Campo.Criptografar)
                    dd.Descriptografar();
            }

            return View(res);
        }

        //#############################################################################################
        [HttpGet("Credencial/ResultadoDependente/{matricula}")]
        [HttpGet("Credencial/ResultadoDependente/")]
        [HttpPost("Credencial/ResultadoDependente/")]
        public IActionResult ResultadoDependente(string matricula = "teste")
        {
            //matricula = matricula.Replace("-", "/");
            return RedirectToAction("Index", "Dependente");
        }

    }
}
