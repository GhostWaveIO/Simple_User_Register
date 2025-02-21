using Cadastro.Data;
using Cadastro.Models.Account.Cadastro.Campos;
using Cadastro.Models.Entities.DB.Account.Form;
using Cadastro.Models.Entities.DB.Account.Form.Seletores;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cadastro.Models.Services.Application {

    /// <summary> É usado quando se inicia uma nova seção do usuários. É feitos verificações para correção de possíveis más configurações </summary>
    public class StartSession {

        public AppDbContext _context { get; set; }

        public StartSession(AppDbContext context) {
            _context = context;
        }

        //##############################################################################################################################
        public async Task CorrecaoGeral() {
            if(_context == null)
                throw new Exception("erro interno. Contexto não informado.");

            //Verificar se existe algum Campo do Cadastro criado, e caso não exista criá-lo
            if(!await _context.CamposCadastro.AnyAsync()) {
                await CriarCamposGenericos();
            }
        }

        //##############################################################################################################################
        private async Task CriarCamposGenericos() {
            //Criar grupo genérico
            Grupo_Cadastro grupo = new Grupo_Cadastro() {
                Generico = true
            };
            await _context.GruposCadastro.AddAsync(grupo);
            await _context.SaveChangesAsync();

            //Cria linhas
            Dictionary<string, Linha_Cadastro> dicionarioLinhas = new Dictionary<string, Linha_Cadastro>() {
        { "Email", new Linha_Cadastro() { Grupo_CadastroId = grupo.Grupo_CadastroId, Ordem = 0 } },
        { "Senha", new Linha_Cadastro() { Grupo_CadastroId = grupo.Grupo_CadastroId, Ordem = 1 } },
        { "ConfirmarSenha", new Linha_Cadastro() { Grupo_CadastroId = grupo.Grupo_CadastroId, Ordem = 2 } },
        { "PrimeiroNome", new Linha_Cadastro() { Grupo_CadastroId = grupo.Grupo_CadastroId, Ordem = 3 } },
        { "CPF", new Linha_Cadastro() { Grupo_CadastroId = grupo.Grupo_CadastroId, Ordem = 4 } },
        { "Estado", new Linha_Cadastro() { Grupo_CadastroId = grupo.Grupo_CadastroId, Ordem = 5 } }
      };
            await _context.LinhasCadastro.AddRangeAsync(dicionarioLinhas.Select(l => l.Value));
            await _context.SaveChangesAsync();

            //Criar Campos
            Dictionary<string, Campo_Cadastro> dicionarioCampos = new Dictionary<string, Campo_Cadastro>();
            //Email
            dicionarioCampos.Add("Email",
              new Campo_Cadastro() {
                  Ativo = true,
                  Ordem_Cadastro = 0,
                  ModeloCampo = EModeloCampo.Email,
                  CampoGenerico = ECampoGenerico.Email,
                  Required = true,
                  Unico = true,
                  Nome = "Email de Login",
                  Label = "Email",
                  PlaceHolder = "Ex: usuario@dominio.com.br",
                  ColunasXS_Cadastro = 12,
                  ColunasSM_Cadastro = 12,
                  ColunasMD_Cadastro = 12,
                  ColunasLG_Cadastro = 12,
                  ColunasXL_Cadastro = 12,
                  Generico = true,
                  Linha_CadastroId = dicionarioLinhas["Email"].Linha_CadastroId
              }
            );
            //Senha
            dicionarioCampos.Add("Senha",
              new Campo_Cadastro() {
                  Ativo = true,
                  Ordem_Cadastro = 0,
                  ModeloCampo = EModeloCampo.Senha,
                  CampoGenerico = ECampoGenerico.Senha,
                  Required = true,
                  Nome = "Senha",
                  Label = "Senha",
                  PlaceHolder = "Digite uma senha",
                  ColunasXS_Cadastro = 12,
                  ColunasSM_Cadastro = 12,
                  ColunasMD_Cadastro = 12,
                  ColunasLG_Cadastro = 12,
                  ColunasXL_Cadastro = 12,
                  Generico = true,
                  Linha_CadastroId = dicionarioLinhas["Senha"].Linha_CadastroId
              }
            );
            //Confirmar Senha
            dicionarioCampos.Add("ConfirmarSenha",
              new Campo_Cadastro() {
                  Ativo = true,
                  Ordem_Cadastro = 0,
                  ModeloCampo = EModeloCampo.Confirmar_Senha,
                  CampoGenerico = ECampoGenerico.ConfirmarSenha,
                  Required = true,
                  Nome = "Confirmar Senha",
                  Label = "Confirmar Senha",
                  PlaceHolder = "Confirme a Senha",
                  ColunasXS_Cadastro = 12,
                  ColunasSM_Cadastro = 12,
                  ColunasMD_Cadastro = 12,
                  ColunasLG_Cadastro = 12,
                  ColunasXL_Cadastro = 12,
                  Generico = true,
                  Linha_CadastroId = dicionarioLinhas["ConfirmarSenha"].Linha_CadastroId
              }
            );
            //Primeiro Nome
            dicionarioCampos.Add("PrimeiroNome",
              new Campo_Cadastro() {
                  Ativo = true,
                  Ordem_Cadastro = 0,
                  ModeloCampo = EModeloCampo.Texto_250,
                  CampoGenerico = ECampoGenerico.Primeiro_Nome,
                  GetSetComprimentoTextoMax250 = 80,
                  Required = true,
                  Nome = "Primeiro Nome",
                  Label = "Primeiro Nome",
                  PlaceHolder = "Ex: João",
                  ColunasXS_Cadastro = 12,
                  ColunasSM_Cadastro = 12,
                  ColunasMD_Cadastro = 12,
                  ColunasLG_Cadastro = 12,
                  ColunasXL_Cadastro = 12,
                  Generico = true,
                  Linha_CadastroId = dicionarioLinhas["PrimeiroNome"].Linha_CadastroId
              }
            );
            //CPF
            dicionarioCampos.Add("CPF",
              new Campo_Cadastro() {
                  Ativo = true,
                  Ordem_Cadastro = 0,
                  ModeloCampo = EModeloCampo.Texto_250,
                  CampoGenerico = ECampoGenerico.CPF,
                  Required = true,
                  Unico = true,
                  GetSetComprimentoTextoMax250 = 15,
                  Nome = "CPF",
                  Label = "CPF",
                  PlaceHolder = "Ex: xxx.xxx.xxx-xx",
                  ColunasXS_Cadastro = 12,
                  ColunasSM_Cadastro = 12,
                  ColunasMD_Cadastro = 12,
                  ColunasLG_Cadastro = 12,
                  ColunasXL_Cadastro = 12,
                  Generico = true,
                  Linha_CadastroId = dicionarioLinhas["CPF"].Linha_CadastroId
              }
            );
            //Estado
            dicionarioCampos.Add("Estado",
              new Campo_Cadastro() {
                  Ativo = true,
                  Ordem_Cadastro = 0,
                  ModeloCampo = EModeloCampo.Select,
                  CampoGenerico = ECampoGenerico.Estado,
                  Required = true,
                  Nome = "Estado",
                  Label = "Estado",
                  ColunasXS_Cadastro = 12,
                  ColunasSM_Cadastro = 12,
                  ColunasMD_Cadastro = 12,
                  ColunasLG_Cadastro = 12,
                  ColunasXL_Cadastro = 12,
                  Generico = true,
                  Linha_CadastroId = dicionarioLinhas["Estado"].Linha_CadastroId
              }
            );
            await _context.CamposCadastro.AddRangeAsync(dicionarioCampos.Select(c => c.Value));
            await _context.SaveChangesAsync();

            //Atribuir id do campo à linha
            dicionarioLinhas["Email"].IdCampoGenerico = dicionarioCampos["Email"].Campo_CadastroId;
            dicionarioLinhas["Senha"].IdCampoGenerico = dicionarioCampos["Senha"].Campo_CadastroId;
            dicionarioLinhas["ConfirmarSenha"].IdCampoGenerico = dicionarioCampos["ConfirmarSenha"].Campo_CadastroId;
            dicionarioLinhas["PrimeiroNome"].IdCampoGenerico = dicionarioCampos["PrimeiroNome"].Campo_CadastroId;
            dicionarioLinhas["CPF"].IdCampoGenerico = dicionarioCampos["CPF"].Campo_CadastroId;
            dicionarioLinhas["Estado"].IdCampoGenerico = dicionarioCampos["Estado"].Campo_CadastroId;
            _context.LinhasCadastro.UpdateRange(dicionarioLinhas.Select(l => l.Value));
            await _context.SaveChangesAsync();

            List<Select_Item> listaSelectEstados = new List<Select_Item>() {
        new Select_Item() { Nome = "Acre", NomeResumido = "AC", Campo_CadastroId = dicionarioCampos["Estado"].Campo_CadastroId, Generico = true, Ordem = 0 },
        new Select_Item() { Nome = "Alagoas", NomeResumido = "AL", Campo_CadastroId = dicionarioCampos["Estado"].Campo_CadastroId, Generico = true, Ordem = 1 },
        new Select_Item() { Nome = "Amapá", NomeResumido = "AP", Campo_CadastroId = dicionarioCampos["Estado"].Campo_CadastroId, Generico = true, Ordem = 2 },
        new Select_Item() { Nome = "Amazonas", NomeResumido = "AM", Campo_CadastroId = dicionarioCampos["Estado"].Campo_CadastroId, Generico = true, Ordem = 3 },
        new Select_Item() { Nome = "Bahia", NomeResumido = "BA", Campo_CadastroId = dicionarioCampos["Estado"].Campo_CadastroId, Generico = true, Ordem = 4 },
        new Select_Item() { Nome = "Ceará", NomeResumido = "CE", Campo_CadastroId = dicionarioCampos["Estado"].Campo_CadastroId, Generico = true, Ordem = 5 },
        new Select_Item() { Nome = "Distrito Federal", NomeResumido = "DF", Campo_CadastroId = dicionarioCampos["Estado"].Campo_CadastroId, Generico = true, Ordem = 6 },
        new Select_Item() { Nome = "Espírito Santo", NomeResumido = "ES", Campo_CadastroId = dicionarioCampos["Estado"].Campo_CadastroId, Generico = true, Ordem = 7 },
        new Select_Item() { Nome = "Goiás", NomeResumido = "GO", Campo_CadastroId = dicionarioCampos["Estado"].Campo_CadastroId, Generico = true, Ordem = 8 },
        new Select_Item() { Nome = "Maranhão", NomeResumido = "MA", Campo_CadastroId = dicionarioCampos["Estado"].Campo_CadastroId, Generico = true, Ordem = 9 },
        new Select_Item() { Nome = "Mato Grosso", NomeResumido = "MT", Campo_CadastroId = dicionarioCampos["Estado"].Campo_CadastroId, Generico = true, Ordem = 10 },
        new Select_Item() { Nome = "Mato Grosso do Sul", NomeResumido = "MS", Campo_CadastroId = dicionarioCampos["Estado"].Campo_CadastroId, Generico = true, Ordem = 11 },
        new Select_Item() { Nome = "Minas Gerais", NomeResumido = "MG", Campo_CadastroId = dicionarioCampos["Estado"].Campo_CadastroId, Generico = true, Ordem = 12 },
        new Select_Item() { Nome = "Pará", NomeResumido = "PA", Campo_CadastroId = dicionarioCampos["Estado"].Campo_CadastroId, Generico = true, Ordem = 13 },
        new Select_Item() { Nome = "Paraíba", NomeResumido = "PB", Campo_CadastroId = dicionarioCampos["Estado"].Campo_CadastroId, Generico = true, Ordem = 14 },
        new Select_Item() { Nome = "Paraná", NomeResumido = "PR", Campo_CadastroId = dicionarioCampos["Estado"].Campo_CadastroId, Generico = true, Ordem = 15 },
        new Select_Item() { Nome = "Pernambuco", NomeResumido = "PE", Campo_CadastroId = dicionarioCampos["Estado"].Campo_CadastroId, Generico = true, Ordem = 16 },
        new Select_Item() { Nome = "Piauí", NomeResumido = "PI", Campo_CadastroId = dicionarioCampos["Estado"].Campo_CadastroId, Generico = true, Ordem = 17 },
        new Select_Item() { Nome = "Rio de Janeiro", NomeResumido = "RS", Campo_CadastroId = dicionarioCampos["Estado"].Campo_CadastroId, Generico = true, Ordem = 18 },
        new Select_Item() { Nome = "Rio Grande do Norte", NomeResumido = "RN", Campo_CadastroId = dicionarioCampos["Estado"].Campo_CadastroId, Generico = true, Ordem = 19 },
        new Select_Item() { Nome = "Rio Grande do Sul", NomeResumido = "RS", Campo_CadastroId = dicionarioCampos["Estado"].Campo_CadastroId, Generico = true, Ordem = 20 },
        new Select_Item() { Nome = "Rondônia", NomeResumido = "RO", Campo_CadastroId = dicionarioCampos["Estado"].Campo_CadastroId, Generico = true, Ordem = 21 },
        new Select_Item() { Nome = "Roraima", NomeResumido = "RR", Campo_CadastroId = dicionarioCampos["Estado"].Campo_CadastroId, Generico = true, Ordem = 22 },
        new Select_Item() { Nome = "Santa Catarina", NomeResumido = "SC", Campo_CadastroId = dicionarioCampos["Estado"].Campo_CadastroId, Generico = true, Ordem = 23 },
        new Select_Item() { Nome = "São Paulo", NomeResumido = "SP", Campo_CadastroId = dicionarioCampos["Estado"].Campo_CadastroId, Generico = true, Ordem = 24 },
        new Select_Item() { Nome = "Sergipe", NomeResumido = "SE", Campo_CadastroId = dicionarioCampos["Estado"].Campo_CadastroId, Generico = true, Ordem = 25 },
        new Select_Item() { Nome = "Tocantins", NomeResumido = "TO", Campo_CadastroId = dicionarioCampos["Estado"].Campo_CadastroId, Generico = true, Ordem = 26 }
      };
            await _context.SelectItens.AddRangeAsync(listaSelectEstados);
            await _context.SaveChangesAsync();

        }
    }
}
