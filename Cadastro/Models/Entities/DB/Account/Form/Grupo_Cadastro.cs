using Cadastro.Data;
using Cadastro.Models.ViewModel.Account.Cadastro.Grupo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cadastro.Models.Entities.DB.Account.Form
{
    public class Grupo_Cadastro
    {

        public AppDbContext _context { get; set; }

        public Grupo_Cadastro() { }

        public Grupo_Cadastro(AppDbContext context)
        {
            _context = context;
        }

        public int Grupo_CadastroId { get; set; }

        [Display(Name = "Título")]
        [StringLength(120, MinimumLength = 0, ErrorMessage = "Entre 0 e 120 caracteres")]
        public string Titulo { get; set; }

        /// <summary>
        /// Se esta propriedade for true, significa que não poderá ser apagada, é nativa do sistema
        /// Poderá existir somente um grupo genérico
        /// </summary>
        [Display(Name = "Genérico")]
        public bool Generico { get; set; }

        [Display(Name = "Ordem")]
        [Range(0, 2000000000, ErrorMessage = "Entre 0 e 2000000000")]
        [RegularExpression("^[0-9]+", ErrorMessage = "Somente números e maiores ou igual a 0")]
        public int Ordem { get; set; }



        public List<Linha_Cadastro> Linhas { get; set; }


        //########################################################################################################################
        public void CorrigirCriacao()
        {
            Generico = false;
        }

        //########################################################################################################################
        public void VerificarCriacao()
        {

        }

        //########################################################################################################################
        public void CorrigirEdicao(EditarGrupoVM egSe)
        {

        }

        //########################################################################################################################
        public void VerificarEdicao(EditarGrupoVM egSe)
        {
        }

        //########################################################################################################################
        public async Task Criar()
        {
            if (_context == null) throw new Exception("Erro interno. Contexto não informado");

            await _context.GruposCadastro.AddAsync(this);
            await _context.SaveChangesAsync();
        }

        //########################################################################################################################
        public void CopyToUpdate(Grupo_Cadastro grupo)
        {
            grupo.Titulo = Titulo;
            grupo.Ordem = Ordem;
        }

        //########################################################################################################################
        public async Task Editar(EditarGrupoVM egSe)
        {
            if (_context == null) throw new Exception("Erro interno. Contexto não informado");

            _context.GruposCadastro.Update(this);
            await _context.SaveChangesAsync();
        }

        //########################################################################################################################
        public async Task Remover()
        {
            if (_context == null) throw new Exception("Erro interno. Contexto não informado.");
            if (Generico) throw new Exception("Grupos genéricos não podem ser removidos!");
            if (Linhas.Any(l => l.Campos.Any())) throw new Exception("Existem campos vinculados a este grupo.");

            _context.GruposCadastro.Remove(this);
            await _context.SaveChangesAsync();
        }

    }
}
