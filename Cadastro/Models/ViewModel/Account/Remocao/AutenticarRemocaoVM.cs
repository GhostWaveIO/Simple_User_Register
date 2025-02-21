using Cadastro.Models.Account.Cadastro.Campos;
using Cadastro.Models.Services.Application.Text.Generators.Hash;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace Cadastro.Models.ViewModel.Account.Remocao
{
    public class AutenticarRemocaoVM
    {

        [Display(Name = "Senha")]
        [StringLength(250, MinimumLength = 8, ErrorMessage = "Deve conter entre {2} e {1} caracteres")]
        public string Senha { get; set; }

        public long UsuarioId { get; set; }


        public string res { get; set; }
        //public Campo_Cadastro Campo_Cadastro { get; set; }

        public string GetPassMD5()
        {
            GeradorMD5 gerador = new GeradorMD5();
            return gerador.GerarMD5(Senha);
        }

        public bool ComparePassword(string pass)
        {
            return GetPassMD5() == pass;
        }
    }
}
