using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Cadastro.Models.Account.Cadastro.Campos;
using Cadastro.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Cadastro.Models.Entities.DB.Account.Form.Seletores
{
    public class RadioButton_Item
    {

        public AppDbContext _context { get; set; }

        public int RadioButton_ItemId { get; set; }


        [StringLength(300, ErrorMessage = "Máximo {1} caracteres")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public string Nome { get; set; }

        [Display(Name = "Ordem")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [Range(0, 2000000000, ErrorMessage = "Entre {1} e {2}")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Somente números")]
        public int Ordem { get; set; }

        //Define o item que estará pré-checked
        [Display(Name = "Item Pré-Selecinado")]
        public bool PreChecked { get; set; }


        public List<RadioButton_Checked> RadioButtons { get; set; }

        public int Campo_CadastroId { get; set; }
        public Campo_Cadastro Campo_Cadastro { get; set; }

        //#########################################################################################################
        public async Task PreSelecionar(bool? opcao = null)
        {
            if (_context == null) throw new Exception("Erro interno. Contexto não informado.");

            bool old = PreChecked;

            Campo_Cadastro.RadioButtons.ForEach(s => s.PreChecked = false);
            _context.RadioButtonItens.UpdateRange(Campo_Cadastro.RadioButtons);
            await _context.SaveChangesAsync();

            if (opcao == null)
            {
                PreChecked = !old;
            }
            else
            {
                PreChecked = opcao ?? false;
            }

            _context.RadioButtonItens.Update(this);
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

            _context.RadioButtonItens.Update(this);
            await _context.SaveChangesAsync();

        }

        //#########################################################################################################
        public void CopyToUpdate(RadioButton_Item select)
        {
            select.Nome = Nome;
            select.Ordem = Ordem;
        }

        //#########################################################################################################
        /// <summary> Precisa ser referenciado "this.Campo_Cadastro.RadioButtons" </summary>
        public async Task Remover()
        {
            if (Campo_Cadastro?.RadioButtons == null) throw new Exception("Ocorreu um erro interno de referência.", new Exception("\"this.Campo_Cadastro.RadioButtons\" não foi referenciado"));

            if (!Campo_Cadastro.RadioButtons.Any(r => r.RadioButton_ItemId != RadioButton_ItemId))
                throw new Exception($"Não foi possível remover este item. É necessário ao menos um item para este campo \"{Campo_Cadastro.Label}\"");

            _context.RadioButtonItens.Remove(this);
            await _context.SaveChangesAsync();
        }

    }
}
