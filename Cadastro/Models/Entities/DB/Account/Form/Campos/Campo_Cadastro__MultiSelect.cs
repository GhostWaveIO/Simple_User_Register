using System;

namespace Cadastro.Models.Account.Cadastro.Campos {

  public partial class Campo_Cadastro {

    //#### Criação ####

    //###################################################################################################
    private void Criar_MultiSelect() {

    }

    //###################################################################################################
    private void VerificarCriacao_MultiSelect() {

    }

    //###################################################################################################
    private void CorrigirCriacao_MultiSelect() {

    }

    //#### Edição ####

    //###################################################################################################
    private void Update_MultiSelect() {

    }

    //###################################################################################################
    private void VerificarEdicao_MultiSelect() {

    }

    //###################################################################################################
    private void CorrigirEdicao_MultiSelect() {

    }

    //###################################################################################################
    private void CopyToUpdate_MultiSelect(Campo_Cadastro campo) {
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

    }

  }
}
