using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace Cadastro.Models.Account.Cadastro.Dados {
  public partial class Dado_Cadastro {

    //Coleção de itens selecionados fica na Partial Principal
    [NotMapped]
    public List<int> MultiSelects { get; set; }

    private void Preparar_MultiSelect() {

    }

    //#######################################################################################################################
    private void VerificarCriacao_MultiSelect() {

    }

    //#######################################################################################################################
    private void CorrigirCriacao_MultiSelect() {

    }

    //#######################################################################################################################
    private void Criar_MultiSelect() {

    }

    //#######################################################################################################################
    private void VerificarEdicao_MultiSelect() {

    }

    //#######################################################################################################################
    private void CorrigirEdicao_MultiSelect() {

    }

    //#######################################################################################################################
    private void CopyToUpdate_MultiSelect(Dado_Cadastro dado) {

    }

    //#######################################################################################################################
    private void Update_MultiSelect() {

    }

  }
}
