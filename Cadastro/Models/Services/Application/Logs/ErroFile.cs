using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cadastro.Models.Services.Application.Logs
{
    public class ErroFile
    {

        public ErroFile() { }

        public ErroFile(string Titulo, string Conteudo)
        {
            this.Titulo = Titulo;
            this.Conteudo = Conteudo;
        }

        public string Titulo { get; set; }
        public string Conteudo { get; set; }
        public DateTime Criado { get; set; }
    }
}
