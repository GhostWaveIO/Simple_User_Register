using Cadastro.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;


namespace Cadastro.Models.Services.Application.Text.Generators.Identificadores
{
    public static class GeradorCodigoUsuario
    {

        public static async Task<string> Gerar(AppDbContext context, string siglaEstado)
        {
            string res = "";
            char[] divEstado = new char[] { siglaEstado[0], siglaEstado[1] };
            string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVXWYZ";
            Random rand = new Random();
            string codigoUsuario = "";

            if (divEstado.Length != 2) throw new Exception("Erro ao tentar gerar o Código de Usuário! Sigla de estado incorreto!");

            do
            {
                codigoUsuario = string.Format(
                  "{0}{1}{2}{3}{4}",
                  chars[rand.Next(chars.Length - 1)],
                  chars[rand.Next(chars.Length - 1)],
                  chars[rand.Next(chars.Length - 1)],
                  chars[rand.Next(chars.Length - 1)],
                  chars[rand.Next(chars.Length - 1)],
                  chars[rand.Next(chars.Length - 1)],
                  chars[rand.Next(chars.Length - 1)]
                );

                res = $"{codigoUsuario}";
            } while (await context.Usuarios?.AnyAsync(u => u.CodigoUsuario == res));

            return res;
        }
    }
}
