using Cadastro.Models.Account.Cadastro.Dados;
using System.ComponentModel.DataAnnotations;

namespace Cadastro.Models.Entities.DB.Account.Form.Seletores
{
    public class CheckBox_Checked
    {
        [Key]
        public long CheckBox_CheckedId { get; set; }

        //Checked
        public bool Checked { get; set; }


        public long CheckBox_ItemId { get; set; }
        public CheckBox_Item CheckBox_Item { get; set; }

        public long Dado_CadastroId { get; set; }
        public Dado_Cadastro Dado_Cadastro { get; set; }


    }
}
