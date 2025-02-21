using Cadastro.Data;
using Cadastro.Models.Account.Cadastro.Campos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Cadastro.Models.Entities.DB.Account.Form.Seletores
{
    public class CheckBox_Item
    {

        public AppDbContext _context { get; set; }

        [Key]
        public long CheckBox_ItemId { get; set; }

        [StringLength(300, ErrorMessage = "Máximo {1} caracteres")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [Range(0, 2000000000, ErrorMessage = "Entre {1} e {2}")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Somente números")]
        public int Ordem { get; set; }

        //Define o item que estará pré-checked
        [Display(Name = "Item Pré-Selecinado")]
        public bool PreChecked { get; set; }

        public List<CheckBox_Checked> CheckBoxes { get; set; }

        public int Campo_CadastroId { get; set; }
        public Campo_Cadastro Campo_Cadastro { get; set; }

        //#########################################################################################################
        public async Task PreSelecionar()
        {
            if (_context == null) throw new Exception("Erro interno. Contexto não informado.");

            PreChecked = !PreChecked;

            _context.CheckBoxItens.Update(this);
            await _context.SaveChangesAsync();
        }

        //#########################################################################################################
        public void VerificarEdicao()
        {

        }

        //#########################################################################################################
        public void CorrigirEdicao()
        {

        }

        //#########################################################################################################
        public async Task Update()
        {
            if (Campo_Cadastro.Generico) throw new Exception("Opção de campo genérico não pode ser editado.");

            _context.CheckBoxItens.Update(this);
            await _context.SaveChangesAsync();

        }

        //#########################################################################################################
        public void CopyToUpdate(CheckBox_Item select)
        {
            select.Nome = Nome;
            select.Ordem = Ordem;
        }

        //#########################################################################################################
        /// <summary> Precisa ser referenciado "this.CheckBoxes.RadioButtons" </summary>
        public async Task Remover()
        {
            if (Campo_Cadastro?.CheckBoxes == null) throw new Exception("Ocorreu um erro interno de referência.", new Exception("\"this.Campo_Cadastro.RadioButtons\" não foi referenciado"));

            if (!Campo_Cadastro.CheckBoxes.Any(c => c.CheckBox_ItemId != CheckBox_ItemId))
                throw new Exception($"Não foi possível remover este item. É necessário ao menos um item para este campo \"{Campo_Cadastro.Label}\"");

            _context.CheckBoxItens.Remove(this);
            await _context.SaveChangesAsync();
        }

    }
}
