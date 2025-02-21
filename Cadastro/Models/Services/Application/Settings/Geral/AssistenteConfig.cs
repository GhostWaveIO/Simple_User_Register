using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cadastro.Models.Services.Application.Settings.Geral
{
    public static class AssistenteConfig
    {
        public static byte qtdBoxEvent { get { return (byte)(qtdStatusAtivo() + 1); } }

        public static short qtdStatusAtivo()
        {
            short res = 0;

            foreach (KeyValuePair<string, bool> s in Program.Config.statusUser)
            {
                if (s.Value) res++;
            }

            return res;
        }
    }
}
