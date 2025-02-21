using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cadastro.Models.Entities.DB.Account.Form.Seletores;

namespace Cadastro.Models.Account.Cadastro.Dados
{
    public partial class Dado_Cadastro {

    public RadioButton_Checked RadioButton_Checked { get; set; }

    //#######################################################################################################################
    private void Preparar_RadioButton() {
      this.RadioButton_Checked = new RadioButton_Checked();
      int idPreChecked = Campo.RadioButtons.SingleOrDefault(r => r.PreChecked)?.RadioButton_ItemId??0;

      //Define o item Pré-Selecionado
      if(idPreChecked != 0) this.RadioButton_Checked.RadioButton_ItemId = idPreChecked;

    }

    //#######################################################################################################################
    private void VerificarCriacao_RadioButton() {
      if (!this.Campo.RadioButtons.Any()) return;
      if (this.RadioButton_Checked == null){
        if (this.Campo.Required)
          throw new Exception($"É obrigatório selecionar um item no campo \"{this.Campo.Label}\"");

        return;
      }
      

      if (this.Campo.Required && this.RadioButton_Checked?.RadioButton_ItemId == null) throw new Exception($"É obrigatório selecionar um item no campo \"{this.Campo.Label}\"");
    }

    //#######################################################################################################################
    private void CorrigirCriacao_RadioButton() {
      if (this.RadioButton_Checked == null) return;
      this.Email = null;
      this.Numero = null;
      this.NumeroMonetario = null;
      this.Senha = "";
      this.Texto250 = null;
      this.TextoLongo = null;
    }

    //#######################################################################################################################
    private async Task Criar_RadioButton() {
      if (this.RadioButton_Checked == null) return;
      await _context.AddAsync(this);
      await _context.SaveChangesAsync();

      if(this.RadioButton_Checked == null) return;

      //Acho desnecessário processo abaixo, pq pode ter sido adicionado na operação acima
      if (!await _context.RadioButtonCheckedDados.ContainsAsync(this.RadioButton_Checked)) {
        this.RadioButton_Checked.Dado_CadastroId = this.Dado_CadastroId;
        await _context.RadioButtonCheckedDados.AddAsync(this.RadioButton_Checked);
        await _context.SaveChangesAsync();
      }
    }

    //#######################################################################################################################
    private void VerificarEdicao_RadioButton() {
      if(this.RadioButton_Checked == null && this.Campo.Required) throw new Exception($"É obrigatório selecionar um item no campo \"{this.Campo.Label}\"");
      if (!this.Campo.RadioButtons.Any()) return;
      if (this.RadioButton_Checked == null) {
        if (this.Campo.Required)
          throw new Exception($"É obrigatório selecionar um item no campo \"{this.Campo.Label}\"");

        return;
      }


      if (this.Campo.Required && this.RadioButton_Checked.RadioButton_ItemId == null) throw new Exception($"É obrigatório selecionar um item no campo \"{this.Campo.Label}\"");

    }

    //#######################################################################################################################
    private void CorrigirEdicao_RadioButton() {
      
    }

    //#######################################################################################################################
    private void CopyToUpdate_RadioButton(Dado_Cadastro dado) {
      if(this.RadioButton_Checked == null) return;
      if(dado.RadioButton_Checked == null) dado.RadioButton_Checked = new RadioButton_Checked(){ Dado_CadastroId = this.Dado_CadastroId, RadioButton_ItemId = this.RadioButton_Checked.RadioButton_ItemId};

      dado.RadioButton_Checked.RadioButton_ItemId = this.RadioButton_Checked.RadioButton_ItemId;
    }

    //#######################################################################################################################
    private async Task Update_RadioButton() {
      if(this.RadioButton_Checked == null) return;

      _context.RadioButtonCheckedDados.Update(this.RadioButton_Checked);
      await _context.SaveChangesAsync();
    }

  }
}
