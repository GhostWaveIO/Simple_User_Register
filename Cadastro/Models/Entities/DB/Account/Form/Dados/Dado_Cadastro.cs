using Cadastro.Data;
using Cadastro.Models.Account.Cadastro.Campos;
using Cadastro.Models.Entities.DB.Account;
using Cadastro.Models.ViewModel.Application.AccessContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Cadastro.Models.Account.Cadastro.Dados
{
    public enum ELocalDeOperacao { Cadastro, Perfil }
    public partial class Dado_Cadastro {
        [Key]
        public long Dado_CadastroId { get; set; }

        public AppDbContext _context { get; set; }




        public bool Ativo { get; set; }

        // Local
        /// <summary> Usado para salvar nome da Imagem ou Documento </summary>
        [StringLength(150)]
        public string Local { get; set; }

        //Dados de criptografia
        /// <summary>Define os caracteres da criptografia do dado correspondente</summary>
        public string DadosCriptografados { get; set; }

        [NotMapped]
        public EModeloCampo GetModeloCampo {
            get {
                return this.Campo?.ModeloCampo ?? (EModeloCampo)0;
            }
        }

        [NotMapped]
        public ECampoGenerico GetCampoGenerico {
            get {
                return this.Campo?.CampoGenerico ?? (ECampoGenerico)0;
            }
        }


        #region Entidades

        public int Campo_CadastroId { get; set; }
        public Campo_Cadastro Campo { get; set; }

        public long UsuarioId { get; set; }
        public Usuario Usuario { get; set; }

        #endregion FIM| Entidades


        public void PrepararDado(EModeloCampo modelo) {
            switch (modelo) {
                case EModeloCampo.Texto_250:
                    this.Preparar_Texto250();
                    break;
                case EModeloCampo.Texto_Longo:
                    this.Preparar_TextoLongo();
                    break;
                case EModeloCampo.Email:
                    this.Preparar_Email();
                    break;
                case EModeloCampo.Número:
                    this.Preparar_Numero();
                    break;
                case EModeloCampo.Número_Monetário:
                    this.Preparar_NumeroMonetario();
                    break;
                case EModeloCampo.Select:
                    this.Preparar_Select();
                    break;
                case EModeloCampo.MultiSelect:
                    this.Preparar_MultiSelect();
                    break;
                case EModeloCampo.CheckBox:
                    this.Preparar_CheckBox();
                    break;
                case EModeloCampo.RadioButton:
                    this.Preparar_RadioButton();
                    break;
                case EModeloCampo.Imagem:
                    this.Preparar_Imagem();
                    break;
                case EModeloCampo.Documento:
                    this.Preparar_Documento();
                    break;
            }
        }

        //########################################################################################################################
        public async Task Criar() {
            if (!this.Campo.Ativo || this.Campo.ModeloCampo == EModeloCampo.Confirmar_Senha) return;
            //Selects
            if (this.Campo.ModeloCampo == EModeloCampo.Select && !this.Campo.Selects.Any()) return;
            //Checkbox
            if (this.Campo.ModeloCampo == EModeloCampo.CheckBox && !this.Campo.CheckBoxes.Any()) return;
            //RadioButton
            if (this.Campo.ModeloCampo == EModeloCampo.RadioButton && !this.Campo.RadioButtons.Any()) return;

            switch (this.Campo.ModeloCampo) {
                case EModeloCampo.Texto_250:
                    await this.Criar_Texto250();
                    break;
                case EModeloCampo.Texto_Longo:
                    await this.Criar_TextoLongo();
                    break;
                case EModeloCampo.Email:
                    await this.Criar_Email();
                    break;
                case EModeloCampo.Número:
                    await this.Criar_Numero();
                    break;
                case EModeloCampo.Número_Monetário:
                    await this.Criar_NumeroMonetario();
                    break;
                case EModeloCampo.Select:
                    await this.Criar_Select();
                    break;
                case EModeloCampo.MultiSelect:
                    this.Criar_MultiSelect();
                    break;
                case EModeloCampo.CheckBox:
                    await this.Criar_CheckBox();
                    break;
                case EModeloCampo.RadioButton:
                    await this.Criar_RadioButton();
                    break;
                case EModeloCampo.Imagem:
                    await this.Criar_Imagem();
                    break;
                case EModeloCampo.Documento:
                    await this.Criar_Documento();
                    break;
                case EModeloCampo.Senha:
                    await this.Criar_Senha();
                    break;
            }
        }

        //########################################################################################################################
        public async Task VerificarCriacao(ELocalDeOperacao local) {
            if (_context == null) throw new Exception("Ocorreu um erro ao tentar cadastrar. Nenhum contexto encontrado!");
            if (!await _context.CamposCadastro.AnyAsync(c => c.Campo_CadastroId == this.Campo_CadastroId)) throw new Exception("Ocorreu um erro ao tentar cadastrar, tente novamente!", new Exception("O campo referente a este dado não foi encontrado"));

            this.Campo = await _context.CamposCadastro.Include(cmp => cmp.CheckBoxes).Include(cmp => cmp.Selects).Include(cmp => cmp.RadioButtons).FirstOrDefaultAsync(c => c.Campo_CadastroId == this.Campo.Campo_CadastroId);
            if (this.Campo == null) throw new Exception("Entidade do campo não informado ou campo não existe. Tente novamente mais tarde!");


            if (!this.Campo.Ativo || this.Campo.ModeloCampo == EModeloCampo.Confirmar_Senha || this.Campo.ModeloCampo == EModeloCampo.Html || this.Campo.ModeloCampo == EModeloCampo.Vídeo_Youtube || (this.Campo.StartCriacaoCampo == EStartCriacaoCampo.Perfil && local == ELocalDeOperacao.Cadastro)) return;

            //Verifica se o Campo é exatamente este mesmo modelo
            if (!await _context.CamposCadastro.AnyAsync(c => c.Campo_CadastroId == this.Campo.Campo_CadastroId && c.ModeloCampo == this.Campo.ModeloCampo)) throw new Exception("As informações de modelo de campo não coincidem.");

            if (this.Campo_CadastroId == 0) throw new Exception("Erro interno. O id do campo é nulo ou 0");

            switch (this.Campo.ModeloCampo) {
                case EModeloCampo.Texto_250:
                    await this.VerificarCriacao_Texto250();
                    break;
                case EModeloCampo.Texto_Longo:
                    await this.VerificarCriacao_TextoLongo();
                    break;
                case EModeloCampo.Email:
                    await this.VerificarCriacao_Email();
                    break;
                case EModeloCampo.Número:
                    await this.VerificarCriacao_Numero();
                    break;
                case EModeloCampo.Número_Monetário:
                    await this.VerificarCriacao_NumeroMonetario();
                    break;
                case EModeloCampo.Select:
                    this.VerificarCriacao_Select();
                    break;
                case EModeloCampo.MultiSelect:
                    this.VerificarCriacao_MultiSelect();
                    break;
                case EModeloCampo.CheckBox:
                    this.VerificarCriacao_CheckBox();
                    break;
                case EModeloCampo.RadioButton:
                    this.VerificarCriacao_RadioButton();
                    break;
                case EModeloCampo.Imagem:
                    this.VerificarCriacao_Imagem();
                    break;
                case EModeloCampo.Documento:
                    this.VerificarCriacao_Documento();
                    break;
                case EModeloCampo.Senha:
                    this.VerificarCriacao_Senha();
                    break;
            }
        }

        //########################################################################################################################
        public void CorrigirCriacao() {
            if (this.Campo == null) throw new Exception("Entidade do campo não informado");

            switch (this.Campo.ModeloCampo) {
                case EModeloCampo.Texto_250:
                    this.CorrigirCriacao_Texto250();
                    break;
                case EModeloCampo.Texto_Longo:
                    this.CorrigirCriacao_TextoLongo();
                    break;
                case EModeloCampo.Email:
                    this.CorrigirCriacao_Email();
                    break;
                case EModeloCampo.Número:
                    this.CorrigirCriacao_Numero();
                    break;
                case EModeloCampo.Número_Monetário:
                    this.CorrigirCriacao_NumeroMonetario();
                    break;
                case EModeloCampo.Select:
                    this.CorrigirCriacao_Select();
                    break;
                case EModeloCampo.MultiSelect:
                    this.CorrigirCriacao_MultiSelect();
                    break;
                case EModeloCampo.CheckBox:
                    this.CorrigirCriacao_CheckBox();
                    break;
                case EModeloCampo.RadioButton:
                    this.CorrigirCriacao_RadioButton();
                    break;
                case EModeloCampo.Imagem:
                    this.CorrigirCriacao_Imagem();
                    break;
                case EModeloCampo.Documento:
                    this.CorrigirCriacao_Documento();
                    break;
                case EModeloCampo.Senha:
                    this.CorrigirCriacao_Senha();
                    break;
            }
        }

        //########################################################################################################################
        /// <summary>Verifica se o próprio proprietário ou o autorizado tem autorização de edição do dado atual</summary>
        /// <param name="proprietario">Entidade de usuário do proprietário da conta</param>
        /// <param name="acessoOperador">Acesso de serviço do operador</param>
        /// <returns>Retorna true se está autorizado a editar</returns>
        public async Task<bool> VerificarAutorizacaoEdicao(Usuario proprietario, AcessoServiceVM acessoOperador) {
            bool res = false;

            //return true;///################################################################REMOVER

            //if (acessoOperador._usuario.IsAdmin() && this.Campo.Generico) return true;

            switch (this.Campo.AutorizadoEditar) {
                case EAutorizadoEditar.Proprietário_e_Autorizado:
                    res = true;
                    break;
                case EAutorizadoEditar.Somente_Proprietário:
                    //Se proprietário é o operador, retornar true
                    res = proprietario.Equals(acessoOperador._usuario) || !await _context.DadosCadastro.ContainsAsync(this);
                    break;
                case EAutorizadoEditar.Somente_Autorizado:
                    res = acessoOperador._permissao.EditarDadosUsuarios || (!await _context.DadosCadastro.ContainsAsync(this) && this.Campo.StartCriacaoCampo == EStartCriacaoCampo.Cadastro);
                    break;
                case EAutorizadoEditar.Não_Editável:
                    if (!await _context.DadosCadastro.ContainsAsync(this))
                        res = true;
                    else
                        res = false;
                    break;
            }

            return res;
        }

        //########################################################################################################################
        public async Task VerificarEdicao(ELocalDeOperacao local) {
            this.Campo = await _context.CamposCadastro.Include(cmp => cmp.CheckBoxes).Include(cmp => cmp.Selects).Include(cmp => cmp.RadioButtons).FirstOrDefaultAsync(c => c.Campo_CadastroId == this.Campo.Campo_CadastroId);
            if (this.Campo == null) throw new Exception("Entidade do campo não informado ou campo não existe. Tente novamente mais tarde!");


            if (!this.Campo.Ativo || this.Campo.ModeloCampo == EModeloCampo.Confirmar_Senha || this.Campo.ModeloCampo == EModeloCampo.Html || this.Campo.ModeloCampo == EModeloCampo.Html) return;

            //Verifica se o Campo é exatamente este mesmo modelo
            if (!await _context.CamposCadastro.AnyAsync(c => c.Campo_CadastroId == this.Campo.Campo_CadastroId && c.ModeloCampo == this.Campo.ModeloCampo)) throw new Exception("As informações de modelo de campo não coincidem.");

            if (this.Campo_CadastroId == 0) throw new Exception("Erro interno. O id do campo é nulo ou 0");

            switch (this.Campo.ModeloCampo) {
                case EModeloCampo.Texto_250:
                    await this.VerificarEdicao_Texto250();
                    break;
                case EModeloCampo.Texto_Longo:
                    await this.VerificarEdicao_TextoLongo();
                    break;
                case EModeloCampo.Email:
                    await this.VerificarEdicao_Email(local);
                    break;
                case EModeloCampo.Número:
                    await this.VerificarEdicao_Numero();
                    break;
                case EModeloCampo.Número_Monetário:
                    await this.VerificarEdicao_NumeroMonetario();
                    break;
                case EModeloCampo.Select:
                    this.VerificarEdicao_Select();
                    break;
                case EModeloCampo.MultiSelect:
                    this.VerificarEdicao_MultiSelect();
                    break;
                case EModeloCampo.CheckBox:
                    await this.VerificarEdicao_CheckBox();
                    break;
                case EModeloCampo.RadioButton:
                    this.VerificarEdicao_RadioButton();
                    break;
                case EModeloCampo.Imagem:
                    this.VerificarEdicao_Imagem();
                    break;
                case EModeloCampo.Documento:
                    this.VerificarEdicao_Documento();
                    break;
                case EModeloCampo.Senha:
                    this.VerificarEdicao_Senha();
                    break;
            }
        }

        //########################################################################################################################
        public void CorrigirEdicao() {
            if (this.Campo == null) throw new Exception("Entidade do campo não informado");

            switch (this.Campo.ModeloCampo) {
                case EModeloCampo.Texto_250:
                    this.CorrigirEdicao_Texto250();
                    break;
                case EModeloCampo.Texto_Longo:
                    this.CorrigirEdicao_TextoLongo();
                    break;
                case EModeloCampo.Email:
                    this.CorrigirEdicao_Email();
                    break;
                case EModeloCampo.Número:
                    this.CorrigirEdicao_Numero();
                    break;
                case EModeloCampo.Número_Monetário:
                    this.CorrigirEdicao_NumeroMonetario();
                    break;
                case EModeloCampo.Select:
                    this.CorrigirEdicao_Select();
                    break;
                case EModeloCampo.MultiSelect:
                    this.CorrigirEdicao_MultiSelect();
                    break;
                case EModeloCampo.CheckBox:
                    this.CorrigirEdicao_CheckBox();
                    break;
                case EModeloCampo.RadioButton:
                    this.CorrigirEdicao_RadioButton();
                    break;
                case EModeloCampo.Imagem:
                    this.CorrigirEdicao_Imagem();
                    break;
                case EModeloCampo.Documento:
                    this.CorrigirEdicao_Documento();
                    break;
                case EModeloCampo.Senha:
                    this.CorrigirEdicao_Senha();
                    break;
            }
        }

        //########################################################################################################################
        public void CopyToUpdate(Dado_Cadastro dado) {
            switch (this.Campo.ModeloCampo) {
                case EModeloCampo.Texto_250:
                    this.CopyToUpdate_Texto250(dado);
                    break;
                case EModeloCampo.Texto_Longo:
                    this.CopyToUpdate_TextoLongo(dado);
                    break;
                case EModeloCampo.Email:
                    this.CopyToUpdate_Email(dado, ELocalDeOperacao.Perfil);
                    break;
                case EModeloCampo.Número:
                    this.CopyToUpdate_Numero(dado);
                    break;
                case EModeloCampo.Número_Monetário:
                    this.CopyToUpdate_NumeroMonetario(dado);
                    break;
                case EModeloCampo.Select:
                    this.CopyToUpdate_Select(dado);
                    break;
                case EModeloCampo.MultiSelect:
                    this.CopyToUpdate_MultiSelect(dado);
                    break;
                case EModeloCampo.CheckBox:
                    this.CopyToUpdate_CheckBox(dado);
                    break;
                case EModeloCampo.RadioButton:
                    this.CopyToUpdate_RadioButton(dado);
                    break;
                case EModeloCampo.Imagem:
                    this.CopyToUpdate_Imagem(dado);
                    break;
                case EModeloCampo.Documento:
                    this.CopyToUpdate_Documento(dado);
                    break;
                case EModeloCampo.Senha:
                    this.CopyToUpdate_Senha(dado);
                    break;
            }
        }

        //########################################################################################################################
        public async Task Update(string cpf = null, string senha = null) {

            switch (this.Campo.ModeloCampo) {
                case EModeloCampo.Texto_250:
                    await this.Update_Texto250();
                    break;
                case EModeloCampo.Texto_Longo:
                    await this.Update_TextoLongo();
                    break;
                case EModeloCampo.Email:
                    await this.Update_Email();
                    break;
                case EModeloCampo.Número:
                    await this.Update_Numero();
                    break;
                case EModeloCampo.Número_Monetário:
                    await this.Update_NumeroMonetario();
                    break;
                case EModeloCampo.Select:
                    await this.Update_Select();
                    break;
                case EModeloCampo.MultiSelect:
                    this.Update_MultiSelect();
                    break;
                case EModeloCampo.CheckBox:
                    await this.Update_CheckBox();
                    break;
                case EModeloCampo.RadioButton:
                    await this.Update_RadioButton();
                    break;
                case EModeloCampo.Imagem:
                    await this.Update_Imagem();
                    break;
                case EModeloCampo.Documento:
                    await this.Update_Documento();
                    break;
                case EModeloCampo.Senha:
                    await this.Update_Senha(cpf, senha);
                    break;
            }
        }

        //########################################################################################################################
        public void Criptografar() {

            switch (this.Campo.ModeloCampo) {
                case EModeloCampo.Texto_250:
                    this.Criptografar_Texto250();
                    break;
                case EModeloCampo.Texto_Longo:
                    this.Criptografar_TextoLongo();
                    break;
                    /*case EModeloCampo.Email:
                      this.Criptografar_Email();
                      break;
                    case EModeloCampo.Número:
                      this.Criptografar_Numero();
                      break;
                    case EModeloCampo.Número_Monetário:
                      this.Criptografar_NumeroMonetario();
                      break;
                    case EModeloCampo.Select:
                      this.Criptografar_Select();
                      break;
                    case EModeloCampo.MultiSelect:
                      this.Criptografar_MultiSelect();
                      break;
                    case EModeloCampo.CheckBox:
                      this.Criptografar_CheckBox();
                      break;
                    case EModeloCampo.RadioButton:
                      this.Criptografar_RadioButton();
                      break;
                    case EModeloCampo.Imagem:
                      this.Criptografar_Imagem();
                      break;
                    case EModeloCampo.Documento:
                      this.Criptografar_Documento();
                      break;
                    case EModeloCampo.Senha:
                      this.Criptografar_Senha();
                      break;*/
            }
        }

        //########################################################################################################################
        public void Descriptografar() {

            switch (this.Campo.ModeloCampo) {
                case EModeloCampo.Texto_250:
                    this.Descriptografar_Texto250();
                    break;
                case EModeloCampo.Texto_Longo:
                    this.Descriptografar_TextoLongo();
                    break;
                    /*case EModeloCampo.Email:
                      this.Descriptografar_Email();
                      break;
                    case EModeloCampo.Número:
                      this.Descriptografar_Numero();
                      break;
                    case EModeloCampo.Número_Monetário:
                      this.Descriptografar_NumeroMonetario();
                      break;
                    case EModeloCampo.Select:
                      this.Descriptografar_Select();
                      break;
                    case EModeloCampo.MultiSelect:
                      this.Descriptografar_MultiSelect();
                      break;
                    case EModeloCampo.CheckBox:
                      this.Descriptografar_CheckBox();
                      break;
                    case EModeloCampo.RadioButton:
                      this.Descriptografar_RadioButton();
                      break;
                    case EModeloCampo.Imagem:
                      this.Descriptografar_Imagem();
                      break;
                    case EModeloCampo.Documento:
                      this.Descriptografar_Documento();
                      break;
                    case EModeloCampo.Senha:
                      this.Descriptografar_Senha();
                      break;*/
            }
        }

    }
}
