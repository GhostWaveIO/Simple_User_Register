using Cadastro.Models.Entities.DB.Account.Form.Seletores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Cadastro.Models.Account.Cadastro.Dados
{
    public partial class Dado_Cadastro {

        [BindProperty]
        public List<CheckBox_Checked> CheckBoxes { get; set; }

        private void Preparar_CheckBox() {
            this.CheckBoxes = (from CheckBox_Item cbI in this.Campo.CheckBoxes
                               select new CheckBox_Checked() { CheckBox_ItemId = cbI.CheckBox_ItemId, Checked = cbI.PreChecked }).ToList();
        }

        //#######################################################################################################################
        private void VerificarCriacao_CheckBox() {
            if (this.Campo == null) throw new Exception("Campo não informado!");

            if (!this.Campo.CheckBoxes.Any()) return;

            if (this.Campo.Required && !this.CheckBoxes.Any(c => c.Checked)) throw new Exception($"Escolha ao menos uma opção no campo \"{this.Campo.Label}\".");

        }

        //#######################################################################################################################
        private void CorrigirCriacao_CheckBox() {

            this.Email = null;
            this.Numero = null;
            this.NumeroMonetario = null;
            this.Senha = "";
            this.Texto250 = null;
            this.TextoLongo = null;
        }

        //#######################################################################################################################
        private async Task Criar_CheckBox() {
            IEnumerable<CheckBox_Checked> checks = this.CheckBoxes;
            this.CheckBoxes = null;

            /*long idSomaDisponivel = 1;
            IQueryable<CheckBox_Checked> checksDB = await Task.FromResult(_context.CheckboxCheckedDados);
            foreach (CheckBox_Checked check in this.CheckBoxes) {
              check.CheckBox_Item = null;
              check.CheckBox_CheckedId = (await checksDB.AnyAsync()?await checksDB.MaxAsync(c => c.CheckBox_CheckedId):0)+(idSomaDisponivel++);
            }*/

            await _context.AddAsync(this);
            await _context.SaveChangesAsync();
            try {
                foreach (CheckBox_Checked check in checks) {
                    check.Dado_CadastroId = this.Dado_CadastroId;
                    check.CheckBox_Item = null;
                }

                await _context.CheckboxCheckedDados.AddRangeAsync(checks);
                await _context.SaveChangesAsync();
            } catch (Exception err) {
                //this.Remover();
                //////////////////////////////
                _context.DadosCadastro.Remove(this);
                await _context.SaveChangesAsync();
                //////////////////////////////
                throw err;
            }

        }

        //#######################################################################################################################
        private async Task VerificarEdicao_CheckBox() {
            this.Campo = await _context.CamposCadastro.Include(cmp => cmp.CheckBoxes).FirstAsync(c => c.Campo_CadastroId == this.Campo.Campo_CadastroId);
            //Verifica se existe algum item, caso não exista ele retornar
            if (!this.Campo.CheckBoxes.Any()) return;
            //Campo obrigatório
            if (this.Campo.Required && !this.CheckBoxes.Any(c => c.Checked)) throw new Exception($"Escolha ao menos uma opção no campo \"{this.Campo.Label}\".");

            //Verifica se todos Check_Checkeds foram devidamente enviados pelo formulário (para evitar fraudes ou bugs)
            IEnumerable<CheckBox_Item> checks = this.Campo.CheckBoxes;
            foreach (CheckBox_Item ck in checks) {
                if (!this.CheckBoxes.Any(c => c.CheckBox_ItemId == ck.CheckBox_ItemId)) throw new Exception($"Há alguma informação pendente no campo \"{this.Campo.Label}\"");
            }

            //Verificar se CheckBox_ItemId existe e/ou não há CheckBox_Checkeds duplicados
            foreach (CheckBox_Checked ck in this.CheckBoxes) {
                //Verifica se há itens duplicados
                try {
                    this.CheckBoxes.Single(c => c.CheckBox_ItemId == ck.CheckBox_ItemId);
                } catch (Exception err) { throw new Exception($"Aparenta ter itens duplicados no campo \"{this.Campo.Label}\"", err); }

                //Verifica se todos checks correspondem a um item existente
                if (!await _context.CheckBoxItens.AnyAsync(c => c.CheckBox_ItemId == ck.CheckBox_ItemId))
                    throw new Exception($"Um dos itens no campo \"{this.Campo.Label}\" não existe");
            }
        }

        //#######################################################################################################################
        private void CorrigirEdicao_CheckBox() {

        }

        //#######################################################################################################################
        private void CopyToUpdate_CheckBox(Dado_Cadastro dado) {
            dado.CheckBoxes = this.CheckBoxes;
        }

        //#######################################################################################################################
        private async Task Update_CheckBox() {

            IEnumerable<CheckBox_Checked> dadosNovos = this.CheckBoxes;
            //Coleta os antigos dados para remoção
            IEnumerable<CheckBox_Checked> dadosAntigos = await _context.CheckboxCheckedDados.Where(ckc => ckc.Dado_CadastroId == this.Dado_CadastroId).ToListAsync();
            if (!dadosAntigos.Any()) throw new Exception($"Ocorreu um erro ao tentar alterar os dados do campo \"{this.Campo.Label}\"");
            _context.CheckboxCheckedDados.RemoveRange(dadosAntigos);
            await _context.SaveChangesAsync();


            long idSomaDisponivel = 1;
            IQueryable<CheckBox_Checked> checksDB = await Task.FromResult(_context.CheckboxCheckedDados);
            foreach (CheckBox_Checked check in dadosNovos) {
                check.CheckBox_Item = null;
                check.CheckBox_CheckedId = (await checksDB.AnyAsync() ? await checksDB.MaxAsync(c => c.CheckBox_CheckedId) : 0) + (idSomaDisponivel++);
                check.Dado_CadastroId = this.Dado_CadastroId;
            }

            await _context.CheckboxCheckedDados.AddRangeAsync(this.CheckBoxes);
            await _context.SaveChangesAsync();
        }

        //#######################################################################################################################
        //Coleta CheckBox_Checkeds
        public List<CheckBox_Checked> GetCheckBoxCheckeds() {

            if (this._context == null) throw new Exception("Erro interno. Contexto não informado");
            if (this.CheckBoxes == null) throw new Exception($"Nenhuma opção {this.Campo.Label} encontrado!");
            if (!this.CheckBoxes.Any(ck => ck.CheckBox_Item != null)) throw new Exception($"Nenhum item {this.Campo.Label} encontrado!");

            List<CheckBox_Checked> checks = this.CheckBoxes.OrderBy(ck => ck.CheckBox_Item.Ordem).ToList();


            return checks;



            /*


            if (this._context == null) throw new Exception("Erro interno. Contexto não informado");
            if (this.CheckBoxes == null) throw new Exception($"Nenhuma opção {this.Campo.Label} encontrado!");
            if (this.Campo?.CheckBoxes == null) throw new Exception("Erro interno!", new Exception($"É obrigatório referenciar \"{this.Campo.CheckBoxes.ToString()}\""));

            List<CheckBox_Item> opcoes = this.Campo.CheckBoxes;
            List<CheckBox_Checked> res = new List<CheckBox_Checked>();
            CheckBox_Checked item = null;

            foreach (CheckBox_Item opc in opcoes) {
              item = this.CheckBoxes?.FirstOrDefault(c => c.Checked);
              res.Add(item ?? new CheckBox_Checked() {
                Checked = false,
                CheckBox_ItemId = opc.CheckBox_ItemId,
                CheckBox_Item = opc
              });
            }
            */
        }

    }
}
