using Cadastro.Data;
using Cadastro.Models.Entities.DB.Account;
using Cadastro.Models.Entities.DB.Account.Form;
using Cadastro.Models.Services.Application.Security.Autorizacao;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cadastro.Models.ViewModel.Application.Settings.Configurations
{
    public class ConfiguracaoVM
    {

        public readonly AppDbContext _context;

        public ConfiguracaoVM(AppDbContext context) { _context = context; }

        public string msgResult { get; set; }
        public List<Hierarquia> listaHierarquias { get; set; }
        public List<Grupo_Cadastro> listaGrupos_Cadastro { get; set; }


        //############################################################################################
        //Gera lista de hierarquias
        public async Task<List<Hierarquia>> listaHierarquia(Usuario operador)
        {
            List<Hierarquia> listaHierarquia = new List<Hierarquia>();
            //Gera lista na ordem da hierarquia
            List<Hierarquia> listaDb = await _context.Hierarquias?.
              Include(h => h.PermissaoHierarquia).
              Include(h => h.HierarquiasUsuario).ThenInclude(hu => hu.Usuario).OrderBy(l => l.Ordem).
              Where(h => !h.PermissaoHierarquia.SomenteAdminPrincipal || operador.IsAdmin()).
              ToListAsync();

            return listaDb;
        }
    }
}