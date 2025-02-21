/* #### Como usar #### */
//O informe a classe "file-input" no input[type="file"] para receber os eventos automaticamente
//crie um input[hidden="hidden"] para cada atributo e implemente em seu value os valores com as seguintes classes
//file-required = "true" para obrigatório e "false" para não-brigatório
//file-max-size = tamanho máximo permitido em bits
//file-extensions = extensões aceitas. Ex: ".jpg, .png, .gif"
//Crie um span[class="file-msg"] que terá a msg de validação

var countCallFiles = 0;
var loopFiles = false;


//-------------------------------------------------------------------------------------------------
function validationFileChange(item) {
  validationFile(item, null);
  countCallFiles = 5;
}

//-------------------------------------------------------------------------------------------------
function validationFileBlur(item) {
  if (!loopFiles) {
    loopFiles = true;
  }
  countCallFiles = 10;
  validationFile(item, null);
  setTimeout(function () { callElementsFile(null) }, 400);
}

//-------------------------------------------------------------------------------------------------
function validationFile(item, e) {

  if (item.disabled == true) return;

  let elFile = item;
  let elParent = elFile;

  let elRequireds = null;
  let elMaxSize = null;
  let elExtensions = null;
  let elValMsg = null;

  let check = false;


  //Coleta os elementos com informações
  let cE = 0;
  do {
    elParent = elParent.parentNode;
    elRequireds = elParent.getElementsByClassName("file-required");
    elMaxSize = elParent.getElementsByClassName("file-max-size");
    elExtensions = elParent.getElementsByClassName("file-extensions");
    elValMsg = elParent.getElementsByClassName("file-msg");
    cE++;
  } while (cE <= 4 && elMaxSize.length == 0);


  //Verifica se algum item foi informado
  if (elMaxSize[0] == undefined || (elFile.value == "" && elRequireds[0].value == "false")) {
    setfileValidationMessage(elValMsg, "");
    return;
  }

  //consulta se os verificadores foram encontrados "maxSize", "extensions"
  if (elMaxSize[0] != undefined && elExtensions[0] != undefined) {

    if (!validationFileSize(elFile, elMaxSize)) {//Verifica se o tamanho do arquivo é válido
      setfileValidationMessage(elValMsg, "Tamanho máximo permitido " + returnFileSize(elMaxSize[0].value));
    } else if (!validationFileExtensions(elFile, elExtensions)) {//Verifica se formato de arquivo é válido
      setfileValidationMessage(elValMsg, "Formato de arquivo inválido. Formatos aceitos (" + elExtensions[0].value + ")");
    } else if (!validationFileRequired(elFile, elRequireds)) {//Verifica se item obrigatório foi informado (caso seja obrigatório)
      setfileValidationMessage(elValMsg, "Campo obrigatório");
    } else {
      setfileValidationMessage(elValMsg, "");
      check = true;
    }
    if (!check && e != null) {
      e.preventDefault();
    }
  }
}


//-------------------------------------------------------------------------------------------------
function validationFileSize(elFile, elMaxSize) {
  if (elFile.files[0] == undefined) return true;

  if (elFile.files[0].size > elMaxSize[0].value)
    return false;

  return true;
}

//-------------------------------------------------------------------------------------------------
function validationFileExtensions(elFile, elExtensions) {
  if (elFile == undefined || elExtensions[0] == undefined) return true;

  let div = elExtensions[0].value.replace("*\g", "").trim().split(",");

  let extFile = elFile.value.split(".");
  
  if (extFile.length > 1)
    extFile = "."+extFile[extFile.length - 1];
  else
    return true;

  for (let c = 0; c < div.length; c++) {
    if (div[c].trim() == extFile.trim())
      return true;
  }
  
  return false;
}

//-------------------------------------------------------------------------------------------------
function validationFileRequired(elFile, elRequireds) {
  if (elRequireds[0] == undefined) return true;

  return (elFile.value != "" || elRequireds.value == "false");
}

//-------------------------------------------------------------------------------------------------
function setfileValidationMessage(objMsg, msg) {
  if (objMsg[0] == undefined) return;
  objMsg[0].innerHTML = msg;
}

//-------------------------------------------------------------------------------------------------
//Chama todos elementos files
function callElementsFile(e = null) {
  let files = document.getElementsByClassName("file-input");
  
  for (let c = 0; c < files.length; c++) {

    setTimeout(function () { validationFile(files[c], e) }, 1);
  }

  if (countCallFiles > 0 && loopFiles) {
    setTimeout(function () { callElementsFile(e) }, 400);
    countCallFiles--
  } else {
    countCallFiles = 0;
    loopFiles = false;
  }
}


//Define Evento nos inputs file
var elFiles = document.getElementsByClassName("file-input");
for (let c = 0; c < elFiles.length; c++) {
  elFiles[c].addEventListener("change", function () { validationFileChange(elFiles[c]); });
  elFiles[c].addEventListener("blur", function () { validationFileBlur(elFiles[c]); });
  if (elFiles[c].hasAttribute("extensions-accept"))
    elFiles[c].setAttribute("accept", elFiles[c].getAttribute("extensions-accept"));
}

//Define evento no botão submit
var btnSubmit = document.querySelector("#submit");
btnSubmit.addEventListener("click", function (e) { countCallFiles = 10; loopFiles = true; callElementsFile(e); });

//-------------------------------------------------------------------------------------------------
//retorna a conversão explicita do tamanho do arquivo
function returnFileSize(bits) {
  if (bits < 1024) {
    return bits + 'bytes';
  } else if (bits >= 1024 && bits < 1048576) {
    return (bits / 1024).toFixed(1) + 'KB';
  } else if (bits >= 1048576) {
    return (bits / 1048576).toFixed(1) + 'MB';
  }
}