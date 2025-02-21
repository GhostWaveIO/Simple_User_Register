using System;
using Cadastro.Models.Account.Cadastro.Dados;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Cadastro.Data;

namespace Cadastro.Models.Entities.DB.Account.Form.Seletores
{
    public class Select_Selected
    {

        public AppDbContext _context { get; set; }

        public long Select_SelectedId { get; set; }

        public bool Selected { get; set; }


        public int? Select_ItemId { get; set; }

        public Select_Item Select_Item { get; set; }

        [NotMapped]
        public int? Select_ItemId_Nullable
        {
            get
            {
                if (Select_Item != null) return Select_Item.Select_ItemId;
                return Select_ItemId;
            }
            set { Select_ItemId = value; }
        }

        [NotMapped]
        [Required(ErrorMessage = "Campo obrigatório")]
        public int? Select_ItemId_Required
        {
            get
            {
                if (Select_Item != null) return Select_Item.Select_ItemId;
                return Select_ItemId;
            }
            set { Select_ItemId = value; }
        }

        public long Dado_CadastroId { get; set; }
        public Dado_Cadastro Dado_Cadastro { get; set; }



        public string GetNomeResumido()
        {
            if (Select_Item == null) throw new Exception("Seletores não encontrado", new Exception("A colheita de seletores dos estados não foi encontrada"));

            return Select_Item.NomeResumido;
        }
    }
}
