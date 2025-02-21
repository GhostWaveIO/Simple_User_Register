using Cadastro.Data;
using Cadastro.Models.Account.Cadastro.Campos;
using Cadastro.Models.Account.Cadastro.Dados;
using Cadastro.Models.Entities.DB.Account.Email;
using Cadastro.Models.Entities.DB.Account.Form;
using Cadastro.Models.Entities.DB.Account.Form.Seletores;
using Cadastro.Models.Entities.DB.Account.Password;
using Cadastro.Models.Entities.DB.Account.Remove;
using Cadastro.Models.Services.Application.Security.Autorizacao;
using Cadastro.Models.Services.Application.Security.Cryptography.Rijndael;
using Cadastro.Models.Services.Application.Text.Generators.Hash;
using Cadastro.Models.Services.Application.Text.Generators.Identificadores;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cadastro.Models.Entities.DB.Account {

    public enum EStatus { Ativo, Inativo, Apto, Inapto, Observação, Aguardando, Suspenso, Excluído, Indisciplinado, Inadimplente, Inválido, Vencido }
    public enum EEstadoCivil { Solteiro, Casado, Viúvo, Divorciado, União_Estável }
    public enum ESexo { Masculino, Feminino/*, Personalizado*/ }//trocar "Lista" por "E"

    public class Usuario {

        public AppDbContext _context { get; set; }

        [NotMapped]
        private Criptografia cripto { get; set; }

        public Usuario() { }
        public Usuario(AppDbContext context) { _context = context; }

        #region Propriedades
        //UsuarioId
        [Key]
        public long UsuarioId { get; set; }


        //Status
        public EStatus Status { get; set; }//Definir uma classe

        //Código de Usuário
        [StringLength(15)]
        public string CodigoUsuario { get; set; }

        //Data de criação da conta
        public DateTime DataCadastro { get; set; }

        ////Inativo para ser Removido
        //public bool InactiveToRemove { get; set; }


        //Coleta o Email
        public string GetEmail {
            get {
                if(!Dados?.Any(d => d.Campo != null) ?? true)
                    return null;
                return Dados?.FirstOrDefault(d => d.Campo.CampoGenerico == ECampoGenerico.Email)?.Email;
            }
        }

        //Retorna a senha convertida para MD5
        public string GetSenhaMD5 {
            get {
                if(Dados == null)
                    return null;
                string senha = Dados.FirstOrDefault(d => d.Campo.CampoGenerico == ECampoGenerico.Senha)?.Senha;
                return new GeradorMD5().GerarMD5(senha);
            }
        }

        //Retorna o nome
        public string GetNome {
            get { return Dados?.FirstOrDefault(d => d.Campo.CampoGenerico == ECampoGenerico.Primeiro_Nome)?.Texto250; }
        }

        //Retorna o CPF
        public string GetCpf {
            get { return Dados?.FirstOrDefault(d => d.Campo.CampoGenerico == ECampoGenerico.CPF)?.Texto250; }
        }

        //Retorna o CPF com Máscara ex: xxx.xxx.xxx-xx
        public string GetCpfComMascara {
            get { return Dados?.FirstOrDefault(d => d.Campo.CampoGenerico == ECampoGenerico.CPF)?.GetCpfComMascara(); }
        }

        //Retorna o Estado
        public string GetEstado {
            get { return Dados?.FirstOrDefault(d => d.Campo.CampoGenerico == ECampoGenerico.Estado)?.Selected?.Select_Item?.Nome; }
        }

        //Retorna a Sigla Estado
        public string GetSiglaEstado {
            get { return Dados?.FirstOrDefault(d => d.Campo.CampoGenerico == ECampoGenerico.Estado)?.Selected?.Select_Item?.NomeResumido; }
        }






        //Ip de Último Acesso
        [StringLength(150)]
        public string IpUltimoAcesso { get; set; }

        #endregion

        #region Entidades

        //Dados de Cadastro
        public List<Dado_Cadastro> Dados { get; set; }

        //Confirmação de E-mail
        public List<EmailValidado> EmailsValidados { get; set; }

        //Autorização
        public List<HierarquiaUsuario> HierarquiasUsuario { get; set; }
        public List<FuncaoUsuario> FuncoesUsuario { get; set; }

        //Recuperacao
        public List<NovaSenhaSolicitada> NovasSenhasSolicidadas { get; set; }


        //Remove Account
        public List<AuthToRemove> AuthsToRemove { get; set; }

        #endregion

        #region getters e setters

        //retorna o primeiro nome
        public string GetFirstName {
            get { return GetNome; }
        }

        //Retorna o nome completo
        public string GetNomeCompleto {
            get {
                string res = null;
                res = Dados?.FirstOrDefault(d => d.Campo?.CampoGenerico == ECampoGenerico.Primeiro_Nome)?.Texto250;
                return res;
            }
        }

        public string GetSobrenome {
            get { return ""; }
        }

        public string GetCpfPodado {
            get { return PodarTexto(GetCpf); }
        }

        #endregion |Fim getters e setters

        #region Métodos

        //############################################################################################################################################
        public async Task Criar(List<Dado_Cadastro> dados) {

            try {
                await _context.Usuarios.AddAsync(this);
                await _context.SaveChangesAsync();

                HierarquiaUsuario hu = new HierarquiaUsuario() { HierarquiaId = 1, UsuarioId = UsuarioId };
                await _context.HierarquiasUsuario.AddAsync(hu);
                await _context.SaveChangesAsync();

                List<Dado_Cadastro> listaDados = dados;
                foreach(Dado_Cadastro d in listaDados) {
                    if(d.Campo.ModeloCampo == EModeloCampo.Confirmar_Senha)
                        continue;
                    d.UsuarioId = UsuarioId;
                    await d.Criar();
                }
            } catch(Exception err) {
                throw err;
            }

        }

        //############################################################################################################################################
        public async Task VerificarCriacao(List<Dado_Cadastro> dados) {
            if(_context == null)
                throw new Exception("Erro interno. Contexto não informado.");

            Dado_Cadastro dadoAtual = null;
            List<Dado_Cadastro> dadosReferentes = null;

            IEnumerable<Campo_Cadastro> campos = await _context.CamposCadastro.
              Include(c => c.Selects).
              Include(c => c.CheckBoxes).
              Include(c => c.RadioButtons).
              ToListAsync();
            //Verifica se todos dados de cada campo foram informados
            foreach(Campo_Cadastro campo in campos) {
                if(!campo.Ativo || campo.StartCriacaoCampo == EStartCriacaoCampo.Perfil)
                    continue;
                //para selects caso não exista opções
                if(!campo.Selects.Any() && campo.ModeloCampo == EModeloCampo.Select)
                    continue;
                //para CheckBoxs caso não exista opções
                if(!campo.CheckBoxes.Any() && campo.ModeloCampo == EModeloCampo.CheckBox)
                    continue;
                //para RadioButtons caso não exista opções
                if(!campo.RadioButtons.Any() && campo.ModeloCampo == EModeloCampo.RadioButton)
                    continue;

                dadosReferentes = dados.Where(d => d.Campo_CadastroId == campo.Campo_CadastroId).ToList();
                if(dadosReferentes.Count() != 1)
                    throw new Exception("Informação Ausente. Tente novamente mais tarde!");
                //Atribui o dado referente ao campo atual
                dadoAtual = dadosReferentes.First();
                dadoAtual._context = _context;
                dadoAtual.Campo = campo;

                //No caso dos selects- para posteriormente coletar o ItemId informado
                if(dadoAtual.Campo.ModeloCampo == EModeloCampo.Select && dadoAtual.Selected?.Select_ItemId != null && campo.Selects.Any())
                    dadoAtual.Selected.Select_Item = campo.Selects.Single(s => s.Select_ItemId == dadoAtual.Selected.Select_ItemId);
            }

            foreach(Dado_Cadastro dado in dados)
                await dado.VerificarCriacao(ELocalDeOperacao.Cadastro);
        }

        //############################################################################################################################################
        public async Task CorrigirCriacao(List<Dado_Cadastro> dados) {
            if(_context == null)
                throw new Exception("Erro interno. Contexto não informado.");

            Dado_Cadastro dadoEstado = dados.Single(d => d.Campo.CampoGenerico == ECampoGenerico.Estado);
            string strEstado = dadoEstado.Selected.GetNomeResumido();

            CodigoUsuario = await GeradorCodigoUsuario.Gerar(_context, strEstado);
            DataCadastro = DateTime.Now;
            Status = EStatus.Apto;

            //Adicionar usuário à Permissão "Novos Usuários"

            foreach(Dado_Cadastro dado in dados) {
                dado.CorrigirCriacao();
                if(dado.Campo.Criptografar)
                    dado.Criptografar();
            }



        }

        //############################################################################################################################################
        //Retorna se Já existe uma conta com este CPF
        public async Task<bool> CpfExists() {
            if(_context == null)
                throw new Exception("_contexto não informado");

            IEnumerable<Usuario> usuarios = await _context.Usuarios.Include(u => u.Dados).ThenInclude(d => d.Campo).ToListAsync();

            foreach(Usuario u in usuarios) {
                if(PodarTexto(u.GetCpf) == PodarTexto(GetCpf) && u.UsuarioId != UsuarioId)
                    return true;
            }

            return false;
        }

        //############################################################################################################################################
        public string PodarTexto(string texto) {
            if(string.IsNullOrEmpty(texto))
                return texto;
            return texto.Replace(" ", "").Replace("-", "").Replace(".", "").Replace("_", "").Replace("/", "").Replace("\\", "").ToLower().Trim();
        }

        //############################################################################################################################################
        public void CriptografarDados() {
            if(cripto == null)
                cripto = new Criptografia();

        }

        //############################################################################################################################################
        public void DescriptografarDados() {
            if(cripto == null)
                cripto = new Criptografia();
            foreach(Dado_Cadastro d in Dados) {
                d.Descriptografar();
            }

        }

        //############################################################################################################################################
        public async Task<bool> VerificarLimiteValidacao() {
            if(_context == null)
                throw new Exception("_contexto não informado!");

            //Contas validadas serão ignoradas
            if(await _context.EmailsValidados.Where(e => e.UsuarioId == UsuarioId).AnyAsync(e => e.Confirmado) || IsAdmin())
                return false;

            //Se usuário não validou após 24 horas, a conta será removida
            DateTime tempoLimite = DataCadastro.AddHours(Program.Config.PrazoConfirmacaoCadastro);
            if(DateTime.Now >= tempoLimite && !IsAdmin()) {
                await RemoverUsuario();
                return true;
            }

            return false;
        }

        //############################################################################################################################################
        //Coleta dados começando de grupos/Linhas/Campos/Dados_do_usuários e suas referências
        public async Task<List<Grupo_Cadastro>> GetGruposEReferenciasAsync() {
            if(_context == null)
                throw new Exception("Erro interno. Contexto não informado!");
            if(!await _context.Usuarios.ContainsAsync(this))
                throw new Exception("Este usuário não existe!");

            List<Grupo_Cadastro> grupos = await _context.GruposCadastro.
              Include(g => g.Linhas).ThenInclude(l => l.Campos).ThenInclude(c => c.Selects).
              Include(g => g.Linhas).ThenInclude(l => l.Campos).ThenInclude(c => c.CheckBoxes).
              Include(g => g.Linhas).ThenInclude(l => l.Campos).ThenInclude(c => c.RadioButtons).
              //AsNoTracking().
              ToListAsync();

            grupos = grupos.OrderBy(g => g.Ordem).ToList();
            foreach(Grupo_Cadastro grupo in grupos) {
                grupo.Linhas = grupo.Linhas.OrderBy(g => g.Ordem).ToList();
                foreach(Linha_Cadastro linha in grupo.Linhas) {
                    linha.Campos = linha.Campos.OrderBy(c => c.Ordem_Perfil).ToList();
                    foreach(Campo_Cadastro campo in linha.Campos) {
                        campo._context = _context;
                        await campo.ReferenciarDadosUsuarioAsync(this);
                        if(!campo.Dados.Any())
                            campo.GerarNovoDado();
                        if(campo.CampoGenerico == ECampoGenerico.ConfirmarSenha || campo.CampoGenerico == ECampoGenerico.Senha)
                            campo.Dados[0].Senha = "";
                    }
                }
            }

            foreach(Dado_Cadastro dado in Dados) {
                dado.CheckBoxes = dado.CheckBoxes.OrderBy(ck => ck.CheckBox_Item.Ordem).ToList();
            }

            return grupos;
        }

        //############################################################################################################################################
        public async Task CriarAdmin() {
            if(_context == null)
                throw new Exception("Erro interno. Contexto não informado!");

            //Verifica se Admin existe
            if(await _context.DadosCadastro.Include(d => d.Campo).AnyAsync(d => d.Campo.CampoGenerico == ECampoGenerico.Email && d.Email == Program.Config.emailPrincipal))
                return;

            IEnumerable<Campo_Cadastro> campos = null;

            if(!await _context.CamposCadastro.AnyAsync())
                throw new Exception("Não existe nenhum campo criado");
            campos = await _context.CamposCadastro.Include(c => c.Selects).Where(c => c.CampoGenerico != ECampoGenerico.Campo_Personalizado).ToListAsync();


            Usuario admin = new Usuario() {
                DataCadastro = DateTime.Today
            };

            await _context.Usuarios.AddAsync(admin);
            await _context.SaveChangesAsync();

            List<Dado_Cadastro> dados = new List<Dado_Cadastro>();
            //Email
            dados.Add(new Dado_Cadastro() {
                Campo_CadastroId = campos.First(c => c.CampoGenerico == ECampoGenerico.Email).Campo_CadastroId,
                Campo = campos.First(c => c.CampoGenerico == ECampoGenerico.Email),
                Email_Generico = Program.Config.emailPrincipal,
                UsuarioId = admin.UsuarioId
            });
            //Senha
            dados.Add(new Dado_Cadastro() {
                Campo_CadastroId = campos.First(c => c.CampoGenerico == ECampoGenerico.Senha).Campo_CadastroId,
                Campo = campos.First(c => c.CampoGenerico == ECampoGenerico.Senha),
                Senha_Generico = "admin12345",
                UsuarioId = admin.UsuarioId
            });
            //Confirmar Senha
            dados.Add(new Dado_Cadastro() {
                Campo_CadastroId = campos.First(c => c.CampoGenerico == ECampoGenerico.ConfirmarSenha).Campo_CadastroId,
                Campo = campos.First(c => c.CampoGenerico == ECampoGenerico.ConfirmarSenha),
                ConfirmarSenha = "admin12345",
                UsuarioId = admin.UsuarioId
            });
            //Primeiro Nome
            dados.Add(new Dado_Cadastro() {
                Campo_CadastroId = campos.First(c => c.CampoGenerico == ECampoGenerico.Primeiro_Nome).Campo_CadastroId,
                Campo = campos.First(c => c.CampoGenerico == ECampoGenerico.Primeiro_Nome),
                Texto250 = "SUR",
                UsuarioId = admin.UsuarioId
            });
            //CPF
            dados.Add(new Dado_Cadastro() {
                Campo_CadastroId = campos.First(c => c.CampoGenerico == ECampoGenerico.CPF).Campo_CadastroId,
                Campo = campos.First(c => c.CampoGenerico == ECampoGenerico.CPF),
                Cpf_Generico = "123.456.789-10",
                UsuarioId = admin.UsuarioId
            });
            //Estado
            dados.Add(new Dado_Cadastro() {
                Campo_CadastroId = campos.First(c => c.CampoGenerico == ECampoGenerico.Estado).Campo_CadastroId,
                Campo = campos.First(c => c.CampoGenerico == ECampoGenerico.Estado),
                Selected = new Select_Selected() { Select_ItemId = campos.First(c => c.CampoGenerico == ECampoGenerico.Estado).Selects.First(s => s.NomeResumido == "MG").Select_ItemId },
                UsuarioId = admin.UsuarioId
            });

            try {
                foreach(Dado_Cadastro dado in dados) {
                    dado._context = _context;
                    dado.CorrigirCriacao();
                }

                await _context.DadosCadastro.AddRangeAsync(dados);
                await _context.SaveChangesAsync();
            } catch(Exception err) {
                await admin.RemoverUsuario();
                throw err;
            }
        }

        //############################################################################################################################################
        public Dado_Cadastro GetDado_CadastroByCampo_CadastroId(int campo_CadastroId) {
            Dado_Cadastro res = Dados?.FirstOrDefault(d => d.Campo_CadastroId == campo_CadastroId);
            return res;
        }

        //############################################################################################################################################
        public bool IsAdmin() {
            bool res = GetEmail == Program.Config.emailPrincipal;

            return res;
        }

        //############################################################################################################################################
        public async Task RemoverUsuario() {

            Usuario res = await _context.Usuarios.
              Include(u => u.Dados).ThenInclude(d => d.Campo).
              Include(u => u.Dados).ThenInclude(d => d.Selected).
              Include(u => u.Dados).ThenInclude(d => d.CheckBoxes).
              Include(u => u.Dados).ThenInclude(d => d.RadioButton_Checked).
              Include(u => u.EmailsValidados).
              Include(u => u.HierarquiasUsuario).
              Include(u => u.FuncoesUsuario).
              Include(u => u.NovasSenhasSolicidadas).
              FirstOrDefaultAsync(u => u.UsuarioId == UsuarioId);

            if(IsAdmin())
                return;

            string file = null;
            //Remover arquivos enviados
            foreach(Dado_Cadastro dado in res.Dados.Where(d => d.Campo.ModeloCampo == EModeloCampo.Imagem || d.Campo.ModeloCampo == EModeloCampo.Documento)) {
                if(!string.IsNullOrEmpty(dado.Local)) {
                    file = Path.Combine(Environment.CurrentDirectory,
                        "wwwroot",
                        "Files",
                        "Cadastro",
                        dado.Campo.ModeloCampo == EModeloCampo.Imagem ? "Imagens" : "Documentos",
                        dado.Campo_CadastroId.ToString(),
                        dado.Local
                        );
                }
            }

            //Romover Selecteds
            if(res.Dados?.Any(d => d.Selected != null) ?? false) {
                _context.SelectSelectedDados.RemoveRange(res.Dados.Where(d => d.Selected != null).Select(d => d.Selected));
            }

            //Remover CheckBoxes
            List<CheckBox_Checked> checks = new List<CheckBox_Checked>();
            if(res.Dados?.Any(d => d.CheckBoxes != null) ?? false) {
                res.Dados.Where(d => d.CheckBoxes.Any()).Select(d => d.CheckBoxes).ToList().ForEach(ck => checks.AddRange(ck));
                _context.CheckboxCheckedDados.RemoveRange(checks);
            }

            //Remover RadioButtons
            if(res.Dados?.Any(d => d.RadioButton_Checked != null) ?? false) {
                _context.RadioButtonCheckedDados.RemoveRange(res.Dados.Where(d => d.RadioButton_Checked != null).Select(d => d.RadioButton_Checked));
            }

            //Remover Dados
            if(res.Dados.Any())
                _context.DadosCadastro.RemoveRange(res.Dados);

            _context.EmailsValidados.RemoveRange(res.EmailsValidados);
            _context.HierarquiasUsuario.RemoveRange(res.HierarquiasUsuario);
            _context.FuncoesUsuario.RemoveRange(res.FuncoesUsuario);
            _context.NovasSenhasSolicidatas.RemoveRange(res.NovasSenhasSolicidadas);
            _context.Usuarios.Remove(this);

            //Corrige concorrência
            bool falha;
            do {
                falha = false;
                try {
                    await _context.SaveChangesAsync();
                } catch(DbUpdateConcurrencyException ex) {
                    falha = true;
                    if(ex.Entries.Any())
                        await ex.Entries.Single().ReloadAsync();
                    else
                        throw ex;
                }
            } while(falha);


        }

        #endregion


    }
}
