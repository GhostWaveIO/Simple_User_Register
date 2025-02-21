using Cadastro.Models.Account.Cadastro.Campos;
using Cadastro.Models.Entities.DB.Account.Form.Seletores;
using Cadastro.Models.Entities.DB.Account.Form;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Cadastro.Models.Entities.DB.Account;
using Cadastro.Models.Account.Cadastro.Dados;
using Cadastro.Models.Services.Application.Security.Autorizacao;
using System;

namespace Cadastro.Data {
    public partial class AppDbContext : DbContext {


        ////####################################################################################################
        //public void BuildScaffoldForm(ModelBuilder builder) {

        //    builder.Entity<Usuario>().HasData(
        //        new Usuario() {
        //            UsuarioId = 1,
        //            DataCadastro = DateTime.Today
        //        }
        //    );

        //    Grupo_Cadastro grupo = new Grupo_Cadastro() {
        //        Generico = true,
        //        Linhas = new List<Linha_Cadastro>()
        //    };

        //    grupo.Linhas.Add(new Linha_Cadastro() {
        //        Ordem = 0, Campos =
        //        new List<Campo_Cadastro>() {
        //            new Campo_Cadastro() {
        //                Ativo = true,
        //                Ordem_Cadastro = 0,
        //                ModeloCampo = EModeloCampo.Email,
        //                CampoGenerico = ECampoGenerico.Email,
        //                Required = true,
        //                Unico = true,
        //                Nome = "Email de Login",
        //                Label = "Email",
        //                PlaceHolder = "Ex: usuario@dominio.com.br",
        //                ColunasXS_Cadastro = 12,
        //                ColunasSM_Cadastro = 12,
        //                ColunasMD_Cadastro = 12,
        //                ColunasLG_Cadastro = 12,
        //                ColunasXL_Cadastro = 12,
        //                Generico = true,
        //                Dados = new List<Dado_Cadastro>(){
        //                    new Dado_Cadastro() {
        //                        //Campo_CadastroId = campos.First(c => c.CampoGenerico == ECampoGenerico.Email).Campo_CadastroId,
        //                        //Campo = campos.First(c => c.CampoGenerico == ECampoGenerico.Email),
        //                        Email_Generico = "admin@admin.com.br",
        //                        UsuarioId = 1
        //                    }
        //                }
        //            }
        //        }
        //    });//Email
        //    grupo.Linhas.Add(new Linha_Cadastro() {
        //        Grupo_CadastroId = grupo.Grupo_CadastroId, Ordem = 1, Campos =
        //        new List<Campo_Cadastro>() {
        //            new Campo_Cadastro() {
        //                Ativo = true,
        //                Ordem_Cadastro = 0,
        //                ModeloCampo = EModeloCampo.Senha,
        //                CampoGenerico = ECampoGenerico.Senha,
        //                Required = true,
        //                Nome = "Senha",
        //                Label = "Senha",
        //                PlaceHolder = "Digite uma senha",
        //                ColunasXS_Cadastro = 12,
        //                ColunasSM_Cadastro = 12,
        //                ColunasMD_Cadastro = 12,
        //                ColunasLG_Cadastro = 12,
        //                ColunasXL_Cadastro = 12,
        //                Generico = true,
        //                Dados = new List<Dado_Cadastro>() {
        //                    new Dado_Cadastro() {
        //                        //Campo_CadastroId = campos.First(c => c.CampoGenerico == ECampoGenerico.Senha).Campo_CadastroId,
        //                        //Campo = campos.First(c => c.CampoGenerico == ECampoGenerico.Senha),
        //                        Senha_Generico = "admin12345",
        //                        UsuarioId = 1
        //                    }
        //                }
        //            }
        //        }
        //    });//Senha

        //    grupo.Linhas.Add(new Linha_Cadastro() {
        //        Grupo_CadastroId = grupo.Grupo_CadastroId, Ordem = 2, Campos =
        //        new List<Campo_Cadastro>() {
        //            new Campo_Cadastro() {
        //                Ativo = true,
        //                Ordem_Cadastro = 0,
        //                ModeloCampo = EModeloCampo.Confirmar_Senha,
        //                CampoGenerico = ECampoGenerico.ConfirmarSenha,
        //                Required = true,
        //                Nome = "Confirmar Senha",
        //                Label = "Confirmar Senha",
        //                PlaceHolder = "Confirme a Senha",
        //                ColunasXS_Cadastro = 12,
        //                ColunasSM_Cadastro = 12,
        //                ColunasMD_Cadastro = 12,
        //                ColunasLG_Cadastro = 12,
        //                ColunasXL_Cadastro = 12,
        //                Generico = true,
        //                Dados = new List<Dado_Cadastro>(){
        //                    new Dado_Cadastro() {
        //                        //Campo_CadastroId = campos.First(c => c.CampoGenerico == ECampoGenerico.ConfirmarSenha).Campo_CadastroId,
        //                        //Campo = campos.First(c => c.CampoGenerico == ECampoGenerico.ConfirmarSenha),
        //                        ConfirmarSenha = "admin12345",
        //                        UsuarioId = 1
        //                    }
        //                }
        //            }
        //        }
        //    });//ConfirmarSenha

        //    grupo.Linhas.Add(new Linha_Cadastro() {
        //        Grupo_CadastroId = grupo.Grupo_CadastroId, Ordem = 3, Campos =
        //        new List<Campo_Cadastro>() {
        //            new Campo_Cadastro() {
        //                Ativo = true,
        //                Ordem_Cadastro = 0,
        //                ModeloCampo = EModeloCampo.Texto_250,
        //                CampoGenerico = ECampoGenerico.Primeiro_Nome,
        //                GetSetComprimentoTextoMax250 = 80,
        //                Required = true,
        //                Nome = "Primeiro Nome",
        //                Label = "Primeiro Nome",
        //                PlaceHolder = "Ex: João",
        //                ColunasXS_Cadastro = 12,
        //                ColunasSM_Cadastro = 12,
        //                ColunasMD_Cadastro = 12,
        //                ColunasLG_Cadastro = 12,
        //                ColunasXL_Cadastro = 12,
        //                Generico = true,
        //                Dados = new List<Dado_Cadastro>() {
        //                    new Dado_Cadastro() {
        //                        //Campo_CadastroId = campos.First(c => c.CampoGenerico == ECampoGenerico.Primeiro_Nome).Campo_CadastroId,
        //                        //Campo = campos.First(c => c.CampoGenerico == ECampoGenerico.Primeiro_Nome),
        //                        Texto250 = "Admin",
        //                        UsuarioId = 1
        //                    }       
        //                }
        //            }
        //        }
        //    });//PrimeiroNome

        //    grupo.Linhas.Add(new Linha_Cadastro() {
        //        Grupo_CadastroId = grupo.Grupo_CadastroId, Ordem = 4, Campos =
        //        new List<Campo_Cadastro>() {
        //            new Campo_Cadastro() {
        //                Ativo = true,
        //                Ordem_Cadastro = 0,
        //                ModeloCampo = EModeloCampo.Texto_250,
        //                CampoGenerico = ECampoGenerico.CPF,
        //                Required = true,
        //                Unico = true,
        //                GetSetComprimentoTextoMax250 = 15,
        //                Nome = "CPF",
        //                Label = "CPF",
        //                PlaceHolder = "Ex: xxx.xxx.xxx-xx",
        //                ColunasXS_Cadastro = 12,
        //                ColunasSM_Cadastro = 12,
        //                ColunasMD_Cadastro = 12,
        //                ColunasLG_Cadastro = 12,
        //                ColunasXL_Cadastro = 12,
        //                Generico = true,
        //                Dados = new List<Dado_Cadastro>() {
        //                    new Dado_Cadastro() {
        //                        //Campo_CadastroId = campos.First(c => c.CampoGenerico == ECampoGenerico.CPF).Campo_CadastroId,
        //                        //Campo = campos.First(c => c.CampoGenerico == ECampoGenerico.CPF),
        //                        Cpf_Generico = "123.456.789-10",
        //                        UsuarioId = 1
        //                    }
        //                }
        //            }
        //        }
        //    });//CPF

        //    Select_Item selectSp = new Select_Item() { Nome = "São Paulo", NomeResumido = "SP", Generico = true, Ordem = 24 };
        //    Campo_Cadastro campoEstado = new Campo_Cadastro() {
        //        Ativo = true,
        //        Ordem_Cadastro = 0,
        //        ModeloCampo = EModeloCampo.Select,
        //        CampoGenerico = ECampoGenerico.Estado,
        //        Required = true,
        //        Nome = "Estado",
        //        Label = "Estado",
        //        ColunasXS_Cadastro = 12,
        //        ColunasSM_Cadastro = 12,
        //        ColunasMD_Cadastro = 12,
        //        ColunasLG_Cadastro = 12,
        //        ColunasXL_Cadastro = 12,
        //        Generico = true,
        //        Dados = new List<Dado_Cadastro>() {

        //        },
        //        Selects = new List<Select_Item>() {
        //            new Select_Item() { Nome = "Acre", NomeResumido = "AC", Generico = true, Ordem = 0 },
        //            new Select_Item() { Nome = "Alagoas", NomeResumido = "AL", Generico = true, Ordem = 1 },
        //            new Select_Item() { Nome = "Amapá", NomeResumido = "AP", Generico = true, Ordem = 2 },
        //            new Select_Item() { Nome = "Amazonas", NomeResumido = "AM", Generico = true, Ordem = 3 },
        //            new Select_Item() { Nome = "Bahia", NomeResumido = "BA", Generico = true, Ordem = 4 },
        //            new Select_Item() { Nome = "Ceará", NomeResumido = "CE", Generico = true, Ordem = 5 },
        //            new Select_Item() { Nome = "Distrito Federal", NomeResumido = "DF", Generico = true, Ordem = 6 },
        //            new Select_Item() { Nome = "Espírito Santo", NomeResumido = "ES", Generico = true, Ordem = 7 },
        //            new Select_Item() { Nome = "Goiás", NomeResumido = "GO", Generico = true, Ordem = 8 },
        //            new Select_Item() { Nome = "Maranhão", NomeResumido = "MA", Generico = true, Ordem = 9 },
        //            new Select_Item() { Nome = "Mato Grosso", NomeResumido = "MT", Generico = true, Ordem = 10 },
        //            new Select_Item() { Nome = "Mato Grosso do Sul", NomeResumido = "MS", Generico = true, Ordem = 11 },
        //            new Select_Item() { Nome = "Minas Gerais", NomeResumido = "MG", Generico = true, Ordem = 12 },
        //            new Select_Item() { Nome = "Pará", NomeResumido = "PA", Generico = true, Ordem = 13 },
        //            new Select_Item() { Nome = "Paraíba", NomeResumido = "PB", Generico = true, Ordem = 14 },
        //            new Select_Item() { Nome = "Paraná", NomeResumido = "PR", Generico = true, Ordem = 15 },
        //            new Select_Item() { Nome = "Pernambuco", NomeResumido = "PE", Generico = true, Ordem = 16 },
        //            new Select_Item() { Nome = "Piauí", NomeResumido = "PI", Generico = true, Ordem = 17 },
        //            new Select_Item() { Nome = "Rio de Janeiro", NomeResumido = "RS", Generico = true, Ordem = 18 },
        //            new Select_Item() { Nome = "Rio Grande do Norte", NomeResumido = "RN", Generico = true, Ordem = 19 },
        //            new Select_Item() { Nome = "Rio Grande do Sul", NomeResumido = "RS", Generico = true, Ordem = 20 },
        //            new Select_Item() { Nome = "Rondônia", NomeResumido = "RO", Generico = true, Ordem = 21 },
        //            new Select_Item() { Nome = "Roraima", NomeResumido = "RR", Generico = true, Ordem = 22 },
        //            new Select_Item() { Nome = "Santa Catarina", NomeResumido = "SC", Generico = true, Ordem = 23 },
        //            selectSp,
        //            new Select_Item() { Nome = "Sergipe", NomeResumido = "SE", Generico = true, Ordem = 25 },
        //            new Select_Item() { Nome = "Tocantins", NomeResumido = "TO", Generico = true, Ordem = 26 }
        //        }
        //    };
        //    grupo.Linhas.Add(new Linha_Cadastro() {
        //        Grupo_CadastroId = grupo.Grupo_CadastroId, Ordem = 5, Campos =
        //        new List<Campo_Cadastro>() {
        //            campoEstado
        //        }
        //    });//Estado

        //    builder.Entity<Grupo_Cadastro>().HasData(
        //        grupo
        //    );


        //    Dado_Cadastro dadoEstado = new Dado_Cadastro() {
        //        //Campo_CadastroId = campos.First(c => c.CampoGenerico == ECampoGenerico.Estado).Campo_CadastroId,
        //        //Campo = campos.First(c => c.CampoGenerico == ECampoGenerico.Estado),
        //        Selected = new Select_Selected() { 
        //            Select_ItemId = selectSp.Select_ItemId
        //        },
        //        UsuarioId = 1
        //    };

        //    builder.Entity<Grupo_Cadastro>().HasData(dadoEstado);

        //}


        //////####################################################################################################
        ////public void BuildAdmin(ModelBuilder builder) {
        ////    builder.Entity<Dado_Cadastro>().HasData(new Dado_Cadastro() {

        ////    });

        ////}


        ////####################################################################################################
        //public void BuildPermissions(ModelBuilder builder) {
        //    builder.Entity<Hierarquia>().HasData(
        //        new Hierarquia() { Titulo = "Novos Usuários", PermissaoHierarquia = new PermissaoHierarquia(), HierarquiaId = 1 },
        //        new Hierarquia() { Titulo = "Presidente", PermissaoHierarquia = new PermissaoHierarquia(), HierarquiaId = 2 }
        //    );
        //}
    }
}
