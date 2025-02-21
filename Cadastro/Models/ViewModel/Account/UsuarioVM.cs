using Cadastro.Data;
using Cadastro.Models.Entities.DB.Account;
using Cadastro.Models.Services.Application.Security.Autorizacao;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cadastro.Models.ViewModel.Account
{
    public enum ListaErros { Válido, Usuário_Existe, Email_Existe, Senhas_Diferentes, Validacao_Ausente }

    public class UsuarioVM
    {

        public AppDbContext _context;

        public UsuarioVM() { }
        public UsuarioVM(AppDbContext context) { _context = context; }

        public string msgResult { get; set; }

        //Pesquisa
        #region Area de Pesquisa
        public string pesquisa { get; set; }
        public int pagPesquisa { get; set; }
        public int qtdResultado { get; set; }
        //Lista do resultado da Pesquisa
        public List<Usuario> ResultadoPesquisa;
        #endregion

        //Listas
        public List<Hierarquia> listaHierarquias { get; set; }

        //Models
        public Usuario usuario { get; set; }
        public UsuariosVM UserVM { get; set; } = new UsuariosVM();
        public Hierarquia hierarquia { get; set; }


        //######################################################################################
        //Pega a quantadade de itens do resultado da pesquisa
        public void GetQtdResultado(List<Usuario> resultado)
        {
            qtdResultado = resultado.Count();
        }


        public async Task<bool> ConsultarHierarquiaUsuario(Usuario usuario, Hierarquia hierarquia)
        {
            bool res = true;

            Usuario u = await _context.Usuarios.Include(u => u.HierarquiasUsuario).ThenInclude(hu => hu.Hierarquia).FirstOrDefaultAsync(u => u.UsuarioId == usuario.UsuarioId);
            res = u.HierarquiasUsuario?.Any(h => h.Hierarquia.HierarquiaId == hierarquia.HierarquiaId) ?? false;

            return res;
        }
    }
}
