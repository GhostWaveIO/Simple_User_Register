using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cadastro.Models.Services.Application.Text.Generators.Identificadores
{
    public static class GeradorCodigoRelatorio
    {

        public static string Gerar()
        {
            string res = null;
            char[] dic = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'X', 'W', 'Y', 'Z' };
            Random rand = new Random();

            //Gera o prefixo
            string prefixo = "";
            do
            {
                prefixo += dic[rand.Next(dic.Length - 1)];
            } while (prefixo.Length < 4);

            //Junta o prefixo com a data para formar o código
            res = string.Format(
            "{0}/{1}",
              prefixo,
              DateTime.Today.ToString("yy")
            );

            return res;
        }
    }
}
