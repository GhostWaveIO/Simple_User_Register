using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cadastro.Models.Account.Cadastro.Campos {

    public partial class Campo_Cadastro {

        //#### Criação ####
        //###################################################################################################
        private async Task Criar_Texto250() {
            ////Remove CampoNomeCompleto de qualquer campos caso este seja o campo de nome completo (Somente um campo pode ser de nome completo)
            //if (this.CampoNomeCompleto && await _context.CamposCadastro.AnyAsync(c => c.CampoNomeCompleto) && this.ModeloCampo == EModeloCampo.Texto_250) {
            //    List<Campo_Cadastro> camposNomeCompleto = await _context.CamposCadastro.Where(c => c.CampoNomeCompleto).ToListAsync();
            //    foreach (Campo_Cadastro campo in camposNomeCompleto) {
            //        campo.CampoNomeCompleto = false;
            //    }
            //    if (camposNomeCompleto.Any()) {
            //        _context.CamposCadastro.UpdateRange(camposNomeCompleto);
            //        await _context.SaveChangesAsync();
            //    }
            //    this.CampoNomeCompleto = true;
            //}
        }

        //###################################################################################################
        private void VerificarCriacao_Texto250() {
            if (this.ComprimentoTextoMax < 0 || this.ComprimentoTextoMax > 250)
                throw new Exception("O comprimento deve conter entre 0 e 250");
        }

        //###################################################################################################
        private void CorrigirCriacao_Texto250() {
            this.Html = String.Empty;
            this.CasasDecimais = 0;
            this.Min = 0;
            this.Max = 0;
            this.NumeroPreenchido = 0f;
            this.PlaceHolderConfirmarSenha = String.Empty;
            if (this.CampoGenerico != ECampoGenerico.Campo_Personalizado)
                this.ExpressaoRegular = String.Empty;
        }

        //#### Edição ####

        //###################################################################################################
        private void VerificarEdicao_Texto250() {
            if (this.CampoGenerico == ECampoGenerico.CPF && this.ComprimentoTextoMax < 11) {
                throw new Exception("Mínimo permitido 11 caracteres para campo de CPF!");
            }
            if (this.ComprimentoTextoMax < 0 || this.ComprimentoTextoMax > 250)
                throw new Exception("O comprimento deve conter entre 0 e 250");
        }

        //###################################################################################################
        private void CorrigirEdicao_Texto250() {

        }

        //###################################################################################################
        private void CopyToUpdate_Texto250(Campo_Cadastro campo) {
            if (!this.Generico) {
                campo.Required = this.Required;
                campo.StartCriacaoCampo = this.StartCriacaoCampo;
                campo.Unico = this.Unico;
                campo.ExpressaoRegular = this.ExpressaoRegular;
            }
            //if (!this.Generico || this.CampoGenerico == ECampoGenerico.Primeiro_Nome) {
            //    campo.CampoNomeCompleto = this.CampoNomeCompleto;
            //}
            campo.PermitirPesquisa = this.PermitirPesquisa;


            campo.ComprimentoTextoMax = this.ComprimentoTextoMax;
            campo.AutorizadoEditar = this.AutorizadoEditar;
            campo.Label = this.Label;
            //campo.CasasDecimais = this.CasasDecimais;
            //campo.Min = this.Min;
            //campo.Max = this.Max;
            campo.TextoPreenchido = this.TextoPreenchido?.Trim();
            //campo.NumeroPreenchido = this.NumeroPreenchido;
            campo.PlaceHolder = this.PlaceHolder;
            //campo.DirecaoEixo = this.DirecaoEixo;

        }

        //###################################################################################################
        private async Task Update_Texto250() {

            ////Remove CampoNomeCompleto de qualquer campos caso este seja o campo de nome completo (Somente um campo pode ser NomeCompleto)
            //if (this.CampoNomeCompleto && await _context.CamposCadastro.AnyAsync(c => c.Campo_CadastroId == this.Campo_CadastroId && !c.CampoNomeCompleto) && await _context.CamposCadastro.AnyAsync(c => c.CampoNomeCompleto) && this.ModeloCampo == EModeloCampo.Texto_250) {
            //    List<Campo_Cadastro> camposNomeCompleto = await _context.CamposCadastro.Where(c => c.CampoNomeCompleto).ToListAsync();
            //    foreach (Campo_Cadastro campo in camposNomeCompleto) {
            //        campo.CampoNomeCompleto = false;
            //    }
            //    if (camposNomeCompleto.Any()) {
            //        _context.CamposCadastro.UpdateRange(camposNomeCompleto);
            //        await _context.SaveChangesAsync();
            //    }
            //    this.CampoNomeCompleto = true;
            //}
        }

    }
}
