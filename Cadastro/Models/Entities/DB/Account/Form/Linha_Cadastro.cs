using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Cadastro.Data;
using Cadastro.Models.Account.Cadastro.Campos
  ;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Cadastro.Models.ViewModel.Account.Cadastro.Linha;

namespace Cadastro.Models.Entities.DB.Account.Form
{
    public class Linha_Cadastro
    {

        public AppDbContext _context { get; set; }

        [Display(Name = "Id da Linha")]
        public int Linha_CadastroId { get; set; }

        [Display(Name = "Ordem")]
        [Range(0, 2000000000, ErrorMessage = "Entre 0 e 2000000000")]
        [RegularExpression("^[0-9]+", ErrorMessage = "Somente números e maiores ou igual a 0")]
        public int Ordem { get; set; }

        /// <summary>
        /// Caso este campo for diferente de 0, significa que é um campo genérico, então não poderá ser apagado
        /// Cada campo genérico, terá uma linha genérica exclusiva
        /// UMA LINHA GENÉRICA É OBRIGATÓRIO SER DO GRUPO GENÉRICO
        /// </summary>
        public int? IdCampoGenerico { get; set; }

        public List<Campo_Cadastro> Campos { get; set; }

        public int Grupo_CadastroId { get; set; }
        public Grupo_Cadastro Grupo_Cadastro { get; set; }

        //#########################################################################################################################
        public bool IsGenerico()
        {
            return IdCampoGenerico != 0 && IdCampoGenerico != null;
        }

        //#########################################################################################################################
        public void VerificarCriacao()
        {

        }

        //#########################################################################################################################
        public void CorrigirCriacao()
        {

        }

        //#########################################################################################################################
        public void VerificarEdicao(EditarLinhaVM elSe)
        {

        }

        //#########################################################################################################################
        public void CorrigirEdicao(EditarLinhaVM elSe)
        {

        }

        //#########################################################################################################################
        public async Task Criar(CriarLinhaVM clSe)
        {
            if (_context == null) throw new Exception("Erro interno. Contexto não informado!");

            await _context.LinhasCadastro.AddAsync(this);
            await _context.SaveChangesAsync();
        }

        //#########################################################################################################################
        public async Task Editar(EditarLinhaVM elSe)
        {
            if (_context == null) throw new Exception("Erro interno. Contexto não informado!");

            _context.LinhasCadastro.Update(this);
            await _context.SaveChangesAsync();
        }

        //#########################################################################################################################
        public async Task ImportarCampo(int id)
        {
            Campo_Cadastro campo = await _context.CamposCadastro.FirstOrDefaultAsync(c => c.Campo_CadastroId == id);
            if (campo == null) throw new Exception("Campo não encontrado.");

            campo.Linha_CadastroId = Linha_CadastroId;
            _context.CamposCadastro.Update(campo);
            await _context.SaveChangesAsync();
        }

        //#########################################################################################################################
        public void CopyToUpdate(Linha_Cadastro linha)
        {
            linha.Ordem = Ordem;
        }

        //#########################################################################################################################
        public async Task Remover()
        {
            if (_context == null) throw new Exception("Erro interno. Contexto não informado.");
            if (IsGenerico()) throw new Exception("Linhas genéricas não podem ser removidas");
            if (Campos.Any()) throw new Exception("Não é possível remover esta linha. Existem campos vinculados a ela.");

            _context.LinhasCadastro.Remove(this);
            await _context.SaveChangesAsync();
        }

    }
}
