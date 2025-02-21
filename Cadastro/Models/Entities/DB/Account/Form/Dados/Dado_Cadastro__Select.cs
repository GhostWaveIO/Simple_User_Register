using System;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cadastro.Models.Entities.DB.Account.Form.Seletores;

namespace Cadastro.Models.Account.Cadastro.Dados
{
    public partial class Dado_Cadastro {

    [BindProperty]
    public Select_Selected Selected { get; set; }


    private void Preparar_Select() {
      this.Selected = new Select_Selected();
    }

    //#######################################################################################################################
    private void VerificarCriacao_Select() {
      if(this.Campo == null) throw new Exception("Campo não informado");
      if (!this.Campo.Selects.Any()) return;
      if (this.Selected == null) {
        if (this.Campo.Required)
          throw new Exception($"É obrigatório selecionar um item no campo \"{this.Campo.Label}\"");

        return;
      }

      if (this.Campo.Required && this.Selected.Select_ItemId == null) throw new Exception($"É obrigatório selecionar um item no campo \"{this.Campo.Label}\"");
    }

    //#######################################################################################################################
    private void CorrigirCriacao_Select() {
      
      this.Email = null;
      this.Numero = null;
      this.NumeroMonetario = null;
      this.Senha = "";
      this.Texto250 = null;
      this.TextoLongo = null;
    }

    //#######################################################################################################################
    private async Task Criar_Select() {
      await _context.AddAsync(this);
      await _context.SaveChangesAsync();

      //Acho desnecessário, pq o Select_Selected já é criado por ser vinculado ao item acima
      if(this.Selected.Select_SelectedId == 0) {
        this.Selected.Dado_CadastroId = this.Dado_CadastroId;
        await _context.SelectSelectedDados.AddAsync(this.Selected);
        await _context.SaveChangesAsync();
      }
    }

    //#######################################################################################################################
    private void VerificarEdicao_Select() {
      if (this.Campo == null) throw new Exception("Campo não informado");

      if (!this.Campo.Selects.Any()) return;
      if (this.Selected == null) {
        if (this.Campo.Required)
          throw new Exception($"É obrigatório selecionar um item no campo \"{this.Campo.Label}\"");

        return;
      }

      if (this.Campo.Required && this.Selected.Select_ItemId == null) throw new Exception($"Escolha ao menos uma opção no campo \"{this.Campo.Label}\".");
    }

    //#######################################################################################################################
    private void CorrigirEdicao_Select() {

    }

    //#######################################################################################################################
    private void CopyToUpdate_Select(Dado_Cadastro dado) {
      if (this.Selected == null) return;

      dado.Selected.Select_ItemId = this.Selected.Select_ItemId;
    }

    //#######################################################################################################################
    private async Task Update_Select() {
      if(this.Selected == null) return;
      if(!await _context.DadosCadastro.ContainsAsync(this)) throw new Exception($"Este dado não existe no campo \"{this.Campo.Label}\"");

      _context.SelectSelectedDados.Update(this.Selected);
      await _context.SaveChangesAsync();
    }

  }
}
