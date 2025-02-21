using Cadastro.Models.Account.Cadastro.Dados;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cadastro.Models.Entities.DB.Account.Form.Seletores
{
    public class RadioButton_Checked
    {

        public long RadioButton_CheckedId { get; set; }

        public bool Checked { get; set; }


        public int? RadioButton_ItemId { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Campo obrigatório")]
        public int? RadioButton_ItemId_Required
        {
            get { return RadioButton_ItemId; }
            set { RadioButton_ItemId = value; }
        }



        public RadioButton_Item RadioButton_Item { get; set; }

        public long Dado_CadastroId { get; set; }
        public Dado_Cadastro Dado_Cadastro { get; set; }
    }
}
