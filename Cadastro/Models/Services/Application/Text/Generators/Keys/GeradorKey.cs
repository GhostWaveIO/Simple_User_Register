using System;
using System.Collections.Generic;

namespace Cadastro.Models.Services.Application.Text.Generators.Keys
{
    public class GeradorKey
    {
        public static string Gerar(byte length = 30, bool numeros = true, bool minusculos = true, bool maiusculos = false)
        {
            string res = "";

            List<char> num = new List<char>() { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            List<char> letMin = new List<char>() { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'x', 'W', 'y', 'z' };
            List<char> letMai = new List<char> { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'X', 'W', 'Y', 'Z' };
            List<char> lista = new List<char>();

            //Usar números
            if (numeros)
                lista.AddRange(num);

            //Usar letras minúsculas
            if (minusculos)
                lista.AddRange(letMin);

            //Usar letras maiúsculas
            if (maiusculos)
                lista.AddRange(letMai);

            if (!numeros && !minusculos && !maiusculos)
            {
                lista.AddRange(num);
                lista.AddRange(letMin);
            }

            Random rand = new Random();


            for (byte c = 0; c < length; c++)
            {
                res += lista[rand.Next(lista.Count - 1)];
            }

            return res;
        }

    }
}
