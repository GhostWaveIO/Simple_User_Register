using System;

namespace Cadastro.Models.Account.Cadastro.Campos {

  public partial class Campo_Cadastro {

    //#### Criação ####

    //###################################################################################################
    private void Criar_NumeroMonetario() {

    }

    //###################################################################################################
    private void VerificarCriacao_NumeroMonetario() {
      if (this.CasasDecimais < 0 || this.CasasDecimais > 8)
        throw new Exception("As casas decimais devem estar entre 0 e 8");
      if (this.Min > this.Max) throw new Exception("Valor mínimo não pode ser maior que valor máximo");
    }

    //###################################################################################################
    private void CorrigirCriacao_NumeroMonetario() {
      this.ComprimentoTextoMax = null;
      this.Html = null;
      this.CasasDecimais = 2;
      this.TextoPreenchido = null;
      this.PlaceHolderConfirmarSenha = null;
      this.ExpressaoRegular = null;
    }


    //#### Edição ####

    //###################################################################################################
    private void VerificarEdicao_NumeroMonetario() {
      if (this.CasasDecimais < 0 || this.CasasDecimais > 8)
        throw new Exception("As casas decimais devem estar entre 0 e 8");
      if (this.Min > this.Max) throw new Exception("Valor mínimo não pode ser maior que valor máximo");
    }

    //###################################################################################################
    private void CorrigirEdicao_NumeroMonetario() {

    }

    //###################################################################################################
    private void CopyToUpdate_NumeroMonetario(Campo_Cadastro campo) {
      if (!this.Generico) {
        campo.Required = this.Required;
        campo.StartCriacaoCampo = this.StartCriacaoCampo;
        campo.CasasDecimais = 2;
        campo.Min = this.Min;
        campo.Max = this.Max;
        campo.Unico = this.Unico;
      }

      campo.AutorizadoEditar = this.AutorizadoEditar;
      campo.Label = this.Label;
      //campo.ComprimentoTextoMax = this.ComprimentoTextoMax;
      //campo.TextoPreenchido = this.TextoPreenchido;
      campo.NumeroPreenchido = this.NumeroPreenchido;
      campo.PlaceHolder = this.PlaceHolder;
      //campo.ExpressaoRegular = this.ExpressaoRegular;
      //campo.DirecaoEixo = this.DirecaoEixo;

    }

    //###################################################################################################
    private void Update_NumeroMonetario() {

    }

  }
}
