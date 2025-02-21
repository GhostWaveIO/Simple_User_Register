using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cadastro.Models.Services.Application.Security.Cryptography.Rijndael;

namespace Cadastro.Models.Account.Cadastro.Dados
{
    public partial class Dado_Cadastro {

    //Texto em textarea
    [StringLength(3000, ErrorMessage = "Máximo 3000 caracteres")]
    public string TextoLongo { get; set; }

    [NotMapped]
    [Required(ErrorMessage = "Campo obrigatório")]
    [StringLength(3000, ErrorMessage = "Máximo 3000 caracteres")]
    public string TextoLongo_Required {
      get{ return this.TextoLongo; }
      set { this.TextoLongo = value; }
    }

    //#######################################################################################################################
    private void Preparar_TextoLongo() {
      this.TextoLongo = this.Campo.TextoPreenchido;
    }

    //#######################################################################################################################
    private async Task VerificarCriacao_TextoLongo() {
      if(this.Campo == null) throw new Exception("Campo não informado.");

      if((this.TextoLongo?.Length??0) > this.Campo.ComprimentoTextoMax) throw new Exception($"Tamanho do texto acima do permitido no campo \"{this.Campo.Label}\".");
      if((this.TextoLongo?.Length??0) > 3000) throw new Exception($"Tamanho do texto acima do limite máximo no campo \"{this.Campo.Label}\".");

      if(this.Campo.Required && String.IsNullOrWhiteSpace(this.TextoLongo)) throw new Exception($"O campo \"{this.Campo.Label}\" é obrigatório.");
      if (this.Campo.Unico && this.TextoLongo != null) {
        if(await _context.DadosCadastro.AnyAsync(d => d.TextoLongo == this.TextoLongo && d.Campo_CadastroId == this.Campo_CadastroId)) throw new Exception($"Já existe uma conta com esta mesma informação no campo \"{this.Campo.Label}\"");
      }

    }

    //#######################################################################################################################
    private void CorrigirCriacao_TextoLongo() {
      
      this.Email = null;
      this.Numero = null;
      this.NumeroMonetario = null;
      this.Senha = "";
      this.Texto250 = null;
      //this.TextoLongo = null;
    }

    //#######################################################################################################################
    private async Task Criar_TextoLongo() {
      await _context.AddAsync(this);
      await _context.SaveChangesAsync();
    }

    //#######################################################################################################################
    private async Task VerificarEdicao_TextoLongo() {
      if (this.Campo == null) throw new Exception("Campo não informado.");

      if ((this.TextoLongo?.Length ?? 0) > this.Campo.ComprimentoTextoMax) throw new Exception($"Tamanho do texto acima do permitido no campo \"{this.Campo.Label}\".");
      if ((this.TextoLongo?.Length ?? 0) > 3000) throw new Exception($"Tamanho do texto acima do limite máximo no campo \"{this.Campo.Label}\".");

      if (this.Campo.Required && String.IsNullOrWhiteSpace(this.TextoLongo)) throw new Exception($"O campo \"{this.Campo.Label}\" é obrigatório.");

      if (this.Campo.Unico && this.TextoLongo != null) {
        if (await _context.DadosCadastro.AnyAsync(d => d.TextoLongo == this.TextoLongo && d.Campo_CadastroId == this.Campo_CadastroId && d.Dado_CadastroId != this.Dado_CadastroId)) throw new Exception($"Já existe uma conta com esta mesma informação no campo \"{this.Campo.Label}\"");
      }
    }

    //#######################################################################################################################
    private void CorrigirEdicao_TextoLongo() {

    }

    //#######################################################################################################################
    private void CopyToUpdate_TextoLongo(Dado_Cadastro dado) {
      dado.TextoLongo = this.TextoLongo;
    }

    //#######################################################################################################################
    private async Task Update_TextoLongo() {
      if(!await _context.DadosCadastro.ContainsAsync(this)) throw new Exception($"Este dado do campo \"{this.Campo.Label}\"");

      _context.DadosCadastro.Update(this);
      await _context.SaveChangesAsync();
    }

    //###################################################################################################
    public void Criptografar_TextoLongo() {
      CriptografiaRijndael crpWithKey = new CriptografiaRijndael();
      crpWithKey.ChaveObrigatoria = true;

      if (this.TextoLongo == null) return;

      try {
        this.DadosCriptografados = crpWithKey.Encrypt(this.TextoLongo);
        this.TextoLongo = null;
      } catch (Exception err) {
        throw new Exception("Erro interno. Não foi possível ler os dados!", err);
      }

    }

    //###################################################################################################
    public void Descriptografar_TextoLongo() {
      CriptografiaRijndael crpWithKey = new CriptografiaRijndael();
      crpWithKey.ChaveObrigatoria = true;
      if (String.IsNullOrEmpty(this.DadosCriptografados)) return;
      if (!crpWithKey.VerificarPrefixo(this.DadosCriptografados)) throw new Exception("Ocorreu um erro ao tentar ler os dados!", new Exception("Este texto não parece ser uma criptografia."));

      if (this.DadosCriptografados == null) return;

      try {
        this.TextoLongo = crpWithKey.Decrypt(this.DadosCriptografados);
        this.DadosCriptografados = null;
      } catch (Exception err) {
        throw new Exception("Erro interno. Não foi possível ler os dados!", err);
      }

    }

  }
}
