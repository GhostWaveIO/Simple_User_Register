//#### Como usar ####
//crie um div com a class "required-field-check" e dentro vai cada lista de checkboxes
//Crie um item span com a class "required-validation-check" para exibição de erros de validação
//itens com a classe "disabled" receberão "disabled=true" e não serão verificados
//Defina o botão de submit com o id "submit"

//Verifica se um item do checkbox foi selecionado (somente obrigatórios)
var checkBoxesRequired = document.getElementsByClassName("required-field-check");
var itensCB = null;

function checkBoxSelected(item) {
  res = false;
  itensCB = item.querySelectorAll("input[type='checkbox']");
  let errorMessage = null;

  for (let i = 0; i < itensCB.length; i++) {
    if (itensCB[i].hasAttribute("errorMessage"))
      if (itensCB[i].getAttribute("errorMessage") != "")
        errorMessage = itensCB[i].getAttribute("errorMessage");
    if (itensCB[i].disabled == true) return true;
    if (itensCB[i].checked) {
      res = true;
      break;
    }
  }
  //Invoca mensagem de erro na ValidationMessage
  if (res) {
    item.getElementsByClassName("required-validation-check")[0].innerHTML = "";
  } else {
    item.getElementsByClassName("required-validation-check")[0].innerHTML = errorMessage??"Selecione uma opção";
  }
  //Aciona o Foco no CB não selecionado
  if (!res && itensCB.length > 0) itensCB[0].focus();

  return res;
}

//Coletar CheckBoxs Obrigatórios
const btn = document.querySelector("#submit");
btn.addEventListener("click", function (e) {
  let pass = true;
  //Verifica cada checkbox obrigatório
  for (let cb = 0; cb < checkBoxesRequired.length; cb++) {
    if (!checkBoxSelected(checkBoxesRequired[cb])) {
      pass = false;
      break;
    }
  }
  if (!pass) {
    e.preventDefault();
  }
  
});


//--------------------------------------------------------------------------------------------------------------------------------------------
var elDisableds = document.getElementsByClassName("disabled");
for (let ed = 0; ed < elDisableds.length; ed++) {
  elDisableds[ed].disabled = true;
}