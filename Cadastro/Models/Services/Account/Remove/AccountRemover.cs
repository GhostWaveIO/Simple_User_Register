using Cadastro.Data;
using Cadastro.Models.Entities.DB.Account;

namespace Cadastro.Models.Services.Account.Remove
{
    public class AccountRemover
    {
        public AccountRemover(Usuario usuario, AppDbContext context)
        {

        }

        private AppDbContext _context { get; set; }
        private Usuario _usuario { get; set; }
    }
}
