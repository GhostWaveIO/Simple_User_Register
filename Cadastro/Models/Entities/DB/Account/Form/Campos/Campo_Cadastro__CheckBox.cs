using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Cadastro.Models.ViewModel.Account.Cadastro.Campo;
using Cadastro.Models.Entities.DB.Account.Form.Seletores;

namespace Cadastro.Models.Account.Cadastro.Campos
{

    public partial class Campo_Cadastro {

    //#### Criação ####

    //###################################################################################################
    private async Task Criar_CheckBox(CriarCampoVM ccSe) {
      if (String.IsNullOrWhiteSpace(ccSe.campo.ItensSeletores)) return;

      List<string> listaStringItens = new List<string>(ccSe.campo.ItensSeletores.Split(Environment.NewLine));
      listaStringItens.RemoveAll(l => String.IsNullOrWhiteSpace(l));

      if (listaStringItens.Any(i => i.Length > 300)) throw new Exception("Um ou mais itens, ultrapassa o limite de 300 caracteres");

      //Cria os itens referentes às strings divididas acima
      int cSel = 0;
      List<CheckBox_Item> listaSeletores = new List<CheckBox_Item>();
      foreach (string itemStr in listaStringItens) {
        listaSeletores.Add(new CheckBox_Item() {
          Nome = itemStr.Replace("#P#", "").Replace("#p#", "").Trim(),
          Ordem = cSel++,
          PreChecked = itemStr.Contains("#p#"),
          Campo_CadastroId = this.Campo_CadastroId
        });

      }

      await _context.CheckBoxItens.AddRangeAsync(listaSeletores);
      await _context.SaveChangesAsync();
    }

    //###################################################################################################
    private void VerificarCriacao_CheckBox() {
      if (String.IsNullOrWhiteSpace(this.ItensSeletores)) {
        throw new Exception("Ao menos um item precisa ser informado");
      }
    }

    //###################################################################################################
    private void CorrigirCriacao_CheckBox() {
      this.Generico = false;
      this.ComprimentoTextoMax = null;
      this.Html = null;
      this.CasasDecimais = 0;
      this.Min = 0;
      this.Max = 0;
      this.TextoPreenchido = null;
      this.NumeroPreenchido = 0;
      this.PlaceHolder = null;
      this.PlaceHolderConfirmarSenha = null;
      this.ExpressaoRegular = null;
    }


    //#### Edição ####

    //###################################################################################################
    private void VerificarEdicao_CheckBox() {

    }

    //###################################################################################################
    private void CorrigirEdicao_CheckBox() {
      this.ItensSeletores_Novos = this.ItensSeletores_Novos?.Replace("#P#", "#p#");
    }

    //###################################################################################################
    private void CopyToUpdate_CheckBox(Campo_Cadastro campo) {

      if (!this.Generico) {
        campo.Required = this.Required;
        campo.StartCriacaoCampo = this.StartCriacaoCampo;
      }

      campo.AutorizadoEditar = this.AutorizadoEditar;
      campo.Label = this.Label;
      //campo.Unico = this.Unico;
      //campo.ComprimentoTextoMax = this.ComprimentoTextoMax;
      //campo.CasasDecimais = this.CasasDecimais;
      //campo.Min = this.Min;
      //campo.Max = this.Max;
      //campo.TextoPreenchido = this.TextoPreenchido;
      //campo.NumeroPreenchido = this.NumeroPreenchido;
      //campo.PlaceHolder = this.PlaceHolder;
      //campo.ExpressaoRegular = this.ExpressaoRegular;
      campo.DirecaoEixo = this.DirecaoEixo;
      campo.ItensSeletores_Novos = this.ItensSeletores_Novos;

    }

    //###################################################################################################
    private async Task Update_CheckBox(EditarCampoVM ecSe) {
      if (String.IsNullOrWhiteSpace(ecSe.campo.ItensSeletores_Novos)) return;

      List<string> listaStringItens = new List<string>(ecSe.campo.ItensSeletores_Novos.Split(Environment.NewLine));
      listaStringItens.RemoveAll(l => String.IsNullOrWhiteSpace(l));

      if (listaStringItens.Any(i => i.Length >= 300)) throw new Exception("Um ou mais itens, ultrapassa o limite de 300 caracteres");

      //Cria os itens referentes à lista de strings divididas acima
      int cSel = 0;
      if (this.CheckBoxes.Any())
        cSel = this.CheckBoxes.Max(s => s.Ordem)+1;
      List<CheckBox_Item> listaSeletores = new List<CheckBox_Item>();
      foreach (string itemStr in listaStringItens) {
        listaSeletores.Add(new CheckBox_Item() {
          Nome = itemStr.Replace("#P#", "").Replace("#p#", "").Trim(),
          Ordem = cSel++,
          PreChecked = itemStr.Contains("#p#"),
          Campo_CadastroId = this.Campo_CadastroId
        });

      }

      await _context.CheckBoxItens.AddRangeAsync(listaSeletores);
      await _context.SaveChangesAsync();
    }

  }
}
