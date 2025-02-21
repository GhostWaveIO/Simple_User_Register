using BrazilModels;
using Cadastro.Data;
using Cadastro.Models.Account.Cadastro.Campos;
using Cadastro.Models.Account.Cadastro.Dados;
using Cadastro.Models.Entities.DB.Account;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Cadastro.Models.Services.Account {
    public class UserEntityExtractorData {
        public UserEntityExtractorData(Usuario usuario, AppDbContext context) {
            _usuario = usuario;
            _context = context;
        }
        public UserEntityExtractorData(long userId, AppDbContext context) {
            _context = context;
        }

        public UserEntityExtractorData(Usuario usuario) {
            _usuario = usuario;
            _context = null;
        }

        private long _userId { get; set; }
        private Usuario _usuario { get; set; }
        private AppDbContext _context { get; set; }


        //############################################################################################################################
        public async Task<string> ExtractNameAsync() {
            Expression<Func<Dado_Cadastro, bool>> predicate = a => a.Campo.CampoGenerico == ECampoGenerico.Primeiro_Nome;
            IEnumerable<Dado_Cadastro> dados = await CollectDatasAsync(predicate);
            Dado_Cadastro dado = dados.FirstOrDefault();
            return dado?.Texto250;
        }

        //############################################################################################################################
        public string ExtractName() {
            Func<Dado_Cadastro, bool> predicate = a => a.Campo.CampoGenerico == ECampoGenerico.Primeiro_Nome;
            IEnumerable<Dado_Cadastro> dados = CollectDatas();
            Dado_Cadastro dado = dados.FirstOrDefault(predicate);
            return dado?.Texto250;
        }

        //public async Task<string> ExtractFullNameAsync() {

        //}

        //############################################################################################################################
        public async Task<string> ExtractEmailAsync() {
            Expression<Func<Dado_Cadastro, bool>> predicate = a => a.Campo.CampoGenerico == ECampoGenerico.Email;
            IEnumerable<Dado_Cadastro> dados = await CollectDatasAsync(predicate);
            Dado_Cadastro dado = dados.FirstOrDefault();
            return dado?.Email;
        }

        //############################################################################################################################
        public string ExtractEmail() {
            Func<Dado_Cadastro, bool> predicate = a => a.Campo.CampoGenerico == ECampoGenerico.Email;
            IEnumerable<Dado_Cadastro> dados = CollectDatas();
            Dado_Cadastro dado = dados.FirstOrDefault(predicate);
            return dado?.Email;
        }

        //############################################################################################################################
        public async Task<string> ExtractCPFAsync(bool numberOnly = false) {
            Expression<Func<Dado_Cadastro, bool>> predicate = a => a.Campo.CampoGenerico == ECampoGenerico.CPF;
            IEnumerable<Dado_Cadastro> dados = await CollectDatasAsync(predicate);
            Dado_Cadastro dado = dados.FirstOrDefault();
            string res;
            if(numberOnly) {
                res = Cpf.Format(dado.Texto250, withMask:false);
            } else {
                res = Cpf.Format(dado.Texto250, withMask: true);
            }
            return res;
        }

        //############################################################################################################################
        public string ExtractCPF(bool numberOnly = false) {
            Func<Dado_Cadastro, bool> predicate = a => a.Campo.CampoGenerico == ECampoGenerico.CPF;
            IEnumerable<Dado_Cadastro> dados = CollectDatas();
            Dado_Cadastro dado = dados.FirstOrDefault(predicate);
            string res;
            if(numberOnly) {
                res = Cpf.Format(dado.Texto250, withMask: false);
            } else {
                res = Cpf.Format(dado.Texto250, withMask: true);
            }
            return res;
        }

        //############################################################################################################################
        public string ExtractUserCode() {
            BringUser();
            return _usuario.CodigoUsuario;
        }

        //############################################################################################################################
        public async Task<string> ExtractUserCodeAsync() {
            await BringUserAsync();
            return _usuario.CodigoUsuario;
        }

        ////############################################################################################################################
        ///// <summary>Extrai "Dado_Cadastro" pelo id informado do Campo respectivo</summary>
        ///// <param name="id">Id do campo respectivo</param>
        ///// <returns>Retorna o Dado_Cadastro de acordo com o id do campo informado</returns>
        //public async Task<Dado_Cadastro> GetDataByFieldIdAsync(int id) {
        //    Expression<Func<Dado_Cadastro, bool>> predicate = a => a.Campo.Campo_CadastroId == id;

        //    IEnumerable<Dado_Cadastro> dados = await CollectDatasAsync(predicate);
        //    Dado_Cadastro dado = dados.FirstOrDefault();

        //    return dado;
        //}

        ////############################################################################################################################
        ///// <summary>Extrai "Dado_Cadastro" pelo id informado do Campo respectivo</summary>
        ///// <param name="id">Id do campo respectivo</param>
        ///// <returns>Retorna o Dado_Cadastro de acordo com o id do campo informado</returns>
        //public Dado_Cadastro ExtractDataByFieldId(int id) {
        //    Func<Dado_Cadastro, bool> predicate = a => a.Campo.Campo_CadastroId == id;

        //    IEnumerable<Dado_Cadastro> dados = CollectDatas();
        //    Dado_Cadastro dado = dados.FirstOrDefault(predicate);

        //    return dado;
        //}

        //############################################################################################################################
        /// <summary>Extrai "Dado_Cadastro" pelo id informado</summary>
        /// <param name="id">Id do "Dado_Cadastro" informado</param>
        /// <returns>Retorna o Dado_Cadastro de acordo com o id informado</returns>
        public async Task<Dado_Cadastro> GetDataBydIdAsync(int id) {
            Expression<Func<Dado_Cadastro, bool>> predicate = a => a.Dado_CadastroId == id;

            IEnumerable<Dado_Cadastro> dados = await CollectDatasAsync(predicate);
            Dado_Cadastro dado = dados.FirstOrDefault();

            return dado;
        }

        //############################################################################################################################
        /// <summary>Extrai "Dado_Cadastro" pelo id informado</summary>
        /// <param name="id">Id do "Dado_Cadastro" informado</param>
        /// <returns>Retorna o Dado_Cadastro de acordo com o id informado</returns>
        public Dado_Cadastro ExtractDataBydId(int id) {
            Func<Dado_Cadastro, bool> predicate = a => a.Dado_CadastroId == id;

            IEnumerable<Dado_Cadastro> dados = CollectDatas();
            Dado_Cadastro dado = dados.FirstOrDefault(predicate);

            return dado;
        }

        //############################################################################################################################
        private async Task<IEnumerable<Dado_Cadastro>> CollectDatasAsync(Expression<Func<Dado_Cadastro, bool>> predicate = null) {
            await BringUserAsync();
            IEnumerable<Dado_Cadastro> dados = null;
            if(_usuario.Dados.Any() || _context == null) {
                if(predicate != null)
                    dados = _usuario.Dados.Where(predicate.Compile());
                else
                    dados = _usuario.Dados;

            } else if(_context != null) {
                if(predicate != null)
                    dados = await _context.DadosCadastro.Include(d => d.Campo).Where(d => d.UsuarioId == _usuario.UsuarioId).Where(predicate).ToListAsync();
                else
                    dados = await _context.DadosCadastro.Include(d => d.Campo).Where(d => d.UsuarioId == _usuario.UsuarioId).ToListAsync();
            }

            return dados;
        }

        //############################################################################################################################
        private IEnumerable<Dado_Cadastro> CollectDatas() {
            return _usuario.Dados;
        }

        //############################################################################################################################
        private void BringUser() {
            if(_context == null)
                throw new Exception("Ocorreu um erro ao tentar consultar o banco de dados!", new Exception("Contexto não informado!"));

            if(_usuario == null)
                _usuario = _context.Usuarios.FirstOrDefault(u => u.UsuarioId == _userId);
        }

        //############################################################################################################################
        private async Task BringUserAsync() {
            if(_context == null)
                throw new Exception("Ocorreu um erro ao tentar consultar o banco de dados!", new Exception("Contexto não informado!"));

            if(_usuario == null)
                _usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuarioId == _userId);
        }
    }
}