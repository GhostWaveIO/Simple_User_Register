using BrazilModels;
using Cadastro.Models.Account.Cadastro.Campos;
using Cadastro.Models.Services.Application.Security.Cryptography.Rijndael;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cadastro.Models.Account.Cadastro.Dados {
    public partial class Dado_Cadastro {

        //Texto até 250 caracteres
        [StringLength(250, ErrorMessage = "Máximo 250 caracteres")]
        public string Texto250 { get; set; }

        //Texto até 250 caracteres - Obrigatório
        [NotMapped]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(250, ErrorMessage = "Máximo {1} caracteres")]
        public string Texto250_Required {
            get { return this.Texto250; }
            set { this.Texto250 = value; }
        }

        public string GetCpfComMascara() {
            if(this.Campo == null)
                throw new Exception("Campo não associado!");
            if(this.Campo.CampoGenerico != ECampoGenerico.CPF)
                throw new Exception("Este não é um CPF!");
            if(!Regex.IsMatch(this.Texto250, "^(([0-9]{3}[.]{1}[0-9]{3}[.]{1}[0-9]{3}[-]{1}[0-9]{2})|([0-9]{11}))$"))
                throw new Exception("Formato de CPF inválido");

            string numeros = "";
            string res;

            //Coleta somente números
            foreach(char ch in this.Texto250) {
                if(Char.IsDigit(ch))
                    numeros += ch;
            }

            if(numeros.Length != 11)
                throw new Exception("Cpf Incorreto!");

            //Insere caracteres de mascaramento
            res = numeros.Insert(9, "-").Insert(6, ".").Insert(3, ".");

            if(!Regex.IsMatch(res, "^(([0-9]{3}[.]{1}[0-9]{3}[.]{1}[0-9]{3}[-]{1}[0-9]{2})|([0-9]{11}))$"))
                throw new Exception("Formato de CPF inválido");

            return res;
        }

        //#######################################################################################################################
        private void Preparar_Texto250() {
            this.Texto250 = this.Campo.TextoPreenchido;
        }

        //#######################################################################################################################
        private async Task VerificarCriacao_Texto250() {
            if(this.Campo == null)
                throw new Exception("Campo não informado.");

            string supostoTexto = null;

            if(!String.IsNullOrEmpty(this.Cpf_Generico) && this.Campo.CampoGenerico == ECampoGenerico.CPF)
                supostoTexto = this.Cpf_Generico;
            else
                supostoTexto = this.Texto250;

            int tamanhoTexto = (supostoTexto?.Length ?? 0);

            if(tamanhoTexto > 250 && this.Campo.CampoGenerico == ECampoGenerico.Campo_Personalizado)
                throw new Exception($"Tamanho do texto acima do limite máximo no campo \"{this.Campo.Label}\".");
            if(tamanhoTexto > this.Campo.ComprimentoTextoMax && this.Campo.CampoGenerico == ECampoGenerico.Campo_Personalizado)
                throw new Exception($"Tamanho do texto acima do permitido no campo \"{this.Campo.Label}\".");
            if(this.Campo.CampoGenerico == ECampoGenerico.CPF && (String.IsNullOrEmpty(supostoTexto) || !Regex.IsMatch(supostoTexto, "^(([0-9]{3}[.]{1}[0-9]{3}[.]{1}[0-9]{3}[-]{1}[0-9]{2})|([0-9]{11}))$")))
                throw new Exception($"Exemplo: xxx.xxx.xxx-xx. No campo \"{this.Campo.Label}\"");
            if(this.Campo.CampoGenerico == ECampoGenerico.CPF && !Cpf.Validate(supostoTexto))
                throw new Exception("Cpf inválido!");

            if(this.Campo.Required && String.IsNullOrWhiteSpace(supostoTexto))
                throw new Exception($"O campo \"{this.Campo.Label}\" é obrigatório.");
            //Verificar se CPF já existe
            if(this.Campo.CampoGenerico == Campos.ECampoGenerico.CPF) {
                if(await _context.DadosCadastro.AnyAsync(d =>
                d.Texto250.Replace(".", "").Replace(",", "").Replace("-", "").Replace("_", "").Replace("+", "") == supostoTexto.Replace(".", "").Replace(",", "").Replace("-", "").Replace("_", "").Replace("+", "")))
                    throw new Exception("Já existe uma conta usando este mesmo CPF.");
            }
            if((this.Campo.Unico || this.Campo.CampoGenerico == ECampoGenerico.CPF) && supostoTexto != null) {
                if(await _context.DadosCadastro.AnyAsync(d => d.Texto250 == supostoTexto && d.Campo_CadastroId == this.Campo_CadastroId))
                    throw new Exception($"Já existe uma conta com este mesmo texto no campo \"{this.Campo.Label}\"");
            }
        }

        //#######################################################################################################################
        private void CorrigirCriacao_Texto250() {
            if(this.Campo.CampoGenerico == ECampoGenerico.CPF)
                this.Texto250 = this.Cpf_Generico;

            this.Email = null;
            this.Numero = null;
            this.NumeroMonetario = null;
            this.Senha = "";
            //this.Texto250 = null;
            this.TextoLongo = null;
        }

        //#######################################################################################################################
        private async Task Criar_Texto250() {
            await _context.AddAsync(this);
            await _context.SaveChangesAsync();
        }

        //#######################################################################################################################
        private async Task VerificarEdicao_Texto250() {
            if(this.Campo == null)
                throw new Exception("Campo não informado.");

            string supostoTexto = null;

            if(!String.IsNullOrEmpty(this.Cpf_Generico) && this.Campo.CampoGenerico == ECampoGenerico.CPF)
                supostoTexto = this.Cpf_Generico;
            else
                supostoTexto = this.Texto250;

            int tamanhoTexto = (supostoTexto?.Length ?? 0);

            if(tamanhoTexto > 250 && this.Campo.CampoGenerico == ECampoGenerico.Campo_Personalizado)
                throw new Exception($"Tamanho do texto acima do limite máximo no campo \"{this.Campo.Label}\".");
            if(this.Campo.CampoGenerico == ECampoGenerico.Campo_Personalizado && tamanhoTexto > this.Campo.ComprimentoTextoMax)
                throw new Exception($"Tamanho do texto acima do permitido no campo \"{this.Campo.Label}\".");
            if(this.Campo.CampoGenerico == ECampoGenerico.CPF && (String.IsNullOrEmpty(supostoTexto) || !Regex.IsMatch(supostoTexto, "^(([0-9]{3}[.]{1}[0-9]{3}[.]{1}[0-9]{3}[-]{1}[0-9]{2})|([0-9]{11}))$")))
                throw new Exception($"Exemplo: xxx.xxx.xxx-xx. No campo \"{this.Campo.Label}\"");
            if(this.Campo.CampoGenerico == ECampoGenerico.CPF && !Cpf.Validate(supostoTexto))
                throw new Exception("CPF inválido!");

            if((this.Campo.Required || this.Campo.Generico) && String.IsNullOrWhiteSpace(supostoTexto))
                throw new Exception($"O campo \"{this.Campo.Label}\" é obrigatório.");
            if((this.Campo.Unico || this.Campo.CampoGenerico == ECampoGenerico.CPF) && supostoTexto != null) {
                if(await _context.DadosCadastro.AnyAsync(d => d.Texto250 == supostoTexto && d.Campo_CadastroId == this.Campo_CadastroId && d.Dado_CadastroId != this.Dado_CadastroId))
                    throw new Exception($"Já existe uma conta com este mesmo texto no campo \"{this.Campo.Label}\"");
            }
        }

        //#######################################################################################################################
        private void CorrigirEdicao_Texto250() {
            if(this.Campo.CampoGenerico == ECampoGenerico.CPF)
                this.Texto250 = this.Cpf_Generico;
        }

        //#######################################################################################################################
        private void CopyToUpdate_Texto250(Dado_Cadastro dado) {
            if(this.Campo.CampoGenerico == ECampoGenerico.CPF)
                dado.Texto250 = this.Cpf_Generico;
            else
                dado.Texto250 = this.Texto250;
        }

        //#######################################################################################################################
        private async Task Update_Texto250() {
            if(!await _context.DadosCadastro.ContainsAsync(this))
                throw new Exception($"Este dado do campo \"{this.Campo.Label}\"");

            _context.DadosCadastro.Update(this);
            await _context.SaveChangesAsync();
        }

        //###################################################################################################
        public void Criptografar_Texto250() {
            CriptografiaRijndael crpWithKey = new CriptografiaRijndael();
            crpWithKey.ChaveObrigatoria = true;

            if(this.Texto250 == null)
                return;

            try {
                this.DadosCriptografados = crpWithKey.Encrypt(this.Texto250);
                this.Texto250 = null;
            } catch(Exception err) {
                throw new Exception("Erro interno. Não foi possível ler os dados!", err);
            }
        }

        //###################################################################################################
        public void Descriptografar_Texto250() {
            CriptografiaRijndael crpWithKey = new CriptografiaRijndael();
            crpWithKey.ChaveObrigatoria = true;
            if(String.IsNullOrEmpty(this.DadosCriptografados))
                return;
            if(!crpWithKey.VerificarPrefixo(this.DadosCriptografados))
                throw new Exception("Ocorreu um erro ao tentar ler os dados!", new Exception("Este texto não parece ser uma criptografia."));

            if(this.DadosCriptografados == null)
                return;

            try {
                this.Texto250 = crpWithKey.Decrypt(this.DadosCriptografados);
                this.TextoLongo = null;
            } catch(Exception err) {
                throw new Exception("Erro interno. Não foi possível ler os dados!", err);
            }

        }
    }
}
