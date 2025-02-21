using System;
using System.ComponentModel.DataAnnotations;
using Cadastro.Models.Entities.DB.Account;

namespace Cadastro.Models.Entities.DB.Account.Email
{
    public class EmailValidado
    {

        public long EmailValidadoId { get; set; }

        public bool Confirmado { get; set; }

        //Key
        [Required]
        [StringLength(150)]
        public string key { get; set; }

        //Novo Email
        [StringLength(200)]
        public string EmailNovo { get; set; }

        //Data de Criação
        [Required]
        public DateTime Criado { get; set; }

        public long UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
    }
}
