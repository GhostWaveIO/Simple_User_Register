using System;
using System.Text.RegularExpressions;

namespace Cadastro.Models.Account.Cadastro.Campos {

  public partial class Campo_Cadastro {

    //#### Criação ####

    //###################################################################################################
    private void Criar_Email() {

    }

    //###################################################################################################
    private void VerificarCriacao_Email() {
      
    }

    //###################################################################################################
    private void CorrigirCriacao_Email() {
      this.ComprimentoTextoMax = null;
      this.ExpressaoRegular = null;
    }


    //#### Edição ####

    //###################################################################################################
    private void VerificarEdicao_Email() {

    }

    //###################################################################################################
    private void CorrigirEdicao_Email() {

    }

    //###################################################################################################
    private void CopyToUpdate_Email(Campo_Cadastro campo) {
      if (!this.Generico) {
        campo.Required = this.Required;
        campo.StartCriacaoCampo = this.StartCriacaoCampo;
        campo.Unico = this.Unico;
      }
      campo.PermitirPesquisa = this.PermitirPesquisa;

      campo.AutorizadoEditar = this.AutorizadoEditar;
      campo.Label = this.Label;
      //campo.ComprimentoTextoMax = this.ComprimentoTextoMax;
      //campo.CasasDecimais = this.CasasDecimais;
      //campo.Min = this.Min;
      //campo.Max = this.Max;
      //campo.TextoPreenchido = this.TextoPreenchido;
      //campo.NumeroPreenchido = this.NumeroPreenchido;
      campo.PlaceHolder = this.PlaceHolder;
      //campo.ExpressaoRegular = this.ExpressaoRegular;
      //campo.DirecaoEixo = this.DirecaoEixo;

    }

    //###################################################################################################
    private void Update_Email() {

    }

  }
}
