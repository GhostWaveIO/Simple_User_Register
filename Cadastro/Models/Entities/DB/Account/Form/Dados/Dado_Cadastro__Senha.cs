using Cadastro.Models.Services.Application.Text.Generators.Hash;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Cadastro.Models.Account.Cadastro.Dados {
    public partial class Dado_Cadastro {

        //Senha
        [StringLength(50, MinimumLength = 8, ErrorMessage = "A Senha deve conter entre 8 e 50 caracteres")]
        public string Senha { get; set; }

        public string GetSenhaMD5 {
            get {
                if(String.IsNullOrWhiteSpace(this.Senha))
                    return null;
                return new GeradorMD5().GerarMD5(this.Senha);
            }
        }

        private void Preparar_Senha() {

        }

        //#######################################################################################################################
        private void VerificarCriacao_Senha() {
            if(this.Campo == null)
                throw new Exception("Campo não informado.");

            if(String.IsNullOrWhiteSpace(this.Senha_Generico))
                throw new Exception($"O campo \"{this.Campo.Label}\" é obrigatório.");

            if(this.Senha_Generico != ConfirmarSenha)
                throw new Exception("As senhas não coincidem");
            if(Senha_Generico.Length < 8)
                throw new Exception("Senha Mínimo 8 caracteres");
            if(Senha_Generico.Length > 50)
                throw new Exception("Senha Máximo 50 caracteres");
        }

        //#######################################################################################################################
        private void CorrigirCriacao_Senha() {

            this.Email = null;
            this.Numero = null;
            this.NumeroMonetario = null;
            this.Senha = new GeradorMD5().GerarMD5(this.Senha_Generico);
            this.Texto250 = null;
            this.TextoLongo = null;
        }

        //#######################################################################################################################
        private async Task Criar_Senha() {
            await _context.AddAsync(this);
            await _context.SaveChangesAsync();
        }

        //#######################################################################################################################
        private void VerificarEdicao_Senha() {
            if(this.Campo == null)
                throw new Exception("Campo não informado.");

            if(String.IsNullOrWhiteSpace(this.Senha_Generico))
                return;

            if(this.Senha_Generico != ConfirmarSenha)
                throw new Exception("As senhas não coincidem");
            if(Senha_Generico.Length < 8)
                throw new Exception("Senha Mínimo 8 caracteres");
            if(Senha_Generico.Length > 50)
                throw new Exception("Senha Máximo 50 caracteres");

        }

        //#######################################################################################################################
        private void CorrigirEdicao_Senha() {
            if(!String.IsNullOrWhiteSpace(this.Senha_Generico))
                this.Senha = new GeradorMD5().GerarMD5(this.Senha_Generico);
        }

        //#######################################################################################################################
        private void CopyToUpdate_Senha(Dado_Cadastro dado) {
            if(!String.IsNullOrWhiteSpace(this.Senha) && dado.Senha != this.Senha) {
                dado.Senha = this.Senha;
            }
        }

        //#######################################################################################################################
        private async Task Update_Senha(string cpf = null, string senha = null) {
            _context.DadosCadastro.Update(this);
            await _context.SaveChangesAsync();
        }

    }
}
