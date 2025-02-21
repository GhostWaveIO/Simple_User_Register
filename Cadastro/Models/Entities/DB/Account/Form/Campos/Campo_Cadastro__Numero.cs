using System;

namespace Cadastro.Models.Account.Cadastro.Campos {

  public partial class Campo_Cadastro {

    //#### Criação ####

    //###################################################################################################
    private void Criar_Numero() {

    }

    //###################################################################################################
    private void VerificarCriacao_Numero() {
      
    }

    //###################################################################################################
    private void CorrigirCriacao_Numero() {
      this.ComprimentoTextoMax = null;
      this.Html = null;
      this.CasasDecimais = 0;
      this.TextoPreenchido = null;
      this.PlaceHolderConfirmarSenha = null;
      this.ExpressaoRegular = null;
    }

    //#### Edição ####

    //###################################################################################################
    private void Update_Numero() {

    }

    //###################################################################################################
    private void VerificarEdicao_Numero() {
      if(this.Min > this.Max) throw new Exception("Valor mínimo não pode ser maior que valor máximo");
    }

    //###################################################################################################
    private void CorrigirEdicao_Numero() {

    }

    //###################################################################################################
    private void CopyToUpdate_Numero(Campo_Cadastro campo) {
      if (!this.Generico) {
        campo.Required = this.Required;
        campo.StartCriacaoCampo = this.StartCriacaoCampo;
        campo.Unico = this.Unico;
        campo.Min = this.Min;
        campo.Max = this.Max;
      }

      campo.AutorizadoEditar = this.AutorizadoEditar;
      campo.Label = this.Label;
      //campo.ComprimentoTextoMax = this.ComprimentoTextoMax;
      //campo.CasasDecimais = this.CasasDecimais;
      //campo.TextoPreenchido = this.TextoPreenchido;
      campo.NumeroPreenchido = this.NumeroPreenchido;
      campo.PlaceHolder = this.PlaceHolder;
      //campo.ExpressaoRegular = this.ExpressaoRegular;
      //campo.DirecaoEixo = this.DirecaoEixo;

    }

  }
}
