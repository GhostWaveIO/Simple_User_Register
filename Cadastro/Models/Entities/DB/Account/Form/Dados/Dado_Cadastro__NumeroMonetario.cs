using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Cadastro.Models.Account.Cadastro.Dados {
  public partial class Dado_Cadastro {

    //[RegularExpression("^[0-9 ,]+$", ErrorMessage = "Somente números")]
    //[Range(1.55f, 200.55f, ErrorMessage = "Valor entre {1} e {2}")]
    [DataType(DataType.Currency)]
    public float? NumeroMonetario { get; set; }

    [NotMapped]
    [Required(ErrorMessage = "Campo obrigatório")]
    public float? NumeroMonetario_Required { 
      get { return this.NumeroMonetario; }
      set { this.NumeroMonetario = value; }
    }


    private void Preparar_NumeroMonetario() {
      if(this.NumeroMonetario != null) this.NumeroMonetario = this.NumeroMonetario;
    }

    //#######################################################################################################################
    private async Task VerificarCriacao_NumeroMonetario() {
      if(this.Campo == null) throw new Exception("Campo não informado!");

      if(this.Campo.Required && (this.NumeroMonetario == null || (this.NumeroMonetario == 0 && this.Campo.Min > 0))) throw new Exception($"O campo \"{this.Campo.Label}\" é obrigatório.");
      if(this.NumeroMonetario > this.Campo.Max) throw new Exception($"Valor acima do permitido no campo \"{this.Campo.Label}\".");
      if(this.NumeroMonetario < this.Campo.Min) throw new Exception($"Valor abaixo do permitido no campo \"{this.Campo.Label}\".");
      if (this.Campo.Unico && this.NumeroMonetario != null) {
        if (await _context.DadosCadastro.AnyAsync(d => d.NumeroMonetario == this.NumeroMonetario && d.Campo_CadastroId == this.Campo_CadastroId)) throw new Exception($"Já existe uma conta com este mesmo valor no campo \"{this.Campo.Label}\"");
      }
    }

    //#######################################################################################################################
    private void CorrigirCriacao_NumeroMonetario() {
      if(this.NumeroMonetario != null)
        this.NumeroMonetario = (float?)Convert.ToDecimal(this.NumeroMonetario?.ToString("0.00"));
      this.Email = null;
      this.Numero = null;
      this.Senha = "";
      this.Texto250 = null;
      this.TextoLongo = null;
    }

    //#######################################################################################################################
    private async Task Criar_NumeroMonetario() {
      await _context.AddAsync(this);
      await _context.SaveChangesAsync();
    }

    //#######################################################################################################################
    private async Task VerificarEdicao_NumeroMonetario() {
      if (this.Campo == null) throw new Exception("Campo não informado");

      if (this.Campo.Required && (this.NumeroMonetario == null || (this.NumeroMonetario == 0 && this.Campo.Min > 0))) throw new Exception($"O campo \"{this.Campo.Label}\" é obrigatório.");
      if (this.NumeroMonetario > this.Campo.Max) throw new Exception($"Valor acima do permitido no campo \"{this.Campo.Label}\".");
      if (this.NumeroMonetario < this.Campo.Min) throw new Exception($"Valor abaixo do permitido no campo \"{this.Campo.Label}\".");
      if (this.Campo.Unico && this.NumeroMonetario != null) {
        if (await _context.DadosCadastro.AnyAsync(d => d.NumeroMonetario == this.NumeroMonetario && d.Campo_CadastroId == this.Campo_CadastroId && d.Dado_CadastroId != this.Dado_CadastroId)) throw new Exception($"Já existe uma conta com este mesmo valor no campo \"{this.Campo.Label}\"");
      }
    }

    //#######################################################################################################################
    private void CorrigirEdicao_NumeroMonetario() {
      if (this.NumeroMonetario != null)
        this.NumeroMonetario = (float?)Convert.ToDecimal(this.NumeroMonetario?.ToString("0.00"));
    }

    //#######################################################################################################################
    private void CopyToUpdate_NumeroMonetario(Dado_Cadastro dado) {
      dado.NumeroMonetario = this.NumeroMonetario;
    }

    //#######################################################################################################################
    private async Task Update_NumeroMonetario() {
      if(!await _context.DadosCadastro.ContainsAsync(this)) throw new Exception($"Este dado do campo \"{this.Campo.Label}\" não existe no BD!");

      _context.DadosCadastro.Update(this);
      await _context.SaveChangesAsync();
    }

  }
}
