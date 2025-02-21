using System;

namespace Cadastro.Models.Account.Cadastro.Campos {

  public partial class Campo_Cadastro {

    //#### Criação ####

    //###################################################################################################
    private void Criar_Html() {

    }

    //###################################################################################################
    private void VerificarCriacao_Html() {
      if(String.IsNullOrWhiteSpace(this.Html)) return;
      if(this.Html.Length > 3000) throw new Exception("Máximo 3000 caracteres!");
    }

    //###################################################################################################
    private void CorrigirCriacao_Html() {
      this.ComprimentoTextoMax = null;
      this.TextoPreenchido = null;
      this.PlaceHolder = null;
      this.PlaceHolderConfirmarSenha = null;
      this.ExpressaoRegular = null;
      if(String.IsNullOrWhiteSpace(this.Html)) this.Html = null;
    }

    //#### Edição ####

    //###################################################################################################
    private void VerificarEdicao_Html() {
      if (String.IsNullOrWhiteSpace(this.Html)) return;
      if (this.Html.Length > 3000) throw new Exception("Máximo 3000 caracteres!");
    }

    //###################################################################################################
    private void CorrigirEdicao_Html() {

    }

    //###################################################################################################
    private void CopyToUpdate_Html(Campo_Cadastro campo) {
      /*if (!this.Generico) {
        campo.Required = this.Required;
      }*/

      //campo.StartCriacaoCampo = this.StartCriacaoCampo;
      //campo.AutorizadoEditar = this.AutorizadoEditar;
      campo.Label = this.Label;
      if (String.IsNullOrWhiteSpace(this.Html)) this.Html = null;
      campo.Html = this.Html;
      //campo.Unico = this.Unico;
      //campo.ComprimentoTextoMax = this.ComprimentoTextoMax;
      //campo.CasasDecimais = this.CasasDecimais;
      //campo.Min = this.Min;
      //campo.Max = this.Max;
      //campo.TextoPreenchido = this.TextoPreenchido;
      //campo.NumeroPreenchido = this.NumeroPreenchido;
      //campo.PlaceHolder = this.PlaceHolder;
      //campo.ExpressaoRegular = this.ExpressaoRegular;
      //campo.DirecaoEixo = this.DirecaoEixo;
    }

    //###################################################################################################
    private void Update_Html() {

    }

  }
}
