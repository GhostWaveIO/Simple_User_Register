using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cadastro.Models.Services.Application.Security.Autorizacao;

namespace Cadastro.Models.ViewModel.Application.Security.Permissions
{
    public class HierarquiaServiceVM
    {

        public Hierarquia hierarquia { get; set; }
        //public HierarquiaSuperior hierarquiaSuperior { get; set; }
        //public HierarquiaInferior hierarquiaInferior { get; set; }
        public PermissaoHierarquia permissao { get; set; }
        public List<Hierarquia> listaHierarquias { get; set; }

        public bool Apagar { get; set; }
        public int hierarquiaId { get; set; }

    }
}
