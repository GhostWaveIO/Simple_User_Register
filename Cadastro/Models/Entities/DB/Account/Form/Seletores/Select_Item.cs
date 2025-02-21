using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Cadastro.Models.Account.Cadastro.Campos;
using Cadastro.Data;
using System.Linq;

namespace Cadastro.Models.Entities.DB.Account.Form.Seletores
{
    public class Select_Item
    {

        public AppDbContext _context { get; set; }

        public int Select_ItemId { get; set; }

        //Nome
        [StringLength(200, ErrorMessage = "Máximo {1} caracteres")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public string Nome { get; set; }

        //Nome Resumido
        public string NomeResumido { get; set; }

        public bool Generico { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [Range(0, 2000000000, ErrorMessage = "Entre {1} e {2}")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Somente números")]
        public int Ordem { get; set; }

        //Pré-Selecionado
        /// <summary> Define o item pré-selecionado </summary>
        [Display(Name = "Item Pré-Selecinado")]
        public bool PreSelecionado { get; set; }

        #region Entidades

        //Coleção de itens selecionados dos cadastrados
        public List<Select_Selected> Selecteds { get; set; }

        public int Campo_CadastroId { get; set; }
        public Campo_Cadastro Campo_Cadastro { get; set; }

        #endregion FIM| Entidades

        //#########################################################################################################
        public async Task PreSelecionar(bool? opcao = null)
        {
            if (_context == null) throw new Exception("Erro interno. Contexto não informado.");

            bool old = PreSelecionado;

            Campo_Cadastro.Selects.ForEach(s => s.PreSelecionado = false);
            _context.SelectItens.UpdateRange(Campo_Cadastro.Selects);
            await _context.SaveChangesAsync();

            if (opcao == null)
            {
                PreSelecionado = !old;
            }
            else
            {
                PreSelecionado = opcao ?? false;
            }

            _context.SelectItens.Update(this);
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

            _context.SelectItens.Update(this);
            await _context.SaveChangesAsync();

        }

        //#########################################################################################################
        public void CopyToUpdate(Select_Item select)
        {
            select.Nome = Nome;
            select.Ordem = Ordem;
        }

        //#########################################################################################################
        /// <summary> Precisa ser referenciado "this.CheckBoxes.Selects" </summary>
        public async Task Remover()
        {
            if (Campo_Cadastro?.Selects == null) throw new Exception("Ocorreu um erro interno de referência.", new Exception("\"this.Campo_Cadastro.RadioButtons\" não foi referenciado"));

            if (!Campo_Cadastro.Selects.Any(s => s.Select_ItemId != Select_ItemId))
                throw new Exception($"Não foi possível remover este item. É necessário ao menos um item para este campo \"{Campo_Cadastro.Label}\"");

            _context.SelectItens.Remove(this);
            await _context.SaveChangesAsync();
        }

    }
}
