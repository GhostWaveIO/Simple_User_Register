using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Cadastro.Models.Services.Application.StaticPages.ActionResult;

namespace Cadastro.Controllers.Application.StaticPages
{
    public class PaginaController : Controller
    {


        //##############################################################################################################################
        [HttpGet("")]
        [HttpGet("Pagina")]
        public async Task<IActionResult> Index()
        {


            WebContent webContet = new WebContent(Url);

            ActionResult content;

            if (webContet.IsValid)
            {
                content = (ActionResult)await webContet.GetResult();
            }
            else
            {
                content = RedirectToAction("Index", "Login");
            }

            webContet.Dispose();

            return content;
        }

        //###################################################################################################
        [HttpGet("Pagina/{arquivo}/")]
        public async Task<IActionResult> Index(string arquivo)
        {


            WebContent webContet = new WebContent(Url, arquivo);

            ActionResult content;

            if (webContet.IsValid)
            {
                content = (ActionResult)await webContet.GetResult();
            }
            else
            {
                content = new ContentResult();
            }

            webContet.Dispose();

            return content;

        }

        //###################################################################################################
        [HttpGet("Pagina/Reloading/iframe")]
        public IActionResult Reloading()
        {
            return new ContentResult()
            {
                Content = "<!DOCTYPE html><html><head><meta http-equiv=\"refresh\" content=\"240\"></head><body></body></html>",
                ContentType = "text/html"
            };
        }

    }
}
