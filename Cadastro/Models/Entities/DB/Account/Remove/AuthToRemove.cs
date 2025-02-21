using Cadastro.Models.Entities.DB.Account;
using Cadastro.Models.Services.Application.Text.Generators.Keys;
using System;
using System.ComponentModel.DataAnnotations;

namespace Cadastro.Models.Entities.DB.Account.Remove
{
    public class AuthToRemove
    {

        public AuthToRemove(long usuarioId)
        {
            UsuarioId = usuarioId;
            CreatedAt = DateTime.Now;
        }

        public AuthToRemove(long usuarioId, string code)
        {
            UsuarioId = usuarioId;
            CreatedAt = DateTime.Now;
        }

        public int AuthToRemoveId { get; private set; }

        [StringLength(120)]
        public string Code { get; private set; }

        public bool Approved { get; set; }

        public DateTime CreatedAt { get; private set; }


        public Usuario Usuario { get; set; }
        public long UsuarioId { get; set; }


        public void GenerateCode()
        {
            if (!string.IsNullOrEmpty(Code)) throw new Exception("Já existe um código gerado!");
            Code = GeradorKey.Gerar(120, true, true, true);
        }
    }
}
