using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Cadastro.Models.Account.Cadastro.Campos {

  public partial class Campo_Cadastro {

    //#### Criação ####

    //###################################################################################################
    private void Criar_Documento() {

    }

    //###################################################################################################
    private void VerificarCriacao_Documento() {
      if(String.IsNullOrWhiteSpace(this.FormatosArquivo)) throw new Exception("Nenhum formato de arquivo foi informado");
    }

    //###################################################################################################
    private void CorrigirCriacao_Documento() {
      this.ComprimentoTextoMax = null;
      this.ExpressaoRegular = null;
      this.PlaceHolder = null;
      this.PlaceHolderConfirmarSenha = null;

      //Corrige extensões
      this.FormatosArquivo = this.FormatosArquivo.Replace(".", "").Replace(" ", "").Trim().ToLower();
      List<string> formatos = new List<string>();
      foreach(string f in this.FormatosArquivo.Split(',')) {
        formatos.Add($".{f}");
      }
      this.FormatosArquivo = String.Join(", ", formatos);

    }

    //#### Edição ####

    //###################################################################################################
    private void VerificarEdicao_Documento() {
      if (String.IsNullOrWhiteSpace(this.FormatosArquivo)) throw new Exception("Nenhum formato de arquivo foi informado");
    }

    //###################################################################################################
    private void CorrigirEdicao_Documento() {
      this.ComprimentoTextoMax = null;
      this.ExpressaoRegular = null;
      this.PlaceHolder = null;
      this.PlaceHolderConfirmarSenha = null;

      //Corrige extensões
      this.FormatosArquivo = this.FormatosArquivo.Replace(".", "").Replace(" ", "").Trim().ToLower();
      List<string> formatos = new List<string>();
      foreach (string f in this.FormatosArquivo.Split(',')) {
        formatos.Add($".{f}");
      }
      this.FormatosArquivo = String.Join(", ", formatos);
    }

    //###################################################################################################
    private void CopyToUpdate_Documento(Campo_Cadastro campo) {
      if (!this.Generico) {
        campo.Required = this.Required;
        campo.StartCriacaoCampo = this.StartCriacaoCampo;
        campo.FormatosArquivo = this.FormatosArquivo;
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
    private void Update_Documento() {
        
    }

  }
}
