using Cadastro.Data;
using Cadastro.Models.Entities.DB.Account.Form;

namespace Cadastro.Models.ViewModel.Account.Cadastro.Grupo
{
    public class EditarGrupoVM
    {

        public AppDbContext _context { get; set; }

        public EditarGrupoVM() { }
        public EditarGrupoVM(AppDbContext context)
        {
            _context = context;
        }

        public Grupo_Cadastro grupo { get; set; }


    }
}
