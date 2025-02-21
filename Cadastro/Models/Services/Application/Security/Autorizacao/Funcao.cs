using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cadastro.Models.Services.Application.Security.Autorizacao
{
    public class Funcao
    {
        public int FuncaoId { get; set; }

        public IEnumerable<FuncaoUsuario> FuncoesUsuario { get; set; }
    }
}
