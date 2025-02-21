using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cadastro.Data;
using Cadastro.Models.ViewModel.Account.Cadastro.Seletores;
using Cadastro.Models.Entities.DB.Account.Form.Seletores;
using Cadastro.Models.ViewModel.Application.Notifications.Alerts;
using Cadastro.Models.ViewModel.Application.AccessContext;

namespace Cadastro.Controllers.Account.Register.Form
{
    public class SeletoresCadastroController : Controller
    {

        public AppDbContext _context { get; set; }

        public SeletoresCadastroController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        //##################################################################################################
        [HttpGet]
        public async Task<IActionResult> Editar_Select(int id, string msgResult)
        {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar(); ViewData["acesso"] = acesso;
            if (acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            #endregion

            string msg = string.Empty;
            CriarSelectCadastroVM csSe = new CriarSelectCadastroVM();

            try
            {
                csSe.select = await _context.SelectItens.Include(s => s.Campo_Cadastro).FirstOrDefaultAsync(s => s.Select_ItemId == id);
                if (csSe.select == null) throw new Exception("Opção não encontrada.");

                if (!(acesso._permissao.EditarCampoGenericoCadastro && csSe.select.Campo_Cadastro.Generico || acesso._permissao.EditarCampoPersonalizadoCadastro && !csSe.select.Campo_Cadastro.Generico))
                    return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });

                msg = AlertMessage.Success("Alterações salvas com sucesso!");
            }
            catch (Exception err)
            {
                msg = AlertMessage.Danger(err.Message);
            }

            ViewData["msgResult"] = msgResult;

            return View("../Cadastro/Seletores/Editar_Select", csSe);
        }

        //##################################################################################################
        [HttpPost]
        public async Task<IActionResult> Editar_Select(CriarSelectCadastroVM csSe)
        {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar(); ViewData["acesso"] = acesso;
            if (acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            #endregion

            string msg = "";
            Select_Item select = null;

            try
            {
                select = await _context.SelectItens.Include(s => s.Campo_Cadastro).FirstOrDefaultAsync(s => s.Select_ItemId == csSe.select.Select_ItemId);
                if (select == null) throw new Exception("Opção não encontrada.");
                if (!(acesso._permissao.EditarCampoGenericoCadastro && select.Campo_Cadastro.Generico || acesso._permissao.EditarCampoPersonalizadoCadastro && !select.Campo_Cadastro.Generico))
                    return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });

                select._context = _context;

                csSe.select.VerificarEdicao();
                csSe.select.CorrigirEdicao();

                csSe.select.CopyToUpdate(select);
                await select.Update();

                msg = AlertMessage.Success("Alterações salvas com sucesso!");
            }
            catch (Exception err)
            {
                msg = AlertMessage.Danger(err.Message);
                return RedirectToAction("Index", "Configuracao", new { msgResult = msg });
            }

            return RedirectToAction("Editar_Select", "SeletoresCadastro", new { id = csSe.select.Select_ItemId, msgResult = msg });
        }

        //##################################################################################################
        [HttpGet]
        public async Task<IActionResult> Editar_CheckBox(int id, string msgResult)
        {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar(); ViewData["acesso"] = acesso;
            if (acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            #endregion

            string msg = string.Empty;
            CriarCheckBoxCadastroVM csSe = new CriarCheckBoxCadastroVM();

            try
            {
                csSe.check = await _context.CheckBoxItens.Include(c => c.Campo_Cadastro).FirstOrDefaultAsync(s => s.CheckBox_ItemId == id);
                if (csSe.check == null) throw new Exception("Opção não encontrada.");
                if (!(acesso._permissao.EditarCampoGenericoCadastro && csSe.check.Campo_Cadastro.Generico || acesso._permissao.EditarCampoPersonalizadoCadastro && !csSe.check.Campo_Cadastro.Generico))
                    return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });

                msg = AlertMessage.Success("Alterações salvas com sucesso!");
            }
            catch (Exception err)
            {
                msg = AlertMessage.Danger(err.Message);
            }
            ViewData["msgResult"] = msgResult;


            return View("../Cadastro/Seletores/Editar_CheckBox", csSe);
        }

        //##################################################################################################
        [HttpPost]
        public async Task<IActionResult> Editar_CheckBox(CriarCheckBoxCadastroVM ccSe)
        {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar(); ViewData["acesso"] = acesso;
            if (acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            #endregion

            string msg = string.Empty;
            CheckBox_Item check = null;

            try
            {
                check = await _context.CheckBoxItens.Include(s => s.Campo_Cadastro).FirstOrDefaultAsync(c => c.CheckBox_ItemId == ccSe.check.CheckBox_ItemId);
                if (check == null) throw new Exception("Opção não encontrada.");
                if (!(acesso._permissao.EditarCampoGenericoCadastro && check.Campo_Cadastro.Generico || acesso._permissao.EditarCampoPersonalizadoCadastro && !check.Campo_Cadastro.Generico))
                    return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });

                check._context = _context;

                ccSe.check.VerificarEdicao();
                ccSe.check.CorrigirEdicao();

                ccSe.check.CopyToUpdate(check);

                await check.Update();

                msg = AlertMessage.Success("Alterações salvas com sucesso!");
            }
            catch (Exception err)
            {
                msg = AlertMessage.Danger(err.Message);
                return RedirectToAction("Index", "Configuracao", new { msgResult = msg });
            }

            return RedirectToAction("Editar_CheckBox", "SeletoresCadastro", new { id = ccSe.check.CheckBox_ItemId, msgResult = msg });
        }

        //##################################################################################################
        [HttpGet]
        public async Task<IActionResult> Editar_RadioButton(int id, string msgResult)
        {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar(); ViewData["acesso"] = acesso;
            if (acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            #endregion

            string msg = string.Empty;
            CriarRadioButtonCadastroVM csSe = new CriarRadioButtonCadastroVM();

            try
            {
                csSe.radio = await _context.RadioButtonItens.Include(r => r.Campo_Cadastro).FirstOrDefaultAsync(s => s.RadioButton_ItemId == id);
                if (csSe.radio == null) throw new Exception("Opção não encontrada.");
                if (!(acesso._permissao.EditarCampoGenericoCadastro && csSe.radio.Campo_Cadastro.Generico || acesso._permissao.EditarCampoPersonalizadoCadastro && !csSe.radio.Campo_Cadastro.Generico))
                    return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });

                msg = AlertMessage.Success("Alterações salvas com sucesso!");
            }
            catch (Exception err)
            {
                msg = AlertMessage.Danger(err.Message);
            }

            ViewData["msgResult"] = msgResult;

            return View("../Cadastro/Seletores/Editar_RadioButton", csSe);
        }

        //##################################################################################################
        [HttpPost]
        public async Task<IActionResult> Editar_RadioButton(CriarRadioButtonCadastroVM crSe)
        {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar(); ViewData["acesso"] = acesso;
            if (acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            #endregion

            string msg = string.Empty;
            RadioButton_Item radio = null;

            try
            {
                radio = await _context.RadioButtonItens.Include(s => s.Campo_Cadastro).FirstOrDefaultAsync(r => r.RadioButton_ItemId == crSe.radio.RadioButton_ItemId);
                if (radio == null) throw new Exception("Opção não encontrada.");
                if (!(acesso._permissao.EditarCampoGenericoCadastro && radio.Campo_Cadastro.Generico || acesso._permissao.EditarCampoPersonalizadoCadastro && !radio.Campo_Cadastro.Generico))
                    return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });

                radio._context = _context;

                crSe.radio.VerificarEdicao();
                crSe.radio.CorrigirEdicao();

                crSe.radio.CopyToUpdate(radio);
                await radio.Update();

                msg = AlertMessage.Success("Alterações salvas com sucesso!");
            }
            catch (Exception err)
            {
                msg = AlertMessage.Danger(err.Message);
                return RedirectToAction("Index", "Configuracao", new { msgResult = msg });
            }

            return RedirectToAction("Editar_RadioButton", "SeletoresCadastro", new { id = crSe.radio.RadioButton_ItemId, msgResult = msg });
        }

        //##################################################################################################
        public async Task<IActionResult> PreSelecionar_Select(int id)
        {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar(); ViewData["acesso"] = acesso;
            if (acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            #endregion

            string msg = "";
            Select_Item select = null;

            try
            {
                //Procurar Seletor
                select = await _context.SelectItens.Include(s => s.Campo_Cadastro).ThenInclude(c => c.Selects).FirstOrDefaultAsync(s => s.Select_ItemId == id);
                if (select == null) throw new Exception("Opção não encontrada.");
                if (!(acesso._permissao.EditarCampoGenericoCadastro && select.Campo_Cadastro.Generico || acesso._permissao.EditarCampoPersonalizadoCadastro && !select.Campo_Cadastro.Generico))
                    return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });

                select._context = _context;
                await select.PreSelecionar();
                msg = AlertMessage.Success("Opção alterada com sucesso!");
            }
            catch (Exception err)
            {
                msg = AlertMessage.Danger(err.Message);
                return RedirectToAction("Index", "Configuracao", new { msgResult = msg });
            }

            return RedirectToAction("EditarCampo", "Cadastro", new { id = select.Campo_CadastroId, msgResult = msg });
        }

        //##################################################################################################
        public async Task<IActionResult> PreSelecionar_CheckBox(int id)
        {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar(); ViewData["acesso"] = acesso;
            if (acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            #endregion

            string msg = "";
            CheckBox_Item check = null;

            try
            {
                //Procurar Seletor
                check = await _context.CheckBoxItens.Include(s => s.Campo_Cadastro).ThenInclude(c => c.CheckBoxes).FirstOrDefaultAsync(s => s.CheckBox_ItemId == id);
                if (check == null) throw new Exception("Opção não encontrada.");
                if (!(acesso._permissao.EditarCampoGenericoCadastro && check.Campo_Cadastro.Generico || acesso._permissao.EditarCampoPersonalizadoCadastro && !check.Campo_Cadastro.Generico))
                    return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });

                check._context = _context;
                await check.PreSelecionar();
                msg = AlertMessage.Success("Opção alterada com sucesso!");
            }
            catch (Exception err)
            {
                msg = AlertMessage.Danger(err.Message);
                return RedirectToAction("Index", "Configuracao", new { msgResult = msg });
            }

            return RedirectToAction("EditarCampo", "Cadastro", new { id = check.Campo_CadastroId, msgResult = msg });
        }

        //##################################################################################################
        public async Task<IActionResult> PreSelecionar_RadioButton(int id)
        {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar(); ViewData["acesso"] = acesso;
            if (acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            #endregion

            string msg = "";
            RadioButton_Item radio = null;

            try
            {
                //Procurar Seletor
                radio = await _context.RadioButtonItens.Include(s => s.Campo_Cadastro).ThenInclude(c => c.RadioButtons).FirstOrDefaultAsync(s => s.RadioButton_ItemId == id);
                if (radio == null) throw new Exception("Opção não encontrada.");
                if (!(acesso._permissao.EditarCampoGenericoCadastro && radio.Campo_Cadastro.Generico || acesso._permissao.EditarCampoPersonalizadoCadastro && !radio.Campo_Cadastro.Generico))
                    return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });

                radio._context = _context;
                await radio.PreSelecionar();
                msg = AlertMessage.Success("Opção alterada com sucesso!");
            }
            catch (Exception err)
            {
                msg = AlertMessage.Danger(err.Message);
                return RedirectToAction("Index", "Configuracao", new { msgResult = msg });
            }

            return RedirectToAction("EditarCampo", "Cadastro", new { id = radio.Campo_CadastroId, msgResult = msg });
        }

        //##################################################################################################
        public async Task<IActionResult> Remover_Select(int id)
        {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar(); ViewData["acesso"] = acesso;
            if (acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            #endregion

            string msg = "";
            int idCampo = 0;
            Select_Item opcao = null;

            try
            {
                //consultar Opção
                opcao = await _context.SelectItens.Include(s => s.Campo_Cadastro).ThenInclude(cmp => cmp.Selects).FirstOrDefaultAsync(s => s.Select_ItemId == id);
                if (opcao == null) throw new Exception("Opção não encontrada.");
                if (!(acesso._permissao.EditarCampoGenericoCadastro && opcao.Campo_Cadastro.Generico || acesso._permissao.EditarCampoPersonalizadoCadastro && !opcao.Campo_Cadastro.Generico))
                    return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });

                idCampo = opcao.Campo_CadastroId;

                //Remover
                await opcao.Remover();

                msg = AlertMessage.Success("Opção removida com sucesso!");
            }
            catch (Exception err)
            {
                msg = AlertMessage.Danger(err.Message);
                return RedirectToAction("Index", "Configuracao", new { msgResult = msg });
            }

            return RedirectToAction("EditarCampo", "Cadastro", new { id = idCampo, msgResult = msg });
        }

        //##################################################################################################
        public async Task<IActionResult> Remover_Checkbox(int id)
        {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar(); ViewData["acesso"] = acesso;
            if (acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            #endregion

            string msg = "";
            int idCampo = 0;
            CheckBox_Item opcao = null;

            try
            {
                //consultar Opção
                opcao = await _context.CheckBoxItens.Include(ck => ck.Campo_Cadastro).ThenInclude(cmp => cmp.CheckBoxes).FirstOrDefaultAsync(s => s.CheckBox_ItemId == id);
                if (opcao == null) throw new Exception("Opção não encontrada.");
                if (!(acesso._permissao.EditarCampoGenericoCadastro && opcao.Campo_Cadastro.Generico || acesso._permissao.EditarCampoPersonalizadoCadastro && !opcao.Campo_Cadastro.Generico))
                    return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });

                idCampo = opcao.Campo_CadastroId;

                //Remover
                await opcao.Remover();

                msg = AlertMessage.Success("Opção removida com sucesso!");
            }
            catch (Exception err)
            {
                msg = AlertMessage.Danger(err.Message);
                return RedirectToAction("Index", "Configuracao", new { msgResult = msg });
            }

            return RedirectToAction("EditarCampo", "Cadastro", new { id = idCampo, msgResult = msg });
        }

        //##################################################################################################
        public async Task<IActionResult> Remover_RadioButton(int id)
        {
            #region Local de Acesso
            AcessoServiceVM acesso = new AcessoServiceVM(_context, HttpContext);
            await acesso.Consultar(); ViewData["acesso"] = acesso;
            if (acesso._usuario == null)
                return RedirectToAction("Index", "Login", new { msgResult = AlertMessage.Info("Efetue login para ter acesso!") });
            #endregion

            string msg = "";
            int idCampo = 0;
            RadioButton_Item opcao = null;

            try
            {
                //consultar Opção
                opcao = await _context.RadioButtonItens.Include(r => r.Campo_Cadastro).ThenInclude(cmp => cmp.RadioButtons).FirstOrDefaultAsync(s => s.RadioButton_ItemId == id);
                if (opcao == null) throw new Exception("Opção não encontrada.");
                if (!(acesso._permissao.EditarCampoGenericoCadastro && opcao.Campo_Cadastro.Generico || acesso._permissao.EditarCampoPersonalizadoCadastro && !opcao.Campo_Cadastro.Generico))
                    return RedirectToAction("Index", "Home", new { msgResult = AlertMessage.Danger("Você não tem permissão!") });

                idCampo = opcao.Campo_CadastroId;

                //Remover
                await opcao.Remover();

                msg = AlertMessage.Success("Opção removida com sucesso!");
            }
            catch (Exception err)
            {
                msg = AlertMessage.Danger(err.Message);
                return RedirectToAction("Index", "Configuracao", new { msgResult = msg });
            }

            return RedirectToAction("EditarCampo", "Cadastro", new { id = idCampo, msgResult = msg });
        }

    }
}
