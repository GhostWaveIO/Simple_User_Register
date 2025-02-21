using Cadastro.Data;
using Cadastro.Models.Account.Cadastro.Dados;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Cadastro.Models.ViewModel.Account.Cadastro.Campo;
using Cadastro.Models.Entities.DB.Account;
using Cadastro.Models.Entities.DB.Account.Form;
using Cadastro.Models.Entities.DB.Account.Form.Seletores;

namespace Cadastro.Models.Account.Cadastro.Campos
{

    public enum EModeloCampo {
        Texto_250,
        Texto_Longo,
        Senha,
        Confirmar_Senha,
        Email,
        Número,
        Número_Monetário,
        Select,
        MultiSelect,
        CheckBox,
        RadioButton,
        Imagem,
        Documento,
        Html,
        Vídeo_Youtube
    }

    public enum ECampoGenerico {
        Campo_Personalizado,
        Email,
        Senha,
        ConfirmarSenha,
        Primeiro_Nome,
        CPF,
        Estado
    }

    /// <summary>Define onde o campo será criado "Cadastro ou Painel"</summary>
    public enum EStartCriacaoCampo { Cadastro, Perfil }
    /// <summary>Usado nos modelos de CheckBox e RadioButton para especificar a direção dos itens. (Vertical ou Horizontal)</summary>
    public enum EDirecaoEixo { Vertical, Horizontal }

    // /// <summary> Define a quem poderá ver o Campo após Startado </summary>
    //public enum EPermissaoVisualizar { Usuario_Podera_Editar, Somente_Autorizado }

    /// <summary>Permite a quem o campo será editado "Usuário e Autorizado, Somente Autorizado ou Não Editável"</summary>
    public enum EAutorizadoEditar {
        Proprietário_e_Autorizado,
        Somente_Proprietário,
        Somente_Autorizado,
        Não_Editável
    }


    public partial class Campo_Cadastro {

        public AppDbContext _context { get; set; }

        [Display(Name = "Campo")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public int Campo_CadastroId { get; set; }


        [Display(Name = "Ativo")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public bool Ativo { get; set; }

        //Modelo do Cmapo
        /// <summary> Modelo deste Campo </summary>
        [Display(Name = "Modelo do Campo")]
        public EModeloCampo ModeloCampo { get; set; }

        //Modelo do campo
        [NotMapped]
        [Display(Name = "Modelo do Campo")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public EModeloCampo? ModeloCampo_Nullable { get; set; }

        //Campo Genérico
        /// <summary> Usado somente para campos genéricos, Especifica o tipo de "Campo Genérico" </summary>
        public ECampoGenerico CampoGenerico { get; set; }

        ////Campo de Nome Completo
        //[Display(Name = "Usar como Nome Completo")]
        //public bool CampoNomeCompleto { get; set; }


        //Permite o campo de ser pesquisado
        [Display(Name = "Permite Pesquisar Usuários por este Campo")]
        public bool PermitirPesquisa { get; set; }

        //Start Criação
        /// <summary> Define onde o campo será Startado "Cadastro ou Perfil" </summary>
        [Display(Name = "Local de inicialização")]
        public EStartCriacaoCampo StartCriacaoCampo { get; set; }

        //Autorizado a Editar
        /// <summary> Define quem terá a permissão para edição dos dados coletados </summary>
        [Display(Name = "Autorizado a Editar Dados Coletados")]
        public EAutorizadoEditar AutorizadoEditar { get; set; }

        //Criptografar dados
        [Display(Name = "Criptografar Campo")]
        [Required(ErrorMessage = "Campo obrigatório!")]
        public bool Criptografar { get; set; }

        //Label do Campo
        [Display(Name = "Nome de Identificação")]
        [StringLength(150, ErrorMessage = "Máximo {1} caracteres")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public string Nome { get; set; }

        //Label do Campo
        [Display(Name = "Label do Campo")]
        [StringLength(150, ErrorMessage = "Máximo {1} caracteres")]
        public string Label { get; set; }

        //Genérico
        /// <summary>Define se este campo é genérico</summary>
        public bool Generico { get; set; }

        //Rquired (Obrigatório)
        /// <summary>Define se o campo será obrigatório</summary>
        [Display(Name = "Campo Obrigatório")]
        public bool Required { get; set; }

        //Único (Poderá Repetir)
        /// <summary>Define se só poderá ter um cadastro usando este mesmo dado informado. Não poderá repetir este dado em outro cadastro</summary>
        [Display(Name = "Único")]
        public bool Unico { get; set; }

        //Comprimento texto - 250
        /// <summary>Usável somente para campos de texto</summary>
        public int? ComprimentoTextoMax { get; private set; }

        //Comprimento do Texto
        /// <summary>Seta o comprimento do texto - 250</summary>
        [NotMapped]
        [Display(Name = "Comprimento do texto")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [Range(0, 250, ErrorMessage = "Entre {1} e {2}")]
        public int GetSetComprimentoTextoMax250 {
            get { return ComprimentoTextoMax ?? 0; }

            set { this.ComprimentoTextoMax = value; }
        }

        //Comprimento do Texto - 3000
        /// <summary>Seta o comprimento do texto - 3000</summary>
        [NotMapped]
        [Display(Name = "Comprimento do texto")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [Range(0, 3000, ErrorMessage = "Entre {1} e {2}")]
        public int GetSetComprimentoTextoMax3000 {
            get { return ComprimentoTextoMax ?? 0; }

            set { this.ComprimentoTextoMax = value; }
        }

        //Html
        [Display(Name = "Conteúdo Html")]
        [StringLength(3000, ErrorMessage = "Máximo {1} caracteres")]
        public string Html { get; set; }

        //Casas Decimais
        [Display(Name = "Quantidade de Casas Decimais")]
        [Range(1, 8, ErrorMessage = "Entre {1} e {2}")]
        public byte CasasDecimais { get; set; }

        //Mínimo
        /// <summary>Define o valor Mínimo númerico. Usado no modelo Número</summary>
        [Display(Name = "Mínimo")]
        [Range(float.MinValue, float.MaxValue, ErrorMessage = "Entre {1} e {2}")]
        public float? Min { get; set; }

        //Máximo
        /// <summary>Define o valor Máximo númerico. Usado no modelo Número</summary>
        [Display(Name = "Máximo")]
        [Range(float.MinValue, float.MaxValue, ErrorMessage = "Entre {1} e {2}")]
        public float? Max { get; set; }

        //Texto Preenchido
        /// <summary> Esta propriedade tem por objetivo Preencher campos como input text para ser pré-preenchido antes de apresentar </summary>
        //TextoPreenchido
        [Display(Name = "Conteúdo Pré-Preenchido")]
        [StringLength(3000, MinimumLength = 0, ErrorMessage = "Entre {2} e {1} caracteres")]
        public string TextoPreenchido { get; set; }

        //Número Preenchido
        [Display(Name = "Conteúdo Pré-Preenchido")]
        [RegularExpression("^[0-9 ]+$", ErrorMessage = "Somente números")]
        [Range(float.MinValue, float.MaxValue, ErrorMessage = "Entre {1} e {2}")]
        public float? NumeroPreenchido { get; set; }

        //Tamanho máximo do arquivo
        /// <summary> Defina o tamanho máximo em MB </summary>
        [Display(Name = "Tamanho Máximo do Arquivo")]
        [Range(0.001f, 250f, ErrorMessage = "Valor entre {1} e {2}")]
        public float TamanhoArquivo { get; set; }

        //Formatos Aceitos
        /// <summary> Especifica os formatos aceitos no upload separados por vírgula. ex: ".pdf, .img, .gif" </summary>
        [Display(Name = "Formatos Aceitos")]
        [StringLength(80, ErrorMessage = "Máximo {1} caracteres")]
        public string FormatosArquivo { get; set; }

        [NotMapped]
        [Display(Name = "Formatos Aceitos")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(80, ErrorMessage = "Máximo {1} caracteres")]
        public string FormatosArquivo_Required {
            get { return FormatosArquivo; }
            set { FormatosArquivo = value; }
        }

        //PlaceHolder
        /// <summary> Apresenta um PlaceHolder .Usável somente em campos de texto ou número </summary>
        [Display(Name = "PlaceHolder (Exemplo do Campo)")]
        [StringLength(200, MinimumLength = 0, ErrorMessage = "Entre {2} e {1} caracteres")]
        public string PlaceHolder { get; set; }

        //PlaceHolder Confirmar Senha
        /// <summary>Usado no Modelo Confirmar Senha</summary>
        [StringLength(100, MinimumLength = 0, ErrorMessage = "Entre {2} e {1} caracteres")]
        public string PlaceHolderConfirmarSenha { get; set; }

        // Expressão Regular
        [Display(Name = "Expressão Regular")]
        [StringLength(200, MinimumLength = 0, ErrorMessage = "Entre {2} e {1} caracteres")]
        public string ExpressaoRegular { get; set; }

        //#### Colunas do Cadastro ####

        //Colunas XS - Cadastro
        [Display(Name = "Colunas XS no Cadastro (Extra Pequeno <576px)")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Somente números")]
        [Range(1, 12, ErrorMessage = "Entre {1} e {2} colunas")]
        public byte ColunasXS_Cadastro { get; set; }

        //Colunas SM - Cadastro
        [Display(Name = "Colunas SM no Cadastro (Pequeno ≥576px)")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Somente números")]
        [Range(0, 12, ErrorMessage = "Entre {1} e {2} colunas")]
        public byte ColunasSM_Cadastro { get; set; }

        //Colunas MD - Cadastro
        [Display(Name = "Colunas MD no Cadastro (Médio ≥768px)")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Somente números")]
        [Range(0, 12, ErrorMessage = "Entre {1} e {2} colunas")]
        public byte ColunasMD_Cadastro { get; set; }

        //Colunas LG - Cadastro
        [Display(Name = "Colunas LG no Cadastro (Grande ≥992px)")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Somente números")]
        [Range(0, 12, ErrorMessage = "Entre {1} e {2} colunas")]
        public byte ColunasLG_Cadastro { get; set; }

        //Colunas XL - Cadastro
        [Display(Name = "Colunas XL no Cadastro (Extra Grande ≥1200px)")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Somente números")]
        [Range(0, 12, ErrorMessage = "Entre {1} e {2} colunas")]
        public byte ColunasXL_Cadastro { get; set; }

        //#### Colunas do Perfil ####

        //Colunas XS - Perfil
        [Display(Name = "Colunas XS no Perfil (Extra Pequeno <576px)")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Somente números")]
        [Range(0, 12, ErrorMessage = "Entre {1} e {2} colunas")]
        public byte ColunasXS_Perfil { get; set; }

        //Colunas SM - Perfil
        [Display(Name = "Colunas SM no Perfil (Pequeno ≥576px)")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Somente números")]
        [Range(0, 12, ErrorMessage = "Entre {1} e {2} colunas")]
        public byte ColunasSM_Perfil { get; set; }

        //Colunas MD - Perfil
        [Display(Name = "Colunas MD no Perfil (Médio ≥768px)")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Somente números")]
        [Range(0, 12, ErrorMessage = "Entre {1} e {2} colunas")]
        public byte ColunasMD_Perfil { get; set; }

        //Colunas LG - Perfil
        [Display(Name = "Colunas LG no Perfil (Grande ≥992px)")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Somente números")]
        [Range(0, 12, ErrorMessage = "Entre {1} e {2} colunas")]
        public byte ColunasLG_Perfil { get; set; }

        //Colunas XL - Perfil
        [Display(Name = "Colunas XL no Perfil (Extra Grande ≥1200px)")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Somente números")]
        [Range(0, 12, ErrorMessage = "Entre {1} e {2} colunas")]
        public byte ColunasXL_Perfil { get; set; }

        //Ordem - Cadastro
        [Display(Name = "Ordem do Campo")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [RegularExpression("^[0-9]+", ErrorMessage = "Somente números e maiores ou igual a 0")]
        [Range(0, int.MaxValue, ErrorMessage = "Entre {1} e {2}")]
        public int Ordem_Cadastro { get; set; }

        //Ordem - Perfil
        [Display(Name = "Ordem do Campo")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [RegularExpression("^[0-9]+", ErrorMessage = "Somente números e maiores ou igual a 0")]
        [Range(0, int.MaxValue, ErrorMessage = "Entre {1} e {2}")]
        public int Ordem_Perfil { get; set; }

        //Direção do Eixo. Usado em campos do tipo TextBox e radioButton
        [Display(Name = "Direção dos Itens")]
        public EDirecaoEixo DirecaoEixo { get; set; }

        /* Coletor de itens dos seletores na criação */
        /// <summary> Coletor de itens para o seletor deste campo informados pelo usuário autorizado </summary>
        [NotMapped]
        [Display(Name = "Itens")]
        [Required(ErrorMessage = "Informe ao menos 1 item")]
        public string ItensSeletores { get; set; }

        /* Coletor de itens dos seletores na edicao */
        /// <summary> Coletor de itens para o seletor deste campo informados pelo usuário autorizado </summary>
        [NotMapped]
        [Display(Name = "Novos Itens")]
        public string ItensSeletores_Novos { get; set; }


        #region Entidades

        /// <summary> Esta propriedade tem por objetivo ser útil somente nesta sessão (é "descartável") </summary>
        [NotMapped]
        public Dado_Cadastro Dado_ThisSession { get; set; }

        public List<Dado_Cadastro> Dados { get; set; }

        public int Linha_CadastroId { get; set; }
        public Linha_Cadastro Linha_Cadastro { get; set; }

        //Seletores
        public List<Select_Item> Selects { get; set; }
        public List<CheckBox_Item> CheckBoxes { get; set; }
        public List<RadioButton_Item> RadioButtons { get; set; }


        #endregion FIM| Entidades

        //##################################################################################################################

        public void GerarNovoDado() {
            if (this.Dados?.Any() ?? false) return;
            List<Dado_Cadastro> res = new List<Dado_Cadastro>(){
        new Dado_Cadastro(){ Campo = this, Campo_CadastroId = this.Campo_CadastroId }
      };

            res[0].PrepararDado(this.ModeloCampo);
            this.Dados = res;


        }

        //##################################################################################################################
        /// <summary>Gera um novo Dado e completa caso tenha sido informado via url</summary>
        /// <param name="autoComplete">Dados que foram informados via url para auto completar</param>
        public void GerarNovoDado(Dictionary<int, string> autoComplete) {
            GerarNovoDado();

            //Auto Preenche campo com url externa
            int numero;
            float numeroMonetario;
            foreach (KeyValuePair<int, string> d in autoComplete) {
                if (d.Key != Campo_CadastroId) continue;

                if (ModeloCampo == EModeloCampo.Texto_250 || ModeloCampo == EModeloCampo.Texto_Longo) {
                    Dados.First().Texto250 = d.Value;
                    Dados.First().Texto250_Required = d.Value;
                    Dados.First().Cpf_Generico = d.Value;
                    Dados.First().TextoLongo = d.Value;
                    Dados.First().TextoLongo_Required = d.Value;
                } else if (ModeloCampo == EModeloCampo.Número) {
                    if (!int.TryParse(d.Value, out numero)) continue;
                    Dados.First().Numero = numero;
                    Dados.First().Numero_Required = numero;
                } else if (ModeloCampo == EModeloCampo.Número_Monetário) {
                    if (!float.TryParse(d.Value, out numeroMonetario)) continue;
                    Dados.First().NumeroMonetario = numeroMonetario;
                    Dados.First().NumeroMonetario_Required = numeroMonetario;
                }

                break;
            }
        }

        //##################################################################################################################
        public async Task Criar(CriarCampoVM ccSe) {
            if (_context == null) throw new Exception("Erro interno. Contexto não informado");

            //Salva as informações do campo
            await _context.CamposCadastro.AddAsync(this);
            await _context.SaveChangesAsync();

            switch (this.ModeloCampo) {
                case EModeloCampo.Select:
                    await this.Criar_Select(ccSe);
                    break;
                case EModeloCampo.CheckBox:
                    await this.Criar_CheckBox(ccSe);
                    break;
                case EModeloCampo.RadioButton:
                    await this.Criar_RadioButton(ccSe);
                    break;
            }

        }

        //##################################################################################################################
        public void VerificarCriacao(CriarCampoVM ccSe) {

            //Verificação geral


            //Aciona a verificação de acordo com o modelo do campo
            switch (this.ModeloCampo) {
                case EModeloCampo.Texto_250:
                    this.VerificarCriacao_Texto250();
                    break;
                case EModeloCampo.Texto_Longo:
                    this.VerificarCriacao_TextoLongo();
                    break;
                case EModeloCampo.Email:
                    this.VerificarCriacao_Email();
                    break;
                case EModeloCampo.Número:
                    this.VerificarCriacao_Numero();
                    break;
                case EModeloCampo.Número_Monetário:
                    this.VerificarCriacao_NumeroMonetario();
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
                case EModeloCampo.Html:
                    this.VerificarCriacao_Html();
                    break;
                case EModeloCampo.Vídeo_Youtube:
                    this.VerificarCriacao_VideoYoutube();
                    break;
                default:
                    throw new Exception("Este modelo de campo não existe");
            }
        }

        //##################################################################################################################
        public void CorrigirCriacao(CriarCampoVM ccSe) {

            //Correção do modelo global
            this.CampoGenerico = ECampoGenerico.Campo_Personalizado;
            this.Generico = false;

            if (this.ColunasXS_Cadastro == 0) this.ColunasXS_Cadastro = 0;



            switch (this.ModeloCampo) {
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
                case EModeloCampo.Html:
                    this.CorrigirCriacao_Html();
                    break;
                case EModeloCampo.Vídeo_Youtube:
                    this.CorrigirCriacao_VideoYoutube();
                    break;
                default:
                    throw new Exception("Este modelo de campo não existe");
            }
        }

        //##################################################################################################################
        public void VerificarEdicao(EditarCampoVM ecSe) {
            switch (this.ModeloCampo) {
                case EModeloCampo.Texto_250:
                    this.VerificarEdicao_Texto250();
                    break;
                case EModeloCampo.Texto_Longo:
                    this.VerificarEdicao_TextoLongo();
                    break;
                case EModeloCampo.Email:
                    this.VerificarEdicao_Email();
                    break;
                case EModeloCampo.Número:
                    this.VerificarEdicao_Numero();
                    break;
                case EModeloCampo.Número_Monetário:
                    this.VerificarEdicao_NumeroMonetario();
                    break;
                case EModeloCampo.Select:
                    this.VerificarEdicao_Select();
                    break;
                case EModeloCampo.MultiSelect:
                    this.VerificarEdicao_MultiSelect();
                    break;
                case EModeloCampo.CheckBox:
                    this.VerificarEdicao_CheckBox();
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
                case EModeloCampo.Html:
                    this.VerificarEdicao_Html();
                    break;
                case EModeloCampo.Vídeo_Youtube:
                    this.VerificarEdicao_VideoYoutube();
                    break;
            }
        }

        //##################################################################################################################
        public void CorrigirEdicao(EditarCampoVM ecSe) {
            switch (this.ModeloCampo) {
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
                case EModeloCampo.Html:
                    this.CorrigirEdicao_Html();
                    break;
                case EModeloCampo.Vídeo_Youtube:
                    this.CorrigirEdicao_VideoYoutube();
                    break;
            }
        }

        //##################################################################################################################
        public void CopyToUpdate(Campo_Cadastro campo) {

            //Para Geral
            if (this.Generico || this.CampoGenerico != ECampoGenerico.Campo_Personalizado) {
                campo.Ativo = true;
            } else if (!this.Generico && this.CampoGenerico == ECampoGenerico.Campo_Personalizado) {
                campo.Ativo = this.Ativo;
                campo.StartCriacaoCampo = this.StartCriacaoCampo;
                campo.Nome = this.Nome;
            }

            //Senha e Confirmar Senha
            switch (this.ModeloCampo) {
                case EModeloCampo.Senha:
                case EModeloCampo.Confirmar_Senha:
                    campo.Label = this.Label;
                    break;
            }
            campo.ColunasXS_Cadastro = this.ColunasXS_Cadastro;
            campo.ColunasSM_Cadastro = this.ColunasSM_Cadastro;
            campo.ColunasMD_Cadastro = this.ColunasMD_Cadastro;
            campo.ColunasLG_Cadastro = this.ColunasLG_Cadastro;
            campo.ColunasXL_Cadastro = this.ColunasXL_Cadastro;
            campo.Ordem_Cadastro = this.Ordem_Cadastro;
            campo.ColunasXS_Perfil = this.ColunasXS_Perfil;
            campo.ColunasSM_Perfil = this.ColunasSM_Perfil;
            campo.ColunasMD_Perfil = this.ColunasMD_Perfil;
            campo.ColunasLG_Perfil = this.ColunasLG_Perfil;
            campo.ColunasXL_Perfil = this.ColunasXL_Perfil;
            campo.Ordem_Perfil = this.Ordem_Perfil;

            switch (this.ModeloCampo) {
                case EModeloCampo.Texto_250:
                    this.CopyToUpdate_Texto250(campo);
                    break;
                case EModeloCampo.Texto_Longo:
                    this.CopyToUpdate_TextoLongo(campo);
                    break;
                case EModeloCampo.Email:
                    this.CopyToUpdate_Email(campo);
                    break;
                case EModeloCampo.Número:
                    this.CopyToUpdate_Numero(campo);
                    break;
                case EModeloCampo.Número_Monetário:
                    this.CopyToUpdate_NumeroMonetario(campo);
                    break;
                case EModeloCampo.Select:
                    this.CopyToUpdate_Select(campo);
                    break;
                case EModeloCampo.MultiSelect:
                    this.CopyToUpdate_MultiSelect(campo);
                    break;
                case EModeloCampo.CheckBox:
                    this.CopyToUpdate_CheckBox(campo);
                    break;
                case EModeloCampo.RadioButton:
                    this.CopyToUpdate_RadioButton(campo);
                    break;
                case EModeloCampo.Imagem:
                    this.CopyToUpdate_Imagem(campo);
                    break;
                case EModeloCampo.Documento:
                    this.CopyToUpdate_Documento(campo);
                    break;
                case EModeloCampo.Html:
                    this.CopyToUpdate_Html(campo);
                    break;
                case EModeloCampo.Vídeo_Youtube:
                    this.CopyToUpdate_VideoYoutube(campo);
                    break;
            }
        }



        //##################################################################################################################
        public async Task Update(EditarCampoVM ecSe) {

            if (_context == null) throw new Exception("Erro interno. Contexto não informado");

            _context.CamposCadastro.Update(this);
            await _context.SaveChangesAsync();


            switch (this.ModeloCampo) {
                case EModeloCampo.Select:
                    await this.Update_Select(ecSe);
                    this.CorrigirEdicao_Select();
                    break;
                case EModeloCampo.CheckBox:
                    await this.Update_CheckBox(ecSe);
                    this.CorrigirEdicao_CheckBox();
                    break;
                case EModeloCampo.RadioButton:
                    await this.Update_RadioButton(ecSe);
                    this.CorrigirEdicao_RadioButton();
                    break;
                case EModeloCampo.MultiSelect:
                    this.Update_MultiSelect();
                    this.CorrigirEdicao_MultiSelect();
                    break;
            }
        }

        //##################################################################################################################
        public async Task ReferenciarDadosUsuarioAsync(Usuario usuario) {
            if (!await _context.CamposCadastro.ContainsAsync(this)) throw new Exception("Este campo não existe!");

            this.Dados = usuario.Dados.Where(d => d.Campo_CadastroId == this.Campo_CadastroId).ToList();
            foreach (Dado_Cadastro dado in this.Dados) {
                if (this.ModeloCampo == EModeloCampo.CheckBox && !dado.CheckBoxes.Any()) dado.PrepararDado(EModeloCampo.CheckBox);
            }
        }

        //##################################################################################################################
        //Retorna um Campo genérico à sua linha genérica de origem
        public async Task DeportarCampoGenerico() {
            if (_context == null) throw new Exception("Erro interno. Contexto não informado.");

            Linha_Cadastro linhaNativa = await _context.LinhasCadastro.SingleAsync(l => l.IdCampoGenerico == this.Campo_CadastroId);
            this.Linha_CadastroId = linhaNativa.Linha_CadastroId;

            this.Ordem_Cadastro = 0;
            this.Ordem_Perfil = 0;
            this.ColunasXS_Cadastro = 12;
            this.ColunasSM_Cadastro = 12;
            this.ColunasMD_Cadastro = 12;
            this.ColunasLG_Cadastro = 12;
            this.ColunasXL_Cadastro = 12;
            this.ColunasXS_Perfil = 0;
            this.ColunasSM_Perfil = 0;
            this.ColunasMD_Perfil = 0;
            this.ColunasLG_Perfil = 0;
            this.ColunasXL_Perfil = 0;

            _context.CamposCadastro.Update(this);
            await _context.SaveChangesAsync();
        }

        //##################################################################################################################
        public async Task Remover() {
            if (_context == null) throw new Exception("Erro interno. Contexto não informado.");
            if (this.Generico) throw new Exception("Campos genéricos não podem ser removidos!");

            List<Dado_Cadastro> dadosRelacionados = await _context.DadosCadastro.Where(d => d.Campo_CadastroId == this.Campo_CadastroId).ToListAsync();
            if (dadosRelacionados.Any()) {
                _context.DadosCadastro.RemoveRange(Dados);
                await _context.SaveChangesAsync();
            }

            _context.CamposCadastro.Remove(this);
            await _context.SaveChangesAsync();
        }

        //##################################################################################################################
        /// <summary> Retorna de 1 a 12 de acordo com o size e local especificado nos parâmetros, nunca retorna 0. Seria a quantidade de colunas ocupadas pelo campo. </summary>
        /// <param name="size">Tamanhos baseados no Bootstrap "xs, sm, md, lg, xl"</param>
        /// <param name="local">c: Cadastro. p: Perfil.</param>
        public int GetCols(string size, char local) {
            size = size.ToLower();
            int res = 0;
            bool definidoC = false;
            bool definidoP = false;

            //coleta a coluna baseada nas propriedades de Colunas 

            //#### Cadastro ####
            //XS - Cadastro
            if (!definidoC) res = this.ColunasXS_Cadastro;
            if (size == "xs") definidoC = true;

            //SM - Cadastro
            if (!definidoC && ColunasSM_Cadastro != 0) res = this.ColunasSM_Cadastro;
            if (size == "sm") definidoC = true;

            //MD - Cadastro
            if (!definidoC && ColunasMD_Cadastro != 0) res = this.ColunasMD_Cadastro;
            if (size == "md") definidoC = true;

            //LG - Cadastro
            if (!definidoC && ColunasLG_Cadastro != 0) res = this.ColunasLG_Cadastro;
            if (size == "lg") definidoC = true;

            //XL - Cadastro
            if (!definidoC && ColunasXL_Cadastro != 0) res = this.ColunasXL_Cadastro;



            //#### Perfil ####
            if (local == 'p') {
                //XS - Perfil
                if (!definidoP && ColunasXS_Perfil != 0) res = this.ColunasXS_Perfil;
                if (size == "xs") definidoP = true;

                //SM - Perfil
                if (!definidoP && ColunasSM_Perfil != 0) res = this.ColunasSM_Perfil;
                if (size == "sm") definidoP = true;

                //MD - Cadastro
                if (!definidoP && ColunasMD_Perfil != 0) res = this.ColunasMD_Perfil;
                if (size == "md") definidoP = true;

                //LG - Cadastro
                if (!definidoP && ColunasLG_Perfil != 0) res = this.ColunasLG_Perfil;
                if (size == "lg") definidoP = true;

                //XL - Cadastro
                if (!definidoP && ColunasXL_Perfil != 0) res = this.ColunasLG_Perfil;

            }


            return res;
        }

    }
}
