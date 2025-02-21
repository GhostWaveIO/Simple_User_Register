using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Cadastro.Models.Account.Cadastro.Dados {
  public partial class Dado_Cadastro {


    [RegularExpression("^[0-9 ]+$", ErrorMessage = "Somente números")]
    [Range(int.MinValue, int.MaxValue, ErrorMessage = "Valor entre {1} e {2}")]
    public int? Numero { get; set; }

    [NotMapped]
    [Required(ErrorMessage = "Campo obrigatório")]
    [RegularExpression("^[0-9]+$", ErrorMessage = "Somente números")]
    [Range(int.MinValue, int.MaxValue, ErrorMessage = "Valor entre {1} e {2}")]
    public int? Numero_Required {
      get { return this.Numero; }
      set { this.Numero = value; }
    }


    private void Preparar_Numero() {
      if(this.Campo.NumeroPreenchido != null)
        this.Numero = Convert.ToInt32(this.Campo.NumeroPreenchido);
    }

    //#######################################################################################################################
    private async Task VerificarCriacao_Numero() {
      if (this.Campo == null) throw new Exception("Campo não informado");

      if (this.Campo.Required && (this.Numero == null || (this.Numero == 0 && this.Campo.Min > 0))) throw new Exception($"O campo \"{this.Campo.Label}\" é obrigatório.");
      if (this.Numero > this.Campo.Max) throw new Exception($"Valor acima do permitido no campo \"{this.Campo.Label}\".");
      if (this.Numero < this.Campo.Min) throw new Exception($"Valor abaixo do permitido no campo \"{this.Campo.Label}\".");
      if (this.Campo.Unico && this.Numero != null) {
        if (await _context.DadosCadastro.AnyAsync(d => d.Numero == this.Numero && d.Campo_CadastroId == this.Campo_CadastroId)) throw new Exception($"Já existe uma conta com este mesmo valor no campo \"{this.Campo.Label}\"");
      }
    }

    //#######################################################################################################################
    private void CorrigirCriacao_Numero() {
      
      
      this.Email = null;
      //this.Numero = null;
      this.NumeroMonetario = null;
      this.Senha = "";
      this.Texto250 = null;
      this.TextoLongo = null;
    }

    //#######################################################################################################################
    private async Task Criar_Numero() {
      await _context.AddAsync(this);
      await _context.SaveChangesAsync();
    }

    //#######################################################################################################################
    private async Task VerificarEdicao_Numero() {
      if (this.Campo == null) throw new Exception("Campo não informado");

      if (this.Campo.Required && (this.Numero == null || (this.Numero == 0 && this.Campo.Min > 0))) throw new Exception($"O campo \"{this.Campo.Label}\" é obrigatório.");
      if (this.Numero > this.Campo.Max) throw new Exception($"Valor acima do permitido no campo \"{this.Campo.Label}\".");
      if (this.Numero < this.Campo.Min) throw new Exception($"Valor abaixo do permitido no campo \"{this.Campo.Label}\".");
      if (this.Campo.Unico && this.Numero != null) {
        if (await _context.DadosCadastro.AnyAsync(d => d.Numero == this.Numero && d.Campo_CadastroId == this.Campo_CadastroId && d.Dado_CadastroId != this.Dado_CadastroId)) throw new Exception($"Já existe uma conta com este mesmo valor no campo \"{this.Campo.Label}\"");
      }
    }

    //#######################################################################################################################
    private void CorrigirEdicao_Numero() {

    }

    //#######################################################################################################################
    private void CopyToUpdate_Numero(Dado_Cadastro dado) {
      dado.Numero = this.Numero;
    }

    //#######################################################################################################################
    private async Task Update_Numero() {
      if(!await _context.DadosCadastro.ContainsAsync(this)) throw new Exception($"Este dado no campo \"{this.Campo.Label}\" não existe!");

      _context.DadosCadastro.Update(this);
      await _context.SaveChangesAsync();
    }

  }
}
