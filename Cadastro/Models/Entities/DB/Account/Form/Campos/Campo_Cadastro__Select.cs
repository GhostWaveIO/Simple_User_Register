using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Cadastro.Models.ViewModel.Account.Cadastro.Campo;
using Cadastro.Models.Entities.DB.Account.Form.Seletores;

namespace Cadastro.Models.Account.Cadastro.Campos
{

    public partial class Campo_Cadastro {

    private List<Select_Item> listaNovosItens = new List<Select_Item>();

    //#### Criação ####

    //########################################################################################################################
    private async Task Criar_Select(CriarCampoVM ccSe) {
      if(String.IsNullOrWhiteSpace(ccSe.campo.ItensSeletores)) return;

      List<string> listaStringItens = new List<string>(ccSe.campo.ItensSeletores.Split(Environment.NewLine));
      listaStringItens.RemoveAll(l => String.IsNullOrWhiteSpace(l));

      if(listaStringItens.Any(i => i.Length >= 300)) throw new Exception("Um ou mais itens, ultrapassam o limite de 300 caracteres");

      //Cria os itens referentes às strings divididas acima
      int cSel = 0;
      List<Select_Item> listaSeletores = new List<Select_Item>();
      bool selecionadoDefinido = false;
      foreach (string itemStr in listaStringItens) {
        listaSeletores.Add(new Select_Item() {
          Nome = itemStr.Replace("#P#", "").Replace("#p#", "").Trim(),
          Ordem = cSel++,
          PreSelecionado = (itemStr.Contains("#p#") && !selecionadoDefinido),
          Campo_CadastroId = this.Campo_CadastroId
        });

        if(itemStr.Contains("#p#") && !selecionadoDefinido) selecionadoDefinido = true;
      }
      
      await _context.SelectItens.AddRangeAsync(listaSeletores);
      await _context.SaveChangesAsync();
    }

    //########################################################################################################################
    private void VerificarCriacao_Select() {
      if (String.IsNullOrWhiteSpace(this.ItensSeletores)) {
        throw new Exception("Ao menos um item precisa ser informado");
      }
    }

    //########################################################################################################################
    private void CorrigirCriacao_Select() {
      this.Generico = false;
      this.ComprimentoTextoMax = null;
      this.Html = null;
      this.CasasDecimais = 0;
      this.Min = 0;
      this.Max = 0;
      this.TextoPreenchido = null;
      this.NumeroPreenchido = 0;
      this.PlaceHolder = null;
      this.PlaceHolderConfirmarSenha = null;
      this.ExpressaoRegular = null;
    }

    //#### Edição ####

    //########################################################################################################################
    private void VerificarEdicao_Select() {
    }

    //########################################################################################################################
    private void CorrigirEdicao_Select() {
      

    }

    //###################################################################################################
    private void CopyToUpdate_Select(Campo_Cadastro campo) {
      if (!this.Generico) {
        campo.Required = this.Required;
        campo.StartCriacaoCampo = this.StartCriacaoCampo;
      }

      campo.AutorizadoEditar = this.AutorizadoEditar;
      campo.Label = this.Label;
      //campo.Unico = this.Unico;
      //campo.ComprimentoTextoMax = this.ComprimentoTextoMax;
      //campo.CasasDecimais = this.CasasDecimais;
      //campo.Min = this.Min;
      //campo.Max = this.Max;
      //campo.TextoPreenchido = this.TextoPreenchido;
      //campo.NumeroPreenchido = this.NumeroPreenchido;
      //campo.PlaceHolder = this.PlaceHolder;
      //campo.ExpressaoRegular = this.ExpressaoRegular;
      campo.DirecaoEixo = this.DirecaoEixo;
      campo.ItensSeletores_Novos = this.ItensSeletores_Novos;

    }

    //########################################################################################################################
    private async Task Update_Select(EditarCampoVM ecSe) {
      if(String.IsNullOrWhiteSpace(this.ItensSeletores_Novos)) return;

      this.ItensSeletores_Novos = this.ItensSeletores_Novos?.Replace("#P#", "#p#");

      List<Select_Item> listaSeletores = null;
      List<string> listaStringItens = new List<string>(this.ItensSeletores_Novos.Split(Environment.NewLine));
      listaStringItens.RemoveAll(l => String.IsNullOrWhiteSpace(l));

      if (listaStringItens.Any(i => i.Length > 300)) throw new Exception("Um ou mais itens, ultrapassam o limite de 300 caracteres");

      //Cria os itens referentes às strings divididas acima
      int cSel = 0;
      if (this.Selects.Any() && !this.Generico) {
        cSel = this.Selects.Max(s => s.Ordem) + 1;
        listaSeletores = new List<Select_Item>();
        bool selecionadoDefinido = false;
        foreach (string itemStr in listaStringItens) {
          listaSeletores.Add(new Select_Item() {
            Nome = itemStr.Replace("#P#", "").Replace("#p#", "").Trim(),
            Ordem = cSel++,
            PreSelecionado = (itemStr.Contains("#p#") && !selecionadoDefinido),
            Campo_CadastroId = this.Campo_CadastroId
          });

          if (itemStr.Contains("#p#") && !selecionadoDefinido) selecionadoDefinido = true;
        }

        await _context.SelectItens.AddRangeAsync(listaSeletores);
        await _context.SaveChangesAsync();

        if (listaSeletores.Any(l => l.PreSelecionado))
          await listaSeletores.First(l => l.PreSelecionado).PreSelecionar(true);
      }
    }

  }
}
