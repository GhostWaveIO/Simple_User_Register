using System;

namespace Cadastro.Models.Account.Cadastro.Campos {

  public partial class Campo_Cadastro {

    //#### Criação ####
    //###################################################################################################
    private void Criar_TextoLongo() {

    }

    //###################################################################################################
    private void VerificarCriacao_TextoLongo() {
      if (this.ComprimentoTextoMax < 0 || this.ComprimentoTextoMax > 3000)
        throw new Exception("O comprimento deve ser entre 0 e 3000");
    }

    //###################################################################################################
    private void CorrigirCriacao_TextoLongo() {
      this.Html = String.Empty;
      this.CasasDecimais = 0;
      this.Min = 0;
      this.Max = 0;
      this.NumeroPreenchido = 0f;
      this.PlaceHolderConfirmarSenha = String.Empty;
    }

    //#### Edição ####

    //###################################################################################################
    private void VerificarEdicao_TextoLongo() {
      if (this.ComprimentoTextoMax < 0 || this.ComprimentoTextoMax > 3000)
        throw new Exception("O comprimento deve ser entre 0 e 3000");
    }

    //###################################################################################################
    private void CorrigirEdicao_TextoLongo() {

    }

    //###################################################################################################
    private void CopyToUpdate_TextoLongo(Campo_Cadastro campo) {
      if (!this.Generico) {
        campo.Required = this.Required;
        campo.StartCriacaoCampo = this.StartCriacaoCampo;
        campo.Unico = this.Unico;
        campo.ComprimentoTextoMax = this.ComprimentoTextoMax;
      }

      campo.AutorizadoEditar = this.AutorizadoEditar;
      campo.Label = this.Label;
      //campo.CasasDecimais = this.CasasDecimais;
      //campo.Min = this.Min;
      //campo.Max = this.Max;
      campo.TextoPreenchido = this.TextoPreenchido?.Trim();
      //campo.NumeroPreenchido = this.NumeroPreenchido;
      campo.PlaceHolder = this.PlaceHolder;
      //campo.ExpressaoRegular = this.ExpressaoRegular;
      //campo.DirecaoEixo = this.DirecaoEixo;

    }

    //###################################################################################################
    private void Update_TextoLongo() {

    }

  }
}
