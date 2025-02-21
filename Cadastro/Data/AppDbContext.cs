using Cadastro.Models.Account.Cadastro.Campos;
using Cadastro.Models.Account.Cadastro.Dados;
using Cadastro.Models.Entities.DB.Account;
using Cadastro.Models.Entities.DB.Account.Email;
using Cadastro.Models.Entities.DB.Account.Form;
using Cadastro.Models.Entities.DB.Account.Form.Seletores;
using Cadastro.Models.Entities.DB.Account.Password;
using Cadastro.Models.Entities.DB.Account.Remove;
using Cadastro.Models.Services.Application.Security.Autorizacao;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Cadastro.Data {
    public partial class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        //Define Email como "Unique"
        protected override void OnModelCreating(ModelBuilder modelBuilder) {

            //Abaixo são chamadas para evitar remoção redundante
            modelBuilder.Entity<Select_Item>()
                .HasMany(e => e.Selecteds)
                .WithOne(e => e.Select_Item)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CheckBox_Item>()
                .HasMany(e => e.CheckBoxes)
                .WithOne(e => e.CheckBox_Item)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RadioButton_Item>()
                .HasMany(e => e.RadioButtons)
                .WithOne(e => e.RadioButton_Item)
                .OnDelete(DeleteBehavior.Restrict);


            //BuildScaffoldForm(modelBuilder);
            //BuildPermissions(modelBuilder);
        }

        //Dados do Usuário
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<EmailValidado> EmailsValidados { get; set; }
        public DbSet<Grupo_Cadastro> GruposCadastro { get; set; }
        public DbSet<Linha_Cadastro> LinhasCadastro { get; set; }
        public DbSet<Campo_Cadastro> CamposCadastro { get; set; }
        public DbSet<Dado_Cadastro> DadosCadastro { get; set; }
        public DbSet<Select_Item> SelectItens { get; set; }
        public DbSet<CheckBox_Item> CheckBoxItens { get; set; }
        public DbSet<RadioButton_Item> RadioButtonItens { get; set; }
        public DbSet<Select_Selected> SelectSelectedDados { get; set; }
        public DbSet<CheckBox_Checked> CheckboxCheckedDados { get; set; }
        public DbSet<RadioButton_Checked> RadioButtonCheckedDados { get; set; }

        //Remove Account
        public DbSet<AuthToRemove> AuthsToRemove { get; set; }


        //Hierarquia
        public DbSet<Hierarquia> Hierarquias { get; set; }
        public DbSet<HierarquiaUsuario> HierarquiasUsuario { get; set; }
        public DbSet<PermissaoHierarquia> PermissoesHierarquias { get; set; }

        //Permissões
        public DbSet<Funcao> Funcoes { get; set; }
        public DbSet<FuncaoUsuario> FuncoesUsuario { get; set; }
        public DbSet<PermissaoFuncao> PermissoesFuncoes { get; set; }

        //Recuperação
        public DbSet<NovaSenhaSolicitada> NovasSenhasSolicidatas { get; set; }


        

    }
}
