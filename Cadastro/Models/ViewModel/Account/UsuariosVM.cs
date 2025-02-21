using Cadastro.Models.ViewModel.Account.Search;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Cadastro.Models.ViewModel.Account
{
    public class UsuariosVM
    {

        //Gera o grid de seleção de página
        public string GeraGridPaginaPesquisa(PesquisarUsuarios resultado, IUrlHelper urlPagDestino)
        {
            //int? pagina, int qtdResultado, string pesquisa, string urlPagDestino
            string GGSpace = " &nbsp;&nbsp;&nbsp;&nbsp;";//Espaço GG
            int pagVisiveis = 6;//Define a quantidade de páginas visíveis
            int pagAntesDepis = pagVisiveis / 2;
            int qtdPagina = (int)Math.Ceiling(resultado.QtdResultado / (double)Program.Config.qtdPPagUser);//Converte quantidade de itens por quantidade de páginas
            PesquisarUsuarios pesquisaUrl = new PesquisarUsuarios();


            if (qtdPagina == 1) return "<br/>";//Caso seja uma só página, retorna vazio

            string res = "<p class=\"text-center mt-5\">";
            //Link Início
            if (qtdPagina > pagVisiveis)
            {
                resultado.CopyTo(pesquisaUrl);
                pesquisaUrl.Pagina = 0;
                res += string.Format("<a class='text-primary' href=\"{0}\">Início</a>{2}",
                urlPagDestino.Action("Index", "Usuarios", pesquisaUrl),
                resultado.Pesquisa,
                GGSpace);
            }

            for (int c = 0; c < qtdPagina; c++)
            {
                resultado.CopyTo(pesquisaUrl);
                pesquisaUrl.Pagina = c;

                if (c >= resultado.Pagina - pagAntesDepis && c <= resultado.Pagina + pagAntesDepis)
                {//Exibe somente algumas páginas abaixo e acima da página atual
                    res += string.Format("<a class='{3}' href=\"{0}\">{2}</a>",
                      urlPagDestino.Action("Index", "Usuarios", pesquisaUrl),
                      resultado.Pagina,
                      c + 1,
                      resultado.Pagina == c ? "text-danger" : "text-primary"
                    );
                    if (c < qtdPagina)
                        res += GGSpace;
                }
            }

            //Link Último
            if (qtdPagina > pagVisiveis)
            {
                res += string.Format("<a  class='text-primary' href=\"{0}\">Último</a>" +
                "</p>",
                urlPagDestino.Action("Index", "Usuarios", pesquisaUrl),
                resultado.Pesquisa);
            }

            return res;
        }
    }
}
