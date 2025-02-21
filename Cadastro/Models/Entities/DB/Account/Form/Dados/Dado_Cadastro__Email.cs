using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cadastro.Models.Account.Cadastro.Campos;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using Cadastro.Models.Services.Application.Network.Email;

namespace Cadastro.Models.Account.Cadastro.Dados
{
    public partial class Dado_Cadastro {

    [NotMapped]
    public long Dado_CadastroId_Email {get; set; }

    //Email
    [StringLength(250, ErrorMessage = "Máximo {1} caracteres")]
    [EmailAddress(ErrorMessage = "Digite um Email válido")]
    public string Email { get; set; }

    //Email (Obrigatório)
    [NotMapped]
    [Required(ErrorMessage = "Campo obrigatório")]
    [StringLength(250, ErrorMessage = "Máximo {1} caracteres")]
    [EmailAddress(ErrorMessage = "Digite um Email válido")]
    public string Email_Required {
      get { return this.Email; }
      set { this.Email = value; }
    }

    //Email para alteração
    [StringLength(250, ErrorMessage = "Máximo {1} caracteres")]
    [EmailAddress(ErrorMessage = "Digite um Email válido")]
    [Remote(action: "VerificarAlterarEmail", controller: "Usuarios", AdditionalFields = "Email_Comparacao,Dado_CadastroId_Email")]
    //[Remote(action: "VerificarCpf", controller: "Cadastro", AdditionalFields = nameof(Dado_CadastroId_Cpf))]
    public string Email_Novo { get; set; }

    //Email para comparação na alteração
    [NotMapped]
    public string Email_Comparacao { get; set; }

    [NotMapped]
    public bool ConfirmacaoDeEmailEnviado { get; set; }

    [NotMapped]
    public IUrlHelper Url { get; set; }



    //#######################################################################################################################
    private void Preparar_Email() {
      if(String.IsNullOrEmpty(this.Campo.TextoPreenchido))
        this.Email = this.Campo.TextoPreenchido??String.Empty;
    }

    //#######################################################################################################################
    private async Task VerificarCriacao_Email() {
      if (this.Campo == null) throw new Exception("Campo não informado.");
      string supostoEmail = null;

      //Coleta Email usado no processo "Email" ou "Email_Novo"
      if (!String.IsNullOrEmpty(this.Email_Generico) && this.Campo.CampoGenerico == ECampoGenerico.Email)
        supostoEmail = this.Email_Generico;
      else if (!String.IsNullOrEmpty(this.Email))
        supostoEmail = this.Email;

      //Verifica se é obrigatório e se foi informado
      if (this.Campo.Required && String.IsNullOrWhiteSpace(supostoEmail))
        throw new Exception($"O campo \"{this.Campo.Label}\" é obrigatório.");
      else if(supostoEmail == null && !this.Campo.Required)
        return;

      //Verifica se comprimento é permitido
      if ((supostoEmail?.Length ?? 0) > this.Campo.ComprimentoTextoMax) throw new Exception($"Tamanho do Email acima do permitido no campo \"{this.Campo.Label}\".");
      if ((supostoEmail?.Length ?? 0) > 250) throw new Exception($"Tamanho do Email acima do limite máximo no campo \"{this.Campo.Label}\".");

      //Verifica se modelo de email é válido
      if (!Regex.IsMatch(supostoEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*") && !String.IsNullOrEmpty(supostoEmail)) throw new Exception($"Modelo de email inválido no campo \"{this.Campo.Label}\"");

      //Verifica se é "Único" e se existe outro dado com este mesmo email
      if (this.Campo.Unico && !String.IsNullOrEmpty(supostoEmail) && await _context.DadosCadastro.AnyAsync(d => d.Campo_CadastroId == this.Campo_CadastroId && d.Email == supostoEmail))
        throw new Exception($"Já existe outra conta usando este mesmo Email no campo \"{this.Campo.Label}\"!");

    }

    //#######################################################################################################################
    private void CorrigirCriacao_Email() {
      if (this.Campo.CampoGenerico == ECampoGenerico.Email && !String.IsNullOrEmpty(this.Email_Generico)) this.Email = this.Email_Generico;

      //this.Email = null;
      this.Numero = null;
      this.NumeroMonetario = null;
      this.Senha = "";
      this.Texto250 = null;
      this.TextoLongo = null;
    }

    //#######################################################################################################################
    private async Task Criar_Email() {
      await _context.AddAsync(this);
      await _context.SaveChangesAsync();
    }

    //#######################################################################################################################
    private async Task VerificarEdicao_Email(ELocalDeOperacao local) {
      if (this.Campo == null) throw new Exception("Campo não informado.");
      string supostoEmail = null;

      //Coleta Email usado no processo "Email" ou "Email_Novo"
      if(local == ELocalDeOperacao.Perfil && this.Campo.CampoGenerico == ECampoGenerico.Email)
        supostoEmail = this.Email_Generico;
      else if(!String.IsNullOrEmpty(this.Email))
        supostoEmail = this.Email;

      //Retorna caso seja nulo e não seja obrigatório
      if (String.IsNullOrEmpty(supostoEmail) && !this.Campo.Required && !this.Campo.Generico) return;

      //Verifica se Email foi informado
      if ((this.Campo.Required || this.Campo.Generico) && String.IsNullOrWhiteSpace(supostoEmail)) throw new Exception($"O campo \"{this.Campo.Label}\" é obrigatório.");


      //Verifica se modelo de email é válido
      if (!Regex.IsMatch(supostoEmail, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*") && !String.IsNullOrEmpty(supostoEmail)) throw new Exception($"Modelo de email inválido no campo \"{this.Campo.Label}\"");

      //Verifica se tamanho não extrapole o delimitado por padrão
      if ((supostoEmail?.Length ?? 0) > this.Campo.ComprimentoTextoMax) throw new Exception($"Tamanho do Email acima do permitido no campo \"{this.Campo.Label}\".");
      if ((supostoEmail?.Length ?? 0) > 250) throw new Exception($"Tamanho do Email acima do limite máximo no campo \"{this.Campo.Label}\".");

      //Verifica se Email é do Admin
      if(supostoEmail?.Trim() == Program.Config.emailPrincipal && this.Campo.CampoGenerico == ECampoGenerico.Email) throw new Exception("Este email não está disponível!");

      //Verifica se é "Único" e se existe outro dado com este mesmo email
      if (this.Campo.Unico && !String.IsNullOrEmpty(supostoEmail) && await _context.DadosCadastro.AnyAsync(d => d.Campo_CadastroId == this.Campo_CadastroId && d.Dado_CadastroId != this.Dado_CadastroId && d.Email == supostoEmail))
        throw new Exception($"Já existe outra conta usando este mesmo Email no campo \"{this.Campo.Label}\"!");
    }

    //#######################################################################################################################
    private void CorrigirEdicao_Email() {
      if(this.Campo.CampoGenerico == ECampoGenerico.Email && !String.IsNullOrEmpty(this.Email_Generico)) this.Email = this.Email_Generico;
    }

    //#######################################################################################################################
    private void CopyToUpdate_Email(Dado_Cadastro dado, ELocalDeOperacao local) {
      if(this.Campo.CampoGenerico == ECampoGenerico.Email && local == ELocalDeOperacao.Perfil && !String.IsNullOrEmpty(this.Email_Generico) && this.Email_Generico.Trim() != dado.Email.Trim()) {
        dado.Email_Novo = this.Email_Generico?.Trim();
      } else {
        dado.Email = this.Email?.Trim();
      }
    }

    //#######################################################################################################################
    private async Task Update_Email() {
      if(!await _context.DadosCadastro.ContainsAsync(this)) throw new Exception("Este dado não existe no BD!");
      //string supostoEmail = (String.IsNullOrEmpty(this.Email_Novo)?null: this.Email_Novo)??(String.IsNullOrEmpty(this.Email)?null: this.Email);

      EVerificaConfirmacao? resValidacao = null;
      EmailService emSe = new EmailService(_context);

      if (this.UsuarioId == 0) throw new Exception ("Usuário ID não pode ser nulo!");

      //Verifica situação das validações do usuário
      if (this.Email != this.Email_Novo && !String.IsNullOrEmpty(this.Email_Novo))
        resValidacao = (EVerificaConfirmacao?)await emSe.VerificarConfirmacaoEmail(this.UsuarioId);

      //Se ocorrer erro na verificação
      if (resValidacao == EVerificaConfirmacao.Erro)
        throw new Exception("Ocorreu um erro ao tentar validar seu Email");

      //Enviar Email para validação
      if (resValidacao != null) {
        await emSe.EnviarConfirmacaoCadastro(this.Usuario, Url);
        this.ConfirmacaoDeEmailEnviado = true;
        this.Email_Novo = null;
      }


      _context.DadosCadastro.Update(this);
      await _context.SaveChangesAsync();
    }

  }
}
