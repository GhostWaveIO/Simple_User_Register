using System;

namespace Cadastro.Models.Account.Cadastro.Campos {

  public partial class Campo_Cadastro {

    //#### Criação ####

    //###################################################################################################
    private void Criar_Imagem() {

    }

    //###################################################################################################
    private void VerificarCriacao_Imagem() {

    }

    //###################################################################################################
    private void CorrigirCriacao_Imagem() {
      this.ComprimentoTextoMax = null;
      this.ExpressaoRegular = null;
      this.PlaceHolder = null;
      this.PlaceHolderConfirmarSenha = null;
    }

    //#### Edição ####

    //###################################################################################################
    private void VerificarEdicao_Imagem() {

    }

    //###################################################################################################
    private void CorrigirEdicao_Imagem() {

    }

    //###################################################################################################
    private void CopyToUpdate_Imagem(Campo_Cadastro campo) {
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
      //campo.DirecaoEixo = this.DirecaoEixo;

    }

    //###################################################################################################
    private void Update_Imagem() {

    }

  }
}
