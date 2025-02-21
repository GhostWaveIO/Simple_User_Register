using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

//Todos Campos Genéricos aqui
namespace Cadastro.Models.Account.Cadastro.Dados {
  public partial class Dado_Cadastro {

    //Email Genérico (Modelo Email)
    [NotMapped]
    [Remote(action: "VerificarEmail", controller: "Cadastro", AdditionalFields = nameof(Dado_CadastroId_Email))]
    [Required(ErrorMessage = "Campo obrigatório")]
    [StringLength(250, ErrorMessage = "Máximo {1} caracteres")]
    [EmailAddress(ErrorMessage = "Digite um email válido")]
    public string Email_Generico { get; set; }

    //Email para Recuperação
    [NotMapped]
    [Display(Name = "Seu Email de Acesso")]
    [Required(ErrorMessage = "Este Campo é Obrigatório")]
    [StringLength(250, ErrorMessage = "Ex: usuario@domínio.com.br")]
    [EmailAddress(ErrorMessage = "Digite um email válido")]
    public string Email_Recuperacao {
      get { return this.Email; }
      set { this.Email = value; }
    }

    //Senha Genérico (Modelo Senha)
    [NotMapped]
    [Required(ErrorMessage = "Informe uma senha")]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "A senha deve conter entre {2} e {1} caracteres")]
    public string Senha_Generico { get; set; }

    //Senha Genérico (Modelo Senha)
    [NotMapped]
    //[Required(AllowEmptyStrings = true, ErrorMessage = "Informe uma senha")]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "A senha deve conter entre {2} e {1} caracteres")]
    public string Senha_Perfil {
      get { return Senha_Generico;}
      set { this.Senha_Generico = value;}
    }

    //Confirmar Senha
    [NotMapped]
    [Display(Name = "Confirme sua Senha")]
    [Compare("Senha_Generico", ErrorMessage = "As senhas não coincidem!")]
    [Required(ErrorMessage = "Confirme sua Senha")]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "A senha deve conter entre {2} e {1} caracteres")]
    public string ConfirmarSenha { get; set; }

    //Confirmar Senha
    [NotMapped]
    [Display(Name = "Confirme sua Senha")]
    [Compare("Senha_Perfil", ErrorMessage = "As senhas não coincidem!")]
    //[Required(AllowEmptyStrings = true, ErrorMessage = "Confirme sua Senha")]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "A senha deve conter entre {2} e {1} caracteres")]
    public string ConfirmarSenha_Perfil { 
      get{ return this.ConfirmarSenha; }
      set { this.ConfirmarSenha = value; }
    }

    //Primeiro Nome Genérico (Modelo Texto250)
    [NotMapped]
    [Required(ErrorMessage = "Campo obrigatório")]
    [StringLength(80, ErrorMessage = "Máximo {1} caracteres")]
    public string Nome_Generico { get; set; }

    [NotMapped]
    public long Dado_CadastroId_Cpf { get; set; }

    //Regex para CPF e CNPJ
    //([0-9]{2}[\.]?[0-9]{3}[\.]?[0-9]{3}[\/]?[0-9]{4}[-]?[0-9]{2})|([0-9]{3}[\.]?[0-9]{3}[\.]?[0-9]{3}[-]?[0-9]{2})

    //Primeiro Nome Genérico (Modelo Texto250)
    [NotMapped]
    [Required(ErrorMessage = "Campo obrigatório")]
    [StringLength(15, MinimumLength = 11, ErrorMessage = "Entre {2} e {1} caracteres")]
    [Remote(action: "VerificarCpf", controller: "Cadastro", AdditionalFields = nameof(Dado_CadastroId_Cpf))]
    //[RegularExpression(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$", ErrorMessage = "Ex: 123.456.789-10")]
    //[RegularExpression("^(([0-9]{3}.[0-9]{3}.[0-9]{3}-[0-9]{2})|([0-9]{11}))$", ErrorMessage = "Somente números")]
    [RegularExpression("^(([0-9]{3}[.]{1}[0-9]{3}[.]{1}[0-9]{3}[-]{1}[0-9]{2})|([0-9]{11}))$", ErrorMessage = "Ex: 123.456.789-00")]
    public string Cpf_Generico { get; set; }

    [NotMapped]
    [Required(ErrorMessage = "Campo obrigatório")]
    public int Estado_Generico { get; set; }


  }
}
