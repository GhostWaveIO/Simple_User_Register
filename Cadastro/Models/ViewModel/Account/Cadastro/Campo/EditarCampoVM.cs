using Cadastro.Models.Account.Cadastro.Campos;
using System.ComponentModel.DataAnnotations;

namespace Cadastro.Models.ViewModel.Account.Cadastro.Campo
{
    public class EditarCampoVM
    {

        public Campo_Cadastro campo { get; set; }

        [Display(Name = "Retorna campo genérico para linha genérica e restaura colunas")]
        public bool DeportarCampo { get; set; }
    }
}
